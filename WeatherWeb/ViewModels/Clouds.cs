using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WeatherWeb.ViewModels
{
    [DataContract]
    public class Clouds
    {
        [DataMember]
        public int all { get; set; }

        public virtual ICollection<OpenWeather> OpenWeather { get; set; }
    }
}
