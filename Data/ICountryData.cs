using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using palota_func_countries_assessment.Model;

namespace palota_func_countries_assessment.Data
{
    public interface ICountryData
    {
        public Country GetACountry(string countryCode);
        public List<Country> GetAllCountries();

        public List<string> GetCountryBorders(string countryCode);

        public List<Country> GetCountriesInContinent(string region);

    }
}
