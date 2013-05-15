using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherApp.Models.Interfaces
{
    public interface IWeatherAppService
    {
        string[] FindDistinctLocations(string locationName);
        System.Collections.Generic.List<GeoName> FindLocations(string locationName);
        GeoName GetGeoName(string countryName, string countyName, string locationName);
        GeoName GetGeoNameByGeoLocation(string lat, string lng);
        System.Collections.Generic.List<WeatherData> GetWeatherDataByParams(string country, string countyName, string name);

    }
}
