using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WeatherApp.Models
{
    public static class GeoNameFactory
    {
        public static GeoName Create(XElement element)
        {
            return new GeoName
            {
                geoNameId = int.Parse(element.Element("geonameId").Value),
                countryName = element.Element("countryName").Value,
                countyName = element.Element("adminName1").Value,
                name = element.Element("name").Value,
                latitude = (float)element.Element("lat"),
                longitude = (float)element.Element("lng")

            };
        }
    }
}