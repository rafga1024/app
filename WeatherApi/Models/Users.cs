using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherApi.Models
{
    public class Users
    {
        public int id { get; set; }
        public String login { get; set; }
        public String password { get; set; }
        public bool isAdmin { get; set; }
        public String email { get; set; }
    }
}