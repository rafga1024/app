using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WeatherWeb.ViewModels
{
    [DataContract]
    public class Main
    {
        [DataMember]
        public double temp { get; set; }
        [DataMember]
        public int pressure { get; set; }
        [DataMember]
        public int humidity { get; set; }
        [DataMember]
        public int temp_min { get; set; }
        [DataMember]
        public int temp_max { get; set; }

        public virtual ICollection<OpenWeather> OpenWeather { get; set; }
    }
}
