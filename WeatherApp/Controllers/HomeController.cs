using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeatherApp.Models;
using WeatherApp.ViewModels;
using WeatherApp.Models.Interfaces;
namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private IWeatherAppService _service;

        public HomeController()
            : this(new WeatherAppService())
        { 
            
        }

        public HomeController(IWeatherAppService service)
        {
            this._service = service;
        }


        /// <summary>
        ///  Gets a list of forecasts and returns it to a view
        /// </summary>
        [HttpGet]
        public ActionResult ForeCast(HomeIndexViewModel model)
        { 
            try
            {
                if (ModelState.IsValid)
                {
                    WeatherAppService service = new WeatherAppService();
                    model.WeatherDatas = this._service.GetWeatherDataByParams(model.Country, model.CountyName, model.LocationName);
                    model.GeoName = this._service.GetGeoName(model.Country, model.CountyName, model.LocationName);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_ForeCast", model); 
                    }   
                }
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    this.Response.StatusCode = 400;
                    return Content(ex.Message);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, ex.Message);
                }
            }
            return View("Index", model);
        }

        /// <summary>
        ///  Default home controller method
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            return View();

        }


        /// <summary>
        ///  Function that triggers when the user search for a location
        /// </summary>
        [HttpPost]
        public ActionResult Index([Bind(Include="LocationName")]HomeIndexViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.GeoNames = this._service.FindLocations(model.LocationName);
                    if (model.GeoNames.Count == 1)
                    {
                        GeoName geoName = model.GeoNames.First();
                        model.Country = geoName.countryName;
                        model.CountyName = geoName.countyName;
                        model.LocationName = geoName.name;
                        return ForeCast(model);
                    }
                    else if (model.GeoNames.Count == 0)
                    {
                        throw new Exception(String.Format("No locations named '{0}' were found", model.LocationName));
                    }

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_LocationResult", model);
                    }
                    
                       
                }            
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    this.Response.StatusCode = 404;
                    model.ModalMessage = ex.Message;
                    return PartialView("_ErrorModal", model);
                }
                else
                {
                    model.ModalMessage = ex.Message;
                    return PartialView("_ErrorModal", model);
                }
            }
            return View("Index", model);
        }

        /// <summary>
        ///  Default about controller method
        /// </summary>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        ///  Makes a search by the provided location name.
        /// </summary>
        [HttpPost]      
        public JsonResult FindDistinctLocations(string locationName)
        {
            var list =  Json(this._service.FindDistinctLocations(locationName),JsonRequestBehavior.AllowGet);
            return list;
        }

        [HttpPost]
        public ActionResult GeoLocation([Bind(Include = "Lat, Lng")]HomeIndexViewModel model)
        {   
            try
            {     
                model.GeoName = this._service.GetGeoNameByGeoLocation(model.Lat, model.Lng);
                model.WeatherDatas = this._service.GetWeatherDataByParams(model.GeoName.countryName, model.GeoName.countyName, model.GeoName.name);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ForeCast", model);
                }   
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    this.Response.StatusCode = 400;
                    model.ModalMessage = ex.Message;
                    return PartialView("_ErrorModal", model);
                }
                else
                {
                    model.ModalMessage = ex.Message;
                    return PartialView("_ErrorModal", model);
                }
            }
            return View("Index", model);

        }
    }
}
