using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using WeatherApi.Services;
using WeatherDataAccess;

namespace WeatherApi.Controllers
{
    [RequireHttps]
    public class WeatherController : ApiController
    {
        public HttpResponseMessage Get()
        {
            try
            {
                using (DBweatherEntities _ctx = new DBweatherEntities())
                {

                    var list = _ctx.OpenWeathers.ToList();
                    var listOpenWeather = new List<Models.OpenWeather>();
                    foreach (var tmp in list)
                    {
                        var modeltmp = new Models.OpenWeather()
                        {
                            @base = tmp.@base,
                            name = tmp.name,
                            visibility = tmp.visibility,
                            dt = tmp.dt,
                            id = tmp.id,
                            cod = tmp.cod

                        };

                        tmp.Clouds = _ctx.Clouds1.FirstOrDefault(x => x.id == tmp.idClouds);
                        modeltmp.clouds = Mapper.Map<Models.Clouds>(tmp.Clouds);

                        tmp.Coord = _ctx.Coords.FirstOrDefault(x => x.id == tmp.idCoord);
                        modeltmp.coord = Mapper.Map<Models.Coord>(tmp.Coord);

                        tmp.Main = _ctx.Mains.FirstOrDefault(x => x.id == tmp.idMain);
                        modeltmp.main = Mapper.Map<Models.Main>(tmp.Main);

                        tmp.Sys = _ctx.Sys.FirstOrDefault(x => x.idSys == tmp.idSys);
                        modeltmp.sys = Mapper.Map<Models.Sys>(tmp.Sys);

                        tmp.Wind = _ctx.Winds.FirstOrDefault(x => x.id == tmp.idWind);
                        modeltmp.wind = Mapper.Map<Models.Wind>(tmp.Wind);
                        var listaWeather = new List<Models.Weather>();
                        foreach (var tmpList in _ctx.Weathers.Where(x => x.idOpenWeather == tmp.idOpen).ToList())
                        {
                            listaWeather.Add(Mapper.Map<Models.Weather>(tmpList));
                        }
                        modeltmp.weather = listaWeather;

                        listOpenWeather.Add(modeltmp);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, listOpenWeather);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [BasicAuthentication]
        public HttpResponseMessage Post([FromBody] OpenWeather w)
        {
            if (Thread.CurrentPrincipal.IsInRole("admin"))
            {
                using (DBweatherEntities _ctx = new DBweatherEntities())
                {
                    using (DbContextTransaction tran = _ctx.Database.BeginTransaction())
                    {
                        try
                        {

                            if (!_ctx.OpenWeathers.Where(x => x.name.ToUpper() == w.name.ToUpper() & x.Sys.country == w.Sys.country).Any())
                            {
                                var TmpOpenWeather = new OpenWeather() { @base = w.@base, visibility = w.visibility, dt = w.dt, id = w.id, name = w.name, cod = w.cod };
                                var tmpCloud = new Clouds() { all = w.Clouds.all };
                                var tmpCoord = new Coord() { lat = w.Coord.lat, lon = w.Coord.lon };
                                var tmpMain = new Main()
                                {
                                    humidity = w.Main.humidity,
                                    pressure = w.Main.pressure,
                                    temp = w.Main.temp,
                                    temp_max = w.Main.temp_max,
                                    temp_min = w.Main.temp_min,
                                };
                                var tmpSy = new Sys()
                                {
                                    country = w.Sys.country,
                                    message = w.Sys.message,
                                    sunrise = w.Sys.sunrise,
                                    sunset = w.Sys.sunset,
                                    type = w.Sys.type,
                                    id = w.Sys.id

                                };
                                var tmpWind = new Wind() { deg = w.Wind.deg, speed = w.Wind.speed };

                                _ctx.Clouds1.Add(tmpCloud);
                                _ctx.SaveChanges();
                                TmpOpenWeather.idClouds = tmpCloud.id;
                                _ctx.Coords.Add(tmpCoord);
                                _ctx.SaveChanges();
                                TmpOpenWeather.idCoord = tmpCoord.id;
                                _ctx.Mains.Add(tmpMain);
                                _ctx.SaveChanges();
                                TmpOpenWeather.idMain = tmpMain.id;
                                _ctx.Sys.Add(tmpSy);
                                _ctx.SaveChanges();
                                TmpOpenWeather.idSys = tmpSy.idSys;
                                _ctx.Winds.Add(tmpWind);
                                _ctx.SaveChanges();
                                TmpOpenWeather.idWind = tmpWind.id;
                                _ctx.OpenWeathers.Add(TmpOpenWeather);
                                _ctx.SaveChanges();
                                var tmpId = TmpOpenWeather.idOpen;

                                foreach (var t in w.Weather)
                                {
                                    var tmpWeather = new Weather()
                                    {
                                        idOpenWeather = tmpId,
                                        description = t.description,
                                        icon = t.icon,
                                        main = t.main,
                                        id = t.id
                                    };
                                    _ctx.Weathers.Add(tmpWeather);
                                    _ctx.SaveChanges();
                                }
                                tran.Commit();
                                return Request.CreateResponse(HttpStatusCode.Created);
                            }
                            else
                                return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                        }
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [BasicAuthentication]
        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put([FromBody] OpenWeather w)
        {
            if (Thread.CurrentPrincipal.IsInRole("admin"))
            {
                try
                {
                    using (DBweatherEntities _ctx = new DBweatherEntities())
                    {
                        using (DbContextTransaction tran = _ctx.Database.BeginTransaction())
                        {
                            try
                            {
                                
                                var OpenWeatherFromDB = _ctx.OpenWeathers.FirstOrDefault(z => z.name == w.name);
                                if (OpenWeatherFromDB != null)
                                {

                                    var CloudFromDB = _ctx.Clouds1.FirstOrDefault(x => x.id == OpenWeatherFromDB.idClouds);
                                    var CoordFromDB = _ctx.Coords.FirstOrDefault(x => x.id == OpenWeatherFromDB.idCoord);
                                    var MainFromDB = _ctx.Mains.FirstOrDefault(x => x.id == OpenWeatherFromDB.idMain);
                                    var SysFromDB = _ctx.Sys.FirstOrDefault(x => x.idSys == OpenWeatherFromDB.idSys);
                                    var WindFromDB = _ctx.Winds.FirstOrDefault(x => x.id == OpenWeatherFromDB.idWind);

                                    CloudFromDB.all = w.Clouds.all;
                                    _ctx.SaveChanges();

                                    CoordFromDB.lat = w.Coord.lat;
                                    CoordFromDB.lon = w.Coord.lon;
                                    _ctx.SaveChanges();

                                    MainFromDB.humidity = w.Main.humidity;
                                    MainFromDB.pressure = w.Main.pressure;
                                    MainFromDB.temp = w.Main.temp;
                                    MainFromDB.temp_max = w.Main.temp_max;
                                    MainFromDB.temp_min = w.Main.temp_min;
                                    _ctx.SaveChanges();

                                    SysFromDB.country = w.Sys.country;
                                    SysFromDB.message = w.Sys.message;
                                    SysFromDB.sunrise = w.Sys.sunrise;
                                    SysFromDB.sunset = w.Sys.sunset;
                                    SysFromDB.type = w.Sys.type;
                                    SysFromDB.id = w.Sys.id;
                                    _ctx.SaveChanges();

                                    WindFromDB.deg = w.Wind.deg;
                                    WindFromDB.speed = w.Wind.speed;
                                    _ctx.SaveChanges();

                                    OpenWeatherFromDB.@base = w.@base;
                                    OpenWeatherFromDB.visibility = w.visibility;
                                    OpenWeatherFromDB.dt = w.dt;
                                    OpenWeatherFromDB.id = w.id;
                                    OpenWeatherFromDB.name = w.name;
                                    OpenWeatherFromDB.cod = w.cod;
                                    OpenWeatherFromDB.idClouds = CloudFromDB.id;
                                    OpenWeatherFromDB.idCoord = CoordFromDB.id;
                                    OpenWeatherFromDB.idMain = MainFromDB.id;
                                    OpenWeatherFromDB.idSys = SysFromDB.idSys;
                                    OpenWeatherFromDB.idWind = WindFromDB.id;
                                    _ctx.SaveChanges();

                                    var WeatherFromDB = _ctx.Weathers.Where(x => x.idOpenWeather == OpenWeatherFromDB.idOpen).ToList();
                                    var WeatherFromRequest = w.Weather.ToList();
                                    for (int i = 0; i < WeatherFromDB.Count(); i++)
                                    {
                                        WeatherFromDB[i].idOpenWeather = OpenWeatherFromDB.idOpen;
                                        WeatherFromDB[i].description = WeatherFromRequest[i].description;
                                        WeatherFromDB[i].icon = WeatherFromRequest[i].icon;
                                        WeatherFromDB[i].main = WeatherFromRequest[i].main;
                                        WeatherFromDB[i].id = WeatherFromRequest[i].id;
                                        _ctx.SaveChanges();
                                    }
                                    tran.Commit();
                                    return Request.CreateResponse(HttpStatusCode.Created);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.NotFound);
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [BasicAuthentication]
        [System.Web.Mvc.HttpDelete]
        [System.Web.Mvc.Route("api/Weather")]
        public HttpResponseMessage Delete(String city, String country)
        {
            if (Thread.CurrentPrincipal.IsInRole("admin"))
            {
                try
                {
                    using (DBweatherEntities _ctx = new DBweatherEntities())
                    {
                        using (DbContextTransaction tran = _ctx.Database.BeginTransaction())
                        {
                            try
                            {
                                
                                var OpenWeatherFromDB = _ctx.OpenWeathers.FirstOrDefault(z => z.name == city && z.Sys.country == country);
                                if (OpenWeatherFromDB != null)
                                {

                                    var WeatherFromDB = _ctx.Weathers.Where(x => x.idOpenWeather == OpenWeatherFromDB.idOpen).ToList();
                                    if (WeatherFromDB.Any())
                                    {
                                        foreach (var tmp in WeatherFromDB)
                                        {
                                            _ctx.Weathers.Remove(tmp);
                                            _ctx.SaveChanges();
                                        }
                                    }
                                    var CloudFromDB = _ctx.Clouds1.FirstOrDefault(x => x.id == OpenWeatherFromDB.idClouds);
                                    var CoordFromDB = _ctx.Coords.FirstOrDefault(x => x.id == OpenWeatherFromDB.idCoord);
                                    var MainFromDB = _ctx.Mains.FirstOrDefault(x => x.id == OpenWeatherFromDB.idMain);
                                    var SysFromDB = _ctx.Sys.FirstOrDefault(x => x.idSys == OpenWeatherFromDB.idSys);
                                    var WindFromDB = _ctx.Winds.FirstOrDefault(x => x.id == OpenWeatherFromDB.idWind);
                                    _ctx.OpenWeathers.Remove(OpenWeatherFromDB);
                                    _ctx.SaveChanges();

                                    _ctx.Clouds1.Remove(CloudFromDB);
                                    _ctx.SaveChanges();

                                    _ctx.Coords.Remove(CoordFromDB);
                                    _ctx.SaveChanges();

                                    _ctx.Mains.Remove(MainFromDB);
                                    _ctx.SaveChanges();

                                    _ctx.Sys.Remove(SysFromDB);
                                    _ctx.SaveChanges();

                                    _ctx.Winds.Remove(WindFromDB);
                                    _ctx.SaveChanges();

                                    tran.Commit();
                                    return Request.CreateResponse(HttpStatusCode.Created);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.NotFound);
                                }
                            } 
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

    }
}
