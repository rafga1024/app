using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherWeb.Models;
using WeatherWeb.ViewModels;

namespace WeatherWeb.Controllers
{
    public class HomeController : Controller
    {
        private List<Location> listLocation = new List<Location>();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                String query = $"{Service.ServerName.GetServerName()}/api/login";
                using (var client = new HttpClient())
                {
                    string myJonson = "{'login':'" + model.login + "','password':'" + model.password + "','email':'" + model.email + "','isAdmin':0}";
                    var response = await client.PostAsync(query, new StringContent(myJonson, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.UserMessage = "User has been registered";

                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.NotAcceptable))
                    {
                        ViewBag.UserMessage = "There is already a user with such an email";
                    }
                }
            }
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Weather()
        {
            List<OpenWeather> weather = new List<OpenWeather>();
            String query = $"{Service.ServerName.GetServerName()}/api/weather";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(query);
                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                {
                    using (HttpContent content = response.Result.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        var w = (List<OpenWeather>)JsonConvert.DeserializeObject<List<OpenWeather>>(result);
                        if (w.Any())
                        {
                            listLocation.Clear();
                            weather = w;

                            foreach (var tmpW in weather)
                            {
                                listLocation.Add(new Location() { City = tmpW.name, Country = tmpW.sys.country });
                            }
                        }
                    }
                }

            }
            return View(weather);

        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WeatherUpdate(LoginViewModel model)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    String queryDB = $"{Service.ServerName.GetServerName()}/api/weather";
                    await Weather();
                    foreach (var location in listLocation)
                    {
                        string apiKey = "54441d6cfe7816fd54b59aadc10b9034";
                        string apiResponse = String.Empty;
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            string query = $"http://api.openweathermap.org/data/2.5/weather?q={location.City},{location.Country}&appid={apiKey}";
                            var response = await client.GetAsync(query);
                            if (response.IsSuccessStatusCode)
                            {
                                apiResponse = await response.Content.ReadAsStringAsync();
                            }
                        }
                        String StrinAuth = $"{model.Login}:{model.Password}";

                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            var authBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(StrinAuth));
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authBase64);
                            var response = await client.PutAsync(queryDB, new StringContent(apiResponse, Encoding.UTF8, "application/json"));
                            if (response.IsSuccessStatusCode)
                            {
                                ViewBag.UserMessage = "Successfully updated";
                                continue;
                            }
                            else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                            {
                                ViewBag.UserMessage = "Incorrect Login or Password";
                                break;
                            }
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        public IActionResult WeatherUpdate()
        {

            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewLocationWeather(Location location)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    String queryDB = $"{Service.ServerName.GetServerName()}/api/weather";
                    string apiKey = "54441d6cfe7816fd54b59aadc10b9034";
                    string apiResponse = String.Empty;
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        string query = $"http://api.openweathermap.org/data/2.5/weather?q={location.City},{location.Country}&appid={apiKey}";
                        var response = await client.GetAsync(query);
                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                        }
                        else if(response.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                        {
                            ViewBag.UserMessage = "Not found location";
                            return View();
                        }

                    }
                    String StrinAuth = $"{location.Login}:{location.Password}";

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        var authBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(StrinAuth));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authBase64);
                        var response = await client.PostAsync(queryDB, new StringContent(apiResponse, Encoding.UTF8, "application/json"));
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Weather", "Home");
                        }
                        else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                        {
                            ViewBag.UserMessage = "Incorrect Login or Password";
                        }
                        else if (response.StatusCode.Equals(System.Net.HttpStatusCode.NotAcceptable))
                        {
                            ViewBag.UserMessage = $"There is already a location {location.City}";
                        }
                    }
                }
            }
            return View();
        }

        [Authorize]
        public IActionResult AddNewLocationWeather()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WeatherDelete(Location location)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    string apiResponse = String.Empty;
                    String queryDB = $"{Service.ServerName.GetServerName()}/api/weather";
                    String StrinAuth = $"{location.Login}:{location.Password}";

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        var authBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(StrinAuth));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authBase64);
                        var response = await client.DeleteAsync($"{queryDB}?city={location.City}&country={location.Country}");
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Weather", "Home");
                        }
                        else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                        {
                            ViewBag.UserMessage = "Incorrect Login or Password";
                        }
                        else if (response.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                        {
                            ViewBag.UserMessage = $"Not found location {location.City}";
                        }
                    }
                }
            }
            return View();
        }

        [Authorize]
        public IActionResult WeatherDelete()
        {
            return View();
        }
    }
}
