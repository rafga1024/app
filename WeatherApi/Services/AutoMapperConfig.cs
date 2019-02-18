using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace WeatherApi.Services
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize((config)=> 
            {
                config.CreateMap<WeatherApi.Models.Clouds, WeatherDataAccess.Clouds>().ReverseMap();
                config.CreateMap<WeatherApi.Models.Coord, WeatherDataAccess.Coord>().ReverseMap();
                config.CreateMap<WeatherApi.Models.Main, WeatherDataAccess.Main>().ReverseMap();
                config.CreateMap<WeatherApi.Models.Sys, WeatherDataAccess.Sys>().ReverseMap();
                config.CreateMap<WeatherApi.Models.Weather, WeatherDataAccess.Weather>().ReverseMap();
                config.CreateMap<WeatherApi.Models.Wind, WeatherDataAccess.Wind>().ReverseMap();
                config.CreateMap<WeatherApi.Models.OpenWeather, WeatherDataAccess.OpenWeather>().ReverseMap();
            });
        }
    }
}