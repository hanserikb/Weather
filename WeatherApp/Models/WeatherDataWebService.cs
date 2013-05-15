using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace WeatherApp.Models
{
    public class WeatherDataWebService
    {
        private static XDocument LoadDocument(string requestURI)
        {
            XDocument document;
            var request = WebRequest.Create(requestURI);
            try
            {
                using (var response = request.GetResponse())

                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    return document = XDocument.Load(stream);
                }
            }
            catch
            {
                throw new Exception("YR.no webservice can not be reached, please try again later. ");
            }
            
        }
        
        //http://www.yr.no/place/<countryName>/<name>/<toponymName>/forecast.xml
        public List<WeatherData> FindWeatherData(GeoName geoName)
        {
            var document = WeatherDataWebService.LoadDocument(String.Format("http://www.yr.no/place/{0}/{1}/{2}/forecast.xml", geoName.countryName, geoName.countyName, geoName.name));
            
            return document.Descendants("time").Select(weatherdata => WeatherDataFactory.Create(weatherdata, geoName.locationId)).ToList();
        }
    }
}