using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Globalization;

namespace WeatherApp.Models
{
    public class WeatherDataFactory
    {
         public static WeatherData Create(XElement element, int locationId)
        {
            return new WeatherData
            {
                locationId = locationId,
                lastUpdate = DateTime.ParseExact(element.Parent.Parent.Parent.Element("meta").Element("lastupdate").Value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
                nextUpdate = DateTime.ParseExact(element.Parent.Parent.Parent.Element("meta").Element("nextupdate").Value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
                timeFrom = DateTime.ParseExact(element.Attribute("from").Value,"yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
                timeTo = DateTime.ParseExact(element.Attribute("to").Value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
                period = int.Parse(element.Attribute("period").Value),
                symbolNumber = int.Parse(element.Element("symbol").Attribute("number").Value),
                windDirection = element.Element("windDirection").Attribute("code").Value,
                windDegrees = Math.Round((float)element.Element("windDirection").Attribute("deg"), 2),
                windSpeed =  Math.Round((float)element.Element("windSpeed").Attribute("mps")),
                temperature = (float)element.Element("temperature").Attribute("value"),
                pressure =  Math.Round((float)element.Element("pressure").Attribute("value")),
                precipitation =  Math.Round((float)element.Element("precipitation").Attribute("value"))
            };
        }
    }
}