
namespace RTI.ModelingSystem.Web.Tests.ControllersTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;
    using RTI.ModelingSystem.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// WaterConductivityControllerTests class
    /// </summary>
    [TestClass]
    public class WaterConductivityControllerTests
    {
        #region Properties

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
            mockedCustomerRepository.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
            mockedRepositoryCustomer.Setup(m => m.GetWaterSourceIds(customerId)).Returns(new customer_water() { first_sourceID = 1, second_sourceID = 1 });
            mockedRepositoryCustomer.Setup(m => m.GetCustomerTrains(customerId)).Returns(new List<train>());
            mockedVesselRepository.Setup(m => m.GetAll()).Returns((new List<vessel>()).AsQueryable());

            var controller = new WaterConductivityController(mockedCustomerRepository.Object, mockedRepositoryCustomer.Object, mockedVesselRepository.Object);
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
            mockedCustomerRepository.Verify(m => m.GetAll(), Times.Once());
            mockedRepositoryCustomer.Verify(m => m.GetWaterSourceIds(customerId), Times.Once());
            mockedRepositoryCustomer.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
            mockedVesselRepository.Verify(m => m.GetAll(), Times.Once());   
        }

        #endregion Methods
    }
}
