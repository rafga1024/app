using System;
using System.ComponentModel.DataAnnotations;


namespace WeatherWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(100)]
        public String Login { get; set; }
        [Required]
        [MaxLength(100)]
        public String Password { get; set; }
    }
}
