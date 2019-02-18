using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WeatherWeb.ViewModels
{
    [DataContract]
    public class OpenWeather
    {
        [DataMember]
        public Coord coord { get; set; }
        [DataMember]
        public List<Weather> weather { get; set; }
        [DataMember]
        public string @base { get; set; }
        [DataMember]
        public Main main { get; set; }
        [DataMember]
        public int visibility { get; set; }
        [DataMember]
        public Wind wind { get; set; }
        [DataMember]
        public Clouds clouds { get; set; }
        [DataMember]
        public int dt { get; set; }
        [DataMember]
        public Sys sys { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int cod { get; set; }

        public virtual Coord Coord { get; set; }
        public virtual Weather Weather { get; set; }
        public virtual Main Main { get; set; }
        public virtual Wind Wind { get; set; }
        public virtual Clouds Clouds { get; set; }
        public virtual Sys Sys { get; set; }
    }
}

