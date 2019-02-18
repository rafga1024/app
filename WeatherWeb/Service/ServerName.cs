using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherWeb.Service
{
    public class ServerName
    {
        private const String ServerUrl = "http://myweather.somee.com"; 
        public static String  GetServerName()
        {
            return ServerUrl;
        }
    }
}
