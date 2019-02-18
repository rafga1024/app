using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherWeb.ViewModels;

namespace WeatherWeb.Controllers
{
    public class AuthController : Controller
    {
        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = null;
                String StrinAuth = $"{model.Login}:{model.Password}";
                String query = $"{Service.ServerName.GetServerName()}/api/login/1";
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var authBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(StrinAuth));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authBase64);
                    //var response = client.GetAsync("http://localhost:59959/api/login/1");
                    var response = client.GetAsync(query);
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Result.Content)
                        {
                            var result = await content.ReadAsStringAsync();
                            user = (Users)JsonConvert.DeserializeObject<Users>(result);

                            if (user != null)
                            {
                                List<Claim> claims;
                                if (user.isAdmin == true)
                                {
                                    claims = new List<Claim>{
                                                                new Claim(ClaimTypes.Name , model.Login),
                                                                new Claim(ClaimTypes.Role,"admin")
                                                             };
                                }
                                else
                                {
                                    claims = new List<Claim>{
                                                                new Claim(ClaimTypes.Name , model.Login),
                                                                new Claim(ClaimTypes.Role,"user")
                                                             };
                                }
                                var userIdentity = new ClaimsIdentity(claims, "login");
                                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                                await HttpContext.SignInAsync(principal);
                                return RedirectToAction("Weather", "Home");
                            }
                        }
                    }
                }
                ViewBag.UserMessage = "Incorrect Login or Password";
            }
            
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddUserAdmin(RegisterViewModel model)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    String query = $"{Service.ServerName.GetServerName()}/api/login";
                    using (var client = new HttpClient())
                    {
                        string myJonson = "{'login':'" + model.login + "','password':'" + model.password + "','email':'" + model.email + "','isAdmin':1}";
                        //var response = await client.PostAsync("http://localhost:59959/api/login", new StringContent(myJonson, Encoding.UTF8, "application/json"));
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
            }
                
            return View();
        }

        [Authorize]
        public IActionResult AddUserAdmin()
        {
            return View();
        }

    }
}