using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherApp;
using WeatherApp.Controllers;
using WeatherApp.ViewModels;
using Moq;
using WeatherApp.Models;
namespace WeatherApp.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void FindDistinctLocations()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            JsonResult result = controller.FindDistinctLocations("Kalmar") as JsonResult;
            string[] resultString = (string[])result.Data;

            // Assert
            Assert.IsTrue(resultString.Contains("Kalmar"));
        }

        [TestMethod]
        public void Forecast()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Faks ajax-request
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(r => r.HttpContext.Request["X-Requested-With"]).Returns("XMLHttpRequest");
            controller.ControllerContext = controllerContext.Object;         

            // Act
            HomeIndexViewModel viewModel = new HomeIndexViewModel();
            viewModel.Country = "Sweden";
            viewModel.CountyName = "Kalmar";
            viewModel.LocationName = "Kalmar";

            PartialViewResult result = controller.ForeCast(viewModel) as PartialViewResult;
            
            // Assert
            Assert.IsTrue(result.ViewName == "_ForeCast", "Returned partial view is not correct (_ForeCast)");
            Assert.IsNotNull(result, "ViewResult is null");
        }

        [TestMethod]
        public void IndexPost()
        {
            // Arrange
            HomeController controller = new HomeController();
            HomeIndexViewModel viewModel = new HomeIndexViewModel();
            viewModel.LocationName = "Kalmar";

            // Fake ajax request
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(r => r.HttpContext.Request["X-Requested-With"]).Returns("XMLHttpRequest");
            controller.ControllerContext = controllerContext.Object;


            // Act
            PartialViewResult result = controller.Index(viewModel) as PartialViewResult;
            HomeIndexViewModel newViewModel = result.Model as HomeIndexViewModel;
            Assert.IsTrue(result.ViewName == "_LocationResult", "Wrong view returned");
            Assert.IsTrue(newViewModel.GeoNames.Count > 0, "No matches was found");
        }

    }
}
