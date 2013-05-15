using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherApp.Models.Interfaces;

namespace WeatherApp.Models
{
    public class WeatherAppService : IWeatherAppService
    {
        private IWeatherAppRepository _repository;

        public WeatherAppService()
            : this(new WeatherAppRepository())
        {
            // Blank
        }

        public WeatherAppService(IWeatherAppRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Gets a list of geoname-objects by searching on its name 
        /// in either the database or the webservice
        /// </summary>
        /// <param name="locationName">Input search string</param>
        /// <returns>List of geoName Objects</returns>
        public List<GeoName> FindLocations(string locationName)
        {
            var locations = this._repository.QueryGeoNames().Where(g => g.name.Contains(locationName)).ToList();

            if (locations.Count() == 0)
            {
                var webService = new GeoNamesWebService();
                locations = webService.FindGeoNames(locationName);

                foreach (var location in locations)
                {
                    this._repository.Add(location);
                }
                // ...save the user in the database.
                this._repository.Save();
                return locations;
            }
            return locations;
        }

        
        /// <summary>
        /// Gets a list of forecasts by searching on a locations parameters
        /// </summary>
        /// <param name="country">Country name</param>
        /// <param name="countyName">Country Name</param>
        /// <param name="name">Location Name</param>
        /// <returns></returns>
        public List<WeatherData> GetWeatherDataByParams(string country, string countyName, string name)
        {

            GeoName geoName = this._repository.QueryGeoNames()
                .Where(g => g.countryName == country &&
                       g.countyName == countyName &&
                       g.name == name).SingleOrDefault();

            if (!geoName.WeatherDatas.Any() || DateTime.Now >= geoName.WeatherDatas.First().nextUpdate)
            {
                WeatherDataWebService webservice = new WeatherDataWebService();
                geoName.WeatherDatas.ToList().ForEach(w => this._repository.Delete(w));
                webservice.FindWeatherData(geoName).ForEach(w => geoName.WeatherDatas.Add(w));
                this._repository.Save();
            }

            return geoName.WeatherDatas.OrderBy(w => w.timeFrom).ToList();
            
        }

        /// <summary>
        /// Finds location names for the autocomplete function
        /// </summary>
        /// <param name="locationName">Searchstring</param>
        /// <returns>Stringlist containing names of cities</returns>
        public string[] FindDistinctLocations(string locationName)
        {
            return this._repository.QueryGeoNames().Where(g => g.name.Contains(locationName)).OrderBy(g => g.name).Select(g => g.name).Distinct().ToArray();
        }

      
        /// <summary>
        /// Gets the complete geoname Object by passing country/county/name -parameters
        /// </summary>
        /// <param name="countryName">country name</param>
        /// <param name="countyName">county name</param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public GeoName GetGeoName(string countryName, string countyName, string locationName)
        {
            GeoName geoName = this._repository.QueryGeoNames().Single(g => g.countryName == countryName && g.countyName == countyName && g.name == locationName);

            return geoName;
        }


        /// <summary>
        /// Gets the geoName object via the users location
        /// For use with HTML5 Geolocation
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public GeoName GetGeoNameByGeoLocation(string lat, string lng)
        {
            GeoNamesWebService geoNameWebservice = new GeoNamesWebService();
            GeoName geoName = geoNameWebservice.FindGeoLocation(lat, lng);

            if (this._repository.QueryGeoNames().Where(q => q.geoNameId == geoName.geoNameId).Count() == 0)
	        {
                this._repository.Add(geoName);
                this._repository.Save();
	        }
            return geoName;
        }

    }
}