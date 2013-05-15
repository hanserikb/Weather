using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherApp.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class HomeIndexViewModel
    {
        [DisplayName("Location")]
        [Required]
        [StringLength(100)]
        public string LocationName { get; set; }
        public string CountyName { get; set; }
        public string Country { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public List<WeatherData> WeatherDatas { get; set; }
        public List<GeoName> GeoNames { get; set; }
        public GeoName GeoName { get; set; }

        public string ModalMessage { get; set; }
    }
}