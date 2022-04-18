using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using palota_func_countries_assessment.Model;
using System.Net.Http;
using palota_func_countries_assessment.Data;
using System.Collections.Generic;
using palota_func_countries_assessment.CustomHttpStatusObjects;

namespace palota_func_countries_assessment.Functions
{
    public static class FunctionEndpoints
    {
        [FunctionName("GetACountry")]
        public static async Task<ActionResult> GetACountry(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "countries/{iso3Code}")] HttpRequest req,
            ILogger log, string iso3Code)
        {
            Country? country = null;
            CountryData countryData = new CountryData(log);
            await Task.Delay(5);
            country = countryData.GetACountry(iso3Code);
            if (country == null)
            {
                string errorMessage = String.Format("The country with ISO 3166 Alpha 3 code {0} could not be found.", iso3Code);
                return new NotFoundObjectResult(new Error { message = errorMessage });
            }
            return new OkObjectResult(country);
        }

        [FunctionName("GetCountries")]
        public static async Task<ActionResult<List<Country>>> GetCountries(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "countries")] HttpRequest req,
            ILogger log)
        {
            List<Country> countries = null;
            CountryData countryData = new CountryData(log);
            await Task.Delay(5);
            countries = countryData.GetAllCountries();
            if (countries.Count == 0)
            {
                string errorMessage = String.Format("Sorry ! an internal server error has occured while processing your request");
                return new InternalServerErrorObjectResult(errorMessage);
            }
            return new OkObjectResult(countries);
        }


        [FunctionName("GetCountryBorders")]
        public static async Task<ActionResult<List<string>>> GetCountryBorders(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "countries/{iso3Code}/borders")] HttpRequest req,
    ILogger log, string iso3Code)
        {
            List<string> countries = null;
            CountryData countryData = new CountryData(log);
            await Task.Delay(5);
            countries = countryData.GetCountryBorders(iso3Code);
            if (countries == null)
            {
                string errorMessage = String.Format("The country with ISO 3166 Alpha 3 code {0} could not be found.", iso3Code);
                return new NotFoundObjectResult(errorMessage);
            }
            return new OkObjectResult(countries);
        }

        [FunctionName("GetCountriesInContinent")]
        public static async Task<ActionResult<List<Country>>> GetCountriesInContinent(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "continents/{continentName}/countries/")] HttpRequest req,
    ILogger log, string continentName)
        {
            List<Country> countries = null;
            CountryData countryData = new CountryData(log);
            await Task.Delay(5);
            countries = countryData.GetCountriesInContinent(continentName);
            if (countries.Count == 0)
            {
                string errorMessage = String.Format("The continent with name {0} could not be found.", continentName);
                return new NotFoundObjectResult(errorMessage);
            }
            return new OkObjectResult(countries);
        }
    }


}
