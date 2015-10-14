

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
    /// PredictiveSystemControllerTests class
    /// </summary>
    [TestClass]
    public class PredictiveSystemControllerTests
    {
        #region Properties

        /// <summary>
        /// mocked Predictive Repository
        /// </summary>
        Mock<IPredictiveModelRepository> mockedPredictiveRepository = new Mock<IPredictiveModelRepository>();

        /// <summary>
        /// mocked PredictiveModel Service
        /// </summary>
        Mock<IPredictiveModelService> mockedPredictiveModelService = new Mock<IPredictiveModelService>();

        /// <summary>
        /// mocked Customer Repository
        /// </summary>
        Mock<IRepository<customer>> mockedCustomerRepository = new Mock<IRepository<customer>>();

        /// <summary>
        /// mocked Repository Customer
        /// </summary>
        Mock<ICustomerRepository> mockedRepositoryCustomer = new Mock<ICustomerRepository>();

        /// <summary>
        /// mocked Vessel Repository
        /// </summary>
        Mock<IRepository<vessel>> mockedVesselRepository = new Mock<IRepository<vessel>>();

        /// <summary>
        /// mocked Train Repository
        /// </summary>
        Mock<IRepository<train>> mockedTrainRepository = new Mock<IRepository<train>>();

        /// <summary>
        /// mocked ResinProduct Repository
        /// </summary>
        Mock<IRepository<resin_products>> mockedResinProdRepository = new Mock<IRepository<resin_products>>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// PredictiveSystemPerformance Test case
        /// </summary>
        [TestMethod]
        public void PredictiveSystemPerformance()
        {
			long customerId = 1;
            SystemSummaryViewModel systemSummaryViewModel = new SystemSummaryViewModel();
            mockedCustomerRepository.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
            mockedRepositoryCustomer.Setup(m => m.GetWaterSourceIds(customerId)).Returns(new customer_water() { first_sourceID = 1, second_sourceID = 1 });
            mockedRepositoryCustomer.Setup(m => m.GetCustomerTrains(customerId)).Returns(new List<train>());
            mockedVesselRepository.Setup(m => m.GetAll()).Returns((new List<vessel>()).AsQueryable());

            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
            controller.ControllerContext.HttpContext.Session["HasTrainDetails"] = "Verify";

            var returnObj = controller.PredictiveSystemPerformance();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
            var result = (ViewResult)returnObj;
            Assert.IsNotNull(result);
            mockedCustomerRepository.Verify(m => m.GetAll(), Times.Once());
            mockedRepositoryCustomer.Verify(m => m.GetWaterSourceIds(customerId), Times.Once());
            mockedRepositoryCustomer.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
            mockedVesselRepository.Verify(m => m.GetAll(), Times.Once());
        }

        /// <summary>
        /// PredictiveSystemPerformance Test case
        /// </summary>
        [TestMethod]
        public void GetPerformanceSettings()
        {
			long customerId = 1;
            mockedTrainRepository.Setup(m => m.GetAll()).Returns(new List<train>().AsQueryable());
            mockedVesselRepository.Setup(m => m.GetAll()).Returns(new List<vessel>().AsQueryable());// (new customer_water() { first_sourceID = 1, second_sourceID = 1 });

            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;

            var returnObj = controller.GetPerformanceSettings();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "PerformanceSettings");
            mockedTrainRepository.Verify(m => m.GetAll(), Times.Once());
            mockedVesselRepository.Verify(m => m.GetAll(), Times.Once());
        }

        /// <summary>
        /// Index Test case
        /// </summary>
        [TestMethod]
        public void Index()
        {
            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            var returnObj = controller.Index();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
            var result = (ViewResult)returnObj;
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// System Conditions Test case
        /// </summary>
        [TestMethod]
        public void SystemConditions()
        {
            SystemConditions NewSystemConditionsLocal = new SystemConditions();
            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var returnObj = controller.SystemConditions();
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Throughput Chart Test case
        /// </summary>
        [TestMethod]
        public void ThroughputChart()
        {   
            double startingSS = 25.0;
            double resinLifeExpectancy = 364;
            int simulationconfidence = 95;
            int num_simulation_iterations = 100;
            string simMethod = "Modal value";
            int stdDev_threshold = 2;
            double resinAge = 0;
            double RTIcleaning_effectivness = 62.0;
            double Replacement_Level = 10;
            double RTIcleaning_Level = 17.0;
            double ReGen_effectivness = 99.75;
            string SelectedTrain = "0";
            bool DontReplaceResin = false;
            bool IsDashboard = false;
            double CleaningEffectiveness = 28.0;
            mockedPredictiveModelService.Setup(m => m.CalculateMinSaltSplit(1, "0")).Returns(new List<double>() { 1, 2 });
            mockedPredictiveModelService.Setup(m => m.CurrentSSConditions(1, 100, 100)).Returns(new Dictionary<double, double>());
			mockedPredictiveModelService.Setup(m => m.Thoughputchart(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(new PriceData() { CleanThroughput = new Dictionary<DateTime, Tuple<int, double, string>>(), NormalOpsThroughput = new Dictionary<DateTime, Tuple<int, double, string>>(), RegenTimeAverageBefore = 5, RegenTimeAverageAfter = 5, RegensPerWeekAverageBefore = 5, RegensPerWeekAverageAfter = 5, HoursPerRunAverageBefore = 5, HoursPerRunAverageAfter = 5, ThroughputAverageBefore = 5, ThroughputAverageAfter = 5 });
            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = 1;

            var returnObj = controller.ThroughputChart(startingSS, resinLifeExpectancy, simulationconfidence, num_simulation_iterations, simMethod, stdDev_threshold, resinAge, RTIcleaning_effectivness, Replacement_Level, RTIcleaning_Level, ReGen_effectivness, SelectedTrain, DontReplaceResin, CleaningEffectiveness, IsDashboard);
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
            var result = (ViewResult)returnObj;
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// PlotSalt Split Chart Test case
        /// </summary>
        [TestMethod]
        public void PlotSaltSplitChart()
        {
            double numWeeks = 364.0;
            double AvgResinage = 0;
            double startingSS = 25.0;
            double maxDegSS = 62.0;
            string SelectedTrain = "0";
            double CleaningEffectiveness = 28.0;
            bool IsDashboard = false;
            mockedPredictiveModelService.Setup(m => m.CalculateMinSaltSplit(1, SelectedTrain)).Returns(new List<double>() { 1, 2 });
            mockedPredictiveModelService.Setup(m => m.CurrentSSConditions(1, 100, 100)).Returns(new Dictionary<double, double>());
            mockedPredictiveModelService.Setup(m => m.ComputeDataPoints(numWeeks, startingSS, maxDegSS)).Returns(new Dictionary<double, double>());
            var controller = new PredictiveSystemController(mockedPredictiveRepository.Object, mockedPredictiveModelService.Object, mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object, mockedTrainRepository.Object);
            controller.ControllerContext = new ControllerContext();
            var mockedHttpContext = new Mock<HttpContextBase>();
            var mockedSessionState = new HttpSessionMock();
            mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
            controller.ControllerContext.HttpContext = mockedHttpContext.Object;
            controller.ControllerContext.HttpContext.Session["CustomerId"] = 1;
            var returnObj = controller.PlotSaltSplitChart(numWeeks, AvgResinage, startingSS, maxDegSS, SelectedTrain, CleaningEffectiveness, IsDashboard);
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
            var result = (PartialViewResult)returnObj;
            Assert.IsNotNull(result);
        }

        #endregion Methods
    }
}
