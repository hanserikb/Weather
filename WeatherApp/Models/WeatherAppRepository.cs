using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Data;
using WeatherApp.Models.Interfaces;
namespace WeatherApp.Models
{
    public class WeatherAppRepository : IWeatherAppRepository
    {
        private WeatherEntities _enteties = new WeatherEntities();

        public void Add(GeoName geoName)
        {
            this._enteties.GeoNameSet.AddObject(geoName);
        }

        public void Add(WeatherData weatherData)
        {
            this._enteties.WeatherDataSet.AddObject(weatherData);
        }

        public IQueryable<GeoName> QueryGeoNames()
        {
            return this._enteties.GeoNameSet;
        }

        public IQueryable<WeatherData> QueryWeatherData()
        {
            return this._enteties.WeatherDataSet;
        }

        public void Save()
        {
            this._enteties.SaveChanges();
        }

        public string[] FindLocations(string locationName)
        {
            throw new NotImplementedException();
        }


        public void Update(GeoName geoName)
        {
            if (!this.IsAttatched(geoName))
            {
                this._enteties.GeoNameSet.Attach(geoName);
            }

            this._enteties.ObjectStateManager.ChangeObjectState(geoName, EntityState.Modified);
        }

        public void Delete(WeatherData weatherData)
        {
            if (!this.IsAttatched(weatherData))
            {
                this._enteties.WeatherDataSet.Attach(weatherData);
            }

            this._enteties.WeatherDataSet.DeleteObject(weatherData);
        }

        /// <summary>
        /// Indicates whether the specified object is attached to the context or not.
        /// </summary>
        /// <param name="entity">The entity object to test.</param>
        /// <returns>A Boolean value that is true if the given object is attached; otherwise, false.</returns>
        private bool IsAttatched(object entity)
        {
            ObjectStateEntry entry;
            return this._enteties.ObjectStateManager.TryGetObjectStateEntry(entity, out entry) &&
                entry.State != EntityState.Detached;
        }


    }
}