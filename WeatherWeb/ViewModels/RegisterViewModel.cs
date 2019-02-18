using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public bool isAdmin { get; set; }
    }
}
