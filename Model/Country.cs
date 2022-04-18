using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace palota_func_countries_assessment.Model
{
    public class Country
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("alpha3Code")]
        public string? Iso3Code { get; set; }

        [JsonProperty("capital")]
        public string? Capital { get; set; }
        [JsonProperty("subregion")]
        public string? SubRegion { get; set; }
        [JsonProperty("region")]
        public string? Region { get; set; }
        [JsonProperty("population")]
        public uint? Population { get; set; }
        [JsonProperty("latlng")]
        public CoOrdinate? Location { get; set; }
        [JsonProperty("demonym")]
        public string? Demonym { get; set; }
        [JsonProperty("nativeName")]
        public string? NativeName { get; set; }
        [JsonProperty("numericCode")]
        public string? NumericCode { get; set; }
        [JsonProperty("flag")]
        public string? Flag { get; set; }
    }
}
