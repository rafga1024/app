using System;
using System.ComponentModel.DataAnnotations;


namespace WeatherWeb.ViewModels
{
    public class Location
    {
        [Required]
        [MaxLength(50)]
        public String City { get; set; }
        [Required]
        [MaxLength(2)]
        [MinLength(2)]
        public String Country { get; set; }
        [Required]
        public String Login { get; set; }
        [Required]
        public String Password { get; set; }
    }
}
