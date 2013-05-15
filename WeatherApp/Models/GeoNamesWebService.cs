using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace WeatherApp.Models
{
    public class GeoNamesWebService
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
            catch (Exception)
            {
                throw new Exception("GeoNames webservice can not be reached, please try again later. ");
            }
            
        }

        public List<GeoName> FindGeoNames(string searchString)
        {
            var document = GeoNamesWebService.LoadDocument(String.Format("http://api.geonames.org/search?name={0}&maxRows=100&username=hanserikb&style=full&featureClass=P", searchString));

            return document.Descendants("geoname").Select(geoname => GeoNameFactory.Create(geoname)).ToList();
        }

        public GeoName FindGeoLocation(string lat, string lng)
        {
            var document = GeoNamesWebService.LoadDocument(String.Format("http://api.geonames.org/findNearbyPlaceName?lat={0}&lng={1}&username=hanserikb&style=full", lat, lng));

            return document.Descendants("geoname").Select(geoname => GeoNameFactory.Create(geoname)).First();
        }
    }
} 