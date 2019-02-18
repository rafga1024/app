using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace WeatherWeb.ViewModels
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