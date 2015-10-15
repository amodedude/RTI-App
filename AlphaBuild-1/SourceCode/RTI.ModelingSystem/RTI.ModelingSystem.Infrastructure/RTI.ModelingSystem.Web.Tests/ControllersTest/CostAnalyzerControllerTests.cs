

namespace RTI.ModelingSystem.Web.Tests.ControllersTest
{
    #region Usings

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;
    using RTI.ModelingSystem.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    #endregion Usings

    /// <summary>
    /// CostAnalyzerControllerTests class
    /// </summary>
    [TestClass]
    public class CostAnalyzerControllerTests
    {
        #region Properties

        /// <summary>
        /// mocked Customer Repository
        /// </summary>
        Mock<IRepository<customer>> mockedCustomerRepository = new Mock<IRepository<customer>>();

        /// <summary>
        /// mocked Repository Customer
        /// </summary>
        Mock<ICustomerRepository> mockedCustRepository = new Mock<ICustomerRepository>();

        /// <summary>
        /// mocked Vessel Customer
        /// </summary>
        Mock<IRepository<vessel>> mockedVesselRepository = new Mock<IRepository<vessel>>();

        /// <summary>
        /// mocked CostAnalyzer Service
        /// </summary>
        Mock<ICostAnalyzerService> mockedCostAnalyzerService = new Mock<ICostAnalyzerService>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// CostAnalyzer Test case
        /// </summary>
        [TestMethod]
        public void CostAnalyzer()
        {
			long customerId = 1;
            SystemSummaryViewModel systemSummaryViewModel = new SystemSummaryViewModel();
            mockedCustomerRepository.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
            mockedCustRepository.Setup(m => m.GetWaterSourceIds(customerId)).Returns(new customer_water() { first_sourceID = 1, second_sourceID = 1 });
            mockedCustRepository.Setup(m => m.GetCustomerTrains(customerId)).Returns(new List<train>());
            mockedVesselRepository.Setup(m => m.GetAll()).Returns((new List<vessel>()).AsQueryable());

            var controller = new CostAnalyzerController(mockedCustomerRepository.Object, mockedCustRepository.Object, mockedVesselRepository.Object, mockedCostAnalyzerService.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
            controller.ControllerContext.HttpContext.Session["HasTrainDetails"] = "Verify";

            var returnObj = controller.CostAnalyzer();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
            var result = (ViewResult)returnObj;
            Assert.IsNotNull(result);
            mockedCustomerRepository.Verify(m => m.GetAll(), Times.Once());
            mockedCustRepository.Verify(m => m.GetWaterSourceIds(customerId), Times.Once());
            mockedCustRepository.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
            mockedVesselRepository.Verify(m => m.GetAll(), Times.Once());
        }

        /// <summary>
        /// GetCostSettings Test case
        /// </summary>
        [TestMethod]
        public void GetCostSettings()
        {
            string customerId = "1";
            string selectedTrain = "0";
            mockedCostAnalyzerService.Setup(m => m.GetCostSettings(customerId, selectedTrain)).Returns(new CostSettings());

            var controller = new CostAnalyzerController(mockedCustomerRepository.Object, mockedCustRepository.Object, mockedVesselRepository.Object, mockedCostAnalyzerService.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
            controller.ControllerContext.HttpContext.Session["SelectedTrain"] = selectedTrain;

            var returnObj = controller.GetCostSettings();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "CostSettings");

            mockedCostAnalyzerService.Verify(m => m.GetCostSettings(customerId, selectedTrain), Times.Once());
        }

        /// <summary>
        /// PlotCostAnalyzerChart Test case
        /// </summary>
        [TestMethod]
        public void PlotCostAnalyzerChart()
        {
            double acidPrice = 0.0;
            double causticPrice = 0.0;
            int acidUsage = 0;
            int causticUsage = 0;
            int cationResin = 0;
            int anionResin = 0;
            int loadOnSettingsUpdate = 1;
            PriceData DataToSend = null;
            string customerId = "1";

            var controller = new CostAnalyzerController(mockedCustomerRepository.Object, mockedCustRepository.Object, mockedVesselRepository.Object, mockedCostAnalyzerService.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
            controller.ControllerContext.HttpContext.Session["Data_ToSend"] = DataToSend;

            var returnObj = controller.PlotCostAnalyzerChart((double?)acidPrice, (double?)causticPrice, (int?)acidUsage, (int?)causticUsage, (int?)cationResin, (int?)anionResin, (int?)loadOnSettingsUpdate);
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(JsonResult));
            var result = (JsonResult)returnObj;
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// GetCumulativeSavings Test case
        /// </summary>
        [TestMethod]
        public void GetCumulativeSavings()
        {
            var controller = new CostAnalyzerController(mockedCustomerRepository.Object, mockedCustRepository.Object, mockedVesselRepository.Object, mockedCostAnalyzerService.Object);

            var returnObj = controller.GetCumulativeSavings();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "_CumulativeSavings");
        }

        /// <summary>
        /// GetCumulativeSavings Test case
        /// </summary>
        [TestMethod]
        public void GetResultsTable(string weekNumber, bool isClick)
        {
            //string weekNumber = "1";
			double?[] weekData = new double?[6];
			weekData[0] = 1;
			weekData[1] = 2;
			weekData[2] = 2;
			weekData[3] = 2;
			weekData[4] = 2;
			weekData[5] = 2;
			mockedCostAnalyzerService.Setup(m => m.SelectedWeekDataFinder(1)).Returns(weekData);
			mockedCostAnalyzerService.Setup(m => m.GetCostAnalyzerResultsData()).Returns(new CostAnalyzerResult());
            var controller = new CostAnalyzerController(mockedCustomerRepository.Object, mockedCustRepository.Object, mockedVesselRepository.Object, mockedCostAnalyzerService.Object);

            var returnObj = controller.GetResultsTable(weekNumber, isClick);
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "_CostAnalyzerResultsTable");
        }

        #endregion Methods
    }
}
