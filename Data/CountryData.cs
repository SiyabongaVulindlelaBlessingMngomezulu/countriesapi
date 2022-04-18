using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using palota_func_countries_assessment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace palota_func_countries_assessment.Data
{
    public class CountryData : ICountryData
    {
        private readonly ILogger log;
        public readonly string COUNTRIES_API_URL = Environment.GetEnvironmentVariable("COUNTRIES_API_URL");
        
        public CountryData(ILogger _logger)
        {
            log = _logger;
        }
        public List<string> GetCountryBorders(string countryCode)
        {
            List<string> borders =  GetBorders(countryCode).Result;
            return borders;
        }

        public async Task<List<string>> GetBorders(string countryCode)
        {
            List<string> Borders = new List<string>();
            Country? country = null;
            using (HttpClient client = new HttpClient())
            {
                string URL = COUNTRIES_API_URL + "/v2/alpha?codes=" + countryCode;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL);
                    response.EnsureSuccessStatusCode();
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic countryData = JsonConvert.DeserializeObject(responseBody);
                    JArray? JBorders = null;
                    if (countryData[0].ContainsKey("borders"))
                    {
                        JBorders = JArray.Parse(countryData[0].borders.ToString());
                        foreach (var border in JBorders.Children())
                        {
                            Borders.Add(border.ToString());
                        }
                    }
                    return Borders;
                }
                catch (Exception e)
                {
                    log.LogError(e.Message.ToString());
                    return Borders;
                }

            }
        }

        public List<Country> GetAllCountries()
        {
            List<Country> countries = GetCountries().Result;
            return countries;
        }

        public async Task<List<Country>> GetCountries()
        {
            List<Country> Countries = new List<Country>();
            JArray? countriesData = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);
                    dynamic response = await client.GetAsync(COUNTRIES_API_URL + "/v3.1/all");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    countriesData = JArray.Parse(responseBody);

                    char[] delimiter = new char[] { '[', ']' };
                    char[] delimiter1 = new char[] { '[', ']', ',' };
                    string? nativeName = "";
                    string? subregion = "";
                    string? region = "";
                    string? population = "";
                    string? demonym = "";
                    string? numericCode = "";
                    string? flag = "";
                    string? iso3Code = "";
                    string? capital = "";
                    string? name = "";
                    foreach (var country in countriesData.Children())
                    {
                        var countryProperties = country.Children();
                        dynamic extendedCountryData = JsonConvert.DeserializeObject(country.ToString());
                        name = extendedCountryData.name.common.ToString();

                        if (extendedCountryData.ContainsKey("capital"))
                        {
                            string capitalString = extendedCountryData.capital.ToString();
                            string[] capitalArray = capitalString.Split(delimiter);
                            capital = capitalArray[1];
                            capital = capital.Trim();
                            capital = capital.Replace("\"", "");
                        }
                        else
                        {
                            capital = string.Empty;
                        }

                        subregion = extendedCountryData.subregion;
                        region = extendedCountryData.region;
                        population = extendedCountryData.population;
                        if (extendedCountryData.ContainsKey("demonyms"))
                        {
                            demonym = extendedCountryData.demonyms.eng.f.ToString();
                        }
                        else
                        {
                            demonym = string.Empty;
                        }
                        numericCode = extendedCountryData.ccn3;
                        flag = extendedCountryData.flags.svg;


                        var JCountry = JObject.Parse(extendedCountryData.ToString());

                        if (!JCountry["name"].ContainsKey("nativeName"))
                        {
                            nativeName = string.Empty;
                        }
                        else
                        {
                            string? native_Name = (JCountry["name"]["nativeName"].ToString());
                            var native_NameStringArray = native_Name.Split(':');
                            string aNativeName = native_NameStringArray[3];
                            aNativeName = aNativeName.Replace("}", "");
                            aNativeName = aNativeName.Trim();

                            string[] nativeNames = aNativeName.Split(',');
                            nativeName = nativeNames[0].Trim();
                            nativeName = nativeName.Replace("\"", "");
                        }

                        string latlngString = extendedCountryData.latlng.ToString();
                        string[] latlngArray = latlngString.Split(delimiter1);


                        latlngArray[1] = latlngArray[1].Trim();
                        latlngArray[2] = latlngArray[2].Trim();

                        float latitude = float.Parse(latlngArray[1], CultureInfo.InvariantCulture);
                        float longitude = float.Parse(latlngArray[2], CultureInfo.InvariantCulture);


                        capital = capital.Trim();
                        iso3Code = extendedCountryData.cca3.ToString();

                        Countries.Add(
                           new Country
                           {
                               Name = name,
                               Iso3Code = iso3Code,
                               Capital = capital,
                               SubRegion = subregion,
                               Region = region,
                               Population = UInt32.Parse(population),
                               Location = new CoOrdinate { Latitude = latitude, Longitude = longitude },
                               Demonym = demonym,
                               NativeName = nativeName,
                               NumericCode = numericCode,
                               Flag = flag
                           }
                       );

                    }
                }
                return Countries;
            }
            catch (Exception e)
            {
                log.LogInformation("Exception" + e);
                return Countries;
            }
        }

        public List<Country> GetCountriesInContinent(string region)
        {
            List<Country> countriesInContinent = GetCountriesNamesOfContinent(region).Result;
            return countriesInContinent;
        }

        public async Task<List<Country>> GetCountriesNamesOfContinent(string continent)
        {
            List<Country> Countries = new List<Country>();
            using (HttpClient client = new HttpClient())
            {
                string URL = COUNTRIES_API_URL + "/v3.1/region/" + continent;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Countries;
                    }
                    response.EnsureSuccessStatusCode();
                    char[] delimiter = new char[] { '[', ']' };
                    char[] delimiter1 = new char[] { '[', ']', ',' };
                    string? nativeName = "";
                    string? subregion = "";
                    string? region = "";
                    string? population = "";
                    string? demonym = "";
                    string? numericCode = "";
                    string? flag = "";
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic countriesData = JArray.Parse(responseBody);
                    foreach (var country in countriesData.Children())
                    {
                        var countryProperties = country.Children();
                        dynamic extendedCountryData = JsonConvert.DeserializeObject(country.ToString());
                        string name = extendedCountryData.name.common.ToString();
                        string iso3Code = "";
                        string capital = "";
                        if (extendedCountryData.ContainsKey("capital"))
                        {
                            string capitalString = extendedCountryData.capital.ToString();
                            string[] capitalArray = capitalString.Split(delimiter);
                            capital = capitalArray[1];
                            capital = capital.Trim();
                            capital = capital.Replace("\"", "");
                        }
                        else
                        {
                            capital = string.Empty;
                        }


                        subregion = extendedCountryData.subregion;
                        region = extendedCountryData.region;
                        population = extendedCountryData.population;
                        if (extendedCountryData.ContainsKey("demonyms"))
                        {
                            demonym = extendedCountryData.demonyms.eng.f.ToString();
                        }
                        else
                        {
                            demonym = string.Empty;
                        }
                        numericCode = extendedCountryData.ccn3;
                        flag = extendedCountryData.flags.svg;

                        var JCountry = JObject.Parse(extendedCountryData.ToString());

                        if (!JCountry["name"].ContainsKey("nativeName"))
                        {
                            nativeName = string.Empty;
                        }
                        else
                        {
                            string? native_Name = (JCountry["name"]["nativeName"].ToString());
                            var native_NameStringArray = native_Name.Split(':');
                            string aNativeName = native_NameStringArray[3];
                            aNativeName = aNativeName.Replace("}", "");
                            aNativeName = aNativeName.Trim();

                            string[] nativeNames = aNativeName.Split(',');
                            nativeName = nativeNames[0].Trim();
                            nativeName = nativeName.Replace("\"", "");
                        }

                        string latlngString = extendedCountryData.latlng.ToString();
                        string[] latlngArray = latlngString.Split(delimiter1);


                        latlngArray[1] = latlngArray[1].Trim();
                        latlngArray[2] = latlngArray[2].Trim();

                        float latitude = float.Parse(latlngArray[1], CultureInfo.InvariantCulture);
                        float longitude = float.Parse(latlngArray[2], CultureInfo.InvariantCulture);

                        capital = capital.Trim();
                        iso3Code = extendedCountryData.cca3.ToString();

                        Countries.Add(
                           new Country
                           {
                               Name = name,
                               Iso3Code = iso3Code,
                               Capital = capital,
                               SubRegion = subregion,
                               Region = region,
                               Population = UInt32.Parse(population),
                               Location = new CoOrdinate { Latitude = latitude, Longitude = longitude },
                               Demonym = demonym,
                               NativeName = nativeName,
                               NumericCode = numericCode,
                               Flag = flag
                           }
                       );
                    }
                    return Countries;
                }
                catch (Exception e)
                {
                    log.LogError(e.Message.ToString());
                    return Countries;
                }
            }
        }

        public Country GetACountry(string countryCode)
        {
            Country? country = null;
            country = GetCountry(countryCode).Result;
            return country;
        }

        public async Task<Country> GetCountry(string countryCode)
        {
            Country? country = null;
            using (HttpClient client = new HttpClient())
            {
                String URL = COUNTRIES_API_URL + "/v2/alpha?codes=" + countryCode;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return country;
                    }
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic countryData = JsonConvert.DeserializeObject(responseBody);
                    country = new Country
                    {
                        Name = countryData[0].name,
                        Iso3Code = countryData[0].alpha3Code,
                        Capital = countryData[0].capital,
                        SubRegion = countryData[0].subregion,
                        Region = countryData[0].region,
                        Population = countryData[0].population,
                        Location = new CoOrdinate { Latitude = countryData[0].latlng[0], Longitude = countryData[0].latlng[1] },
                        Demonym = countryData[0].demonym,
                        NativeName = countryData[0].nativeName,
                        NumericCode = countryData[0].numericCode,
                        Flag = countryData[0].flag
                    };

                    return country;
                }
                catch (Exception e)
                {
                    log.LogError(e.Message.ToString());
                    return country;
                }

            }

        }
    }
}
