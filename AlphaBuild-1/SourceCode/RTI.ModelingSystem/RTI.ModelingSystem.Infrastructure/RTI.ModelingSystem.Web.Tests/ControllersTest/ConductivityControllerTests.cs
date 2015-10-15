
namespace RTI.ModelingSystem.Web.Tests
{
	#region Usings

	using System;
	using System.Collections.Generic;
    using System.Linq;
	using System.Web.Mvc;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Interfaces.Services;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Web.Controllers;
    using System.Web;
    using RTI.ModelingSystem.Core.DBModels;

	#endregion Usings

	/// <summary>
	/// Conductivity Controller Tests
	/// </summary>
	[TestClass]
	public class ConductivityControllerTests
	{
		#region Properties

		/// <summary>
		/// mocked Conductivity Repository
		/// </summary>
		Mock<IConductivityRepository> mockedConductivityRepository = new Mock<IConductivityRepository>();

		/// <summary>
		/// mocked Conductivity Repository
		/// </summary>
		Mock<IConductivityService> mockedConductivityService = new Mock<IConductivityService>();

        /// <summary>
        /// mocked Conductivity Repository
        /// </summary>
        Mock<ICustomerRepository> mockedCustomerRepository = new Mock<ICustomerRepository>();

        /// <summary>
        /// mocked Conductivity Repository
        /// </summary>
        Mock<IRepository<customer>> mockedICustomerRepository = new Mock<IRepository<customer>>();

		#endregion Properties

		#region Methods

        /// <summary>
		/// WaterConductivity Test case
		/// </summary>
        [TestMethod]
        public void WaterConductivity()
        {
			long customerId = 1;
            SystemSummaryViewModel systemSummaryViewModel = new SystemSummaryViewModel();
            mockedICustomerRepository.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
            mockedCustomerRepository.Setup(m => m.GetWaterSourceIds(customerId)).Returns(new customer_water() { first_sourceID = 1, second_sourceID = 1 });
            mockedCustomerRepository.Setup(m => m.GetCustomerTrains(customerId)).Returns(new List<train>());
            //mockedVesselRepository.Setup(m => m.GetAll()).Returns((new List<vessel>()).AsQueryable());

            var controller = new ConductivityController(mockedConductivityRepository.Object, mockedConductivityService.Object, mockedICustomerRepository.Object, mockedCustomerRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
            controller.ControllerContext.HttpContext.Session["HasTrainDetails"] = "Verify";

            var returnObj = controller.WaterConductivity();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
            var result = (ViewResult)returnObj;
            Assert.IsNotNull(result);
            mockedICustomerRepository.Verify(m => m.GetAll(), Times.Once());
            mockedCustomerRepository.Verify(m => m.GetWaterSourceIds(customerId), Times.Once());
            mockedCustomerRepository.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
            //mockedVesselRepository.Verify(m => m.GetAll(), Times.Once());
        }

		/// <summary>
		/// Index Test case
		/// </summary>
		[TestMethod]
		public void Index()
		{
            var controller = new ConductivityController(mockedConductivityRepository.Object, mockedConductivityService.Object,mockedICustomerRepository.Object, mockedCustomerRepository.Object);
			var returnObj = controller.Index();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
		}

		/// <summary>
		/// Conductivity Plot Test case
		/// </summary>
		[TestMethod]
		public void ConductivityPlot()
		{
			long usgsSource1ID = 0;
			List<water_data> lstWaterData = new List<water_data>() { new water_data() { dataID = 1, cond = 1, measurment_date = string.Empty } };
			mockedConductivityRepository.Setup(item => item.GetWaterSourceData(usgsSource1ID)).Returns(lstWaterData);
            var controller = new ConductivityController(mockedConductivityRepository.Object, mockedConductivityService.Object, mockedICustomerRepository.Object, mockedCustomerRepository.Object);
			var returnObj = controller.ConductivityPlot(usgsSource1ID);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(JsonResult));
			var result = (JsonResult)returnObj;
			Assert.IsNotNull(result);
			mockedConductivityRepository.Verify(m => m.GetWaterSourceData(usgsSource1ID), Times.Once());
		}

		/// <summary>
		/// Forecast Plot Test case
		/// </summary>
		[TestMethod]
		public void ForecastPlot(long USGSID)
		{
			mockedConductivityService.Setup(item => item.GetForecastdata(7288800)).Returns(new ForecastData());
            var controller = new ConductivityController(mockedConductivityRepository.Object, mockedConductivityService.Object, mockedICustomerRepository.Object, mockedCustomerRepository.Object);
            var returnObj = controller.ForecastPlot(USGSID);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (JsonResult)returnObj;
			Assert.IsNotNull(result);
            mockedConductivityService.Verify(m => m.GetForecastdata(USGSID), Times.Once());
		}

		#endregion Methods
	}
}
