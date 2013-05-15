using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherApp.Models
{
    public interface IWeatherAppRepository
    {
        
        void Add(GeoName geoName);
        void Add(WeatherData weatherData);
        void Update(GeoName geoName);
        void Delete(WeatherData weatherData);
        IQueryable<GeoName> QueryGeoNames();
        IQueryable<WeatherData> QueryWeatherData();
        void Save();
    }
}
