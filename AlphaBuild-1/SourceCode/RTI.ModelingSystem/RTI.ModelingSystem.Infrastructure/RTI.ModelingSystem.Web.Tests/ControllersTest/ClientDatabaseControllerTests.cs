
namespace RTI.ModelingSystem.Web.Tests.ControllersTest
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Web.Controllers; 
    using RTI.ModelingSystem.Core.DBModels;

	#endregion Usings

	/// <summary>
	/// ClientDatabaseControllerTests class
	/// </summary>
	[TestClass]
	public class ClientDatabaseControllerTests
	{
		#region Properties

		/// <summary>
		/// mocked Customer Repository
		/// </summary>
		Mock<ICustomerRepository> mockedCustomerRepository = new Mock<ICustomerRepository>();

		/// <summary>
		/// mocked Repository Customer
		/// </summary>
		Mock<IRepository<customer>> mockedRepositoryCustomer = new Mock<IRepository<customer>>();

		/// <summary>
		/// mocked Repository Source
		/// </summary>
		Mock<IRepository<source>> mockedRepositorySource = new Mock<IRepository<source>>();

		/// <summary>
		/// mocked Repository train
		/// </summary>
		Mock<IRepository<train>> mockedRepositoryTrain = new Mock<IRepository<train>>();

		/// <summary>
		/// mocked Repository vessel
		/// </summary>
		Mock<IRepository<vessel>> mockedRepositoryVessel = new Mock<IRepository<vessel>>();

		/// <summary>
		/// mocked Repository Source
		/// </summary>
		Mock<IRepository<customer_water>> mockedRepositoryCustomerWater = new Mock<IRepository<customer_water>>();

		/// <summary>
		/// mocked Resin Products Repository
		/// </summary>
		Mock<IResinProductsRepository> mockedResinProductsRepository = new Mock<IResinProductsRepository>();

		/// <summary>
		/// mocked Conductivity Repository
		/// </summary>
		Mock<IConductivityRepository> mockedConductivityRepository = new Mock<IConductivityRepository>();

		/// <summary>
		/// mocked Train Repository
		/// </summary>
		Mock<ITrainRepository> mockedTrainRepository = new Mock<ITrainRepository>();

		/// <summary>
		/// mocked Vessel Repository
		/// </summary>
		Mock<IVesselRepository> mockedVesselRepository = new Mock<IVesselRepository>();

		#endregion Properties

		#region Methods

		/// <summary>
		/// DashBoard Test case
		/// </summary>
		[TestMethod]
		public void DashBoard()
		{
			long customerId = 1;
			SystemSummaryViewModel systemSummaryViewModel = new SystemSummaryViewModel();
			mockedRepositoryCustomer.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
			mockedCustomerRepository.Setup(m => m.GetWaterSourceIds(customerId)).Returns(new customer_water() { first_sourceID = 1, second_sourceID = 1 });
			mockedCustomerRepository.Setup(m => m.GetCustomerTrains(customerId)).Returns(new List<train>());
			mockedRepositoryVessel.Setup(m => m.GetAll()).Returns((new List<vessel>()).AsQueryable());

			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			controller.ControllerContext.HttpContext.Session["HasTrainDetails"] = "Verify";

			var returnObj = controller.DashBoard();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
			mockedCustomerRepository.Verify(m => m.GetWaterSourceIds(customerId), Times.Once());
			mockedCustomerRepository.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
			mockedRepositoryVessel.Verify(m => m.GetAll(), Times.Once());
		}

		/// <summary>
		/// Get System Settings case
		/// </summary>
		[TestMethod]
		public void GetSystemSettings()
		{
			long customerId = 1;
			string stringifiedCustomerId = Convert.ToString(customerId);
			train train = new train() { using_manifold = "NO" };
			source source = new source() { state_name = "Florida", city = "Florida", sources_sourceID = 1 };
			customer_water customerWater = new customer_water() { first_sourceID = 1, second_sourceID = 1 };
			mockedCustomerRepository.Setup(m => m.FindById(stringifiedCustomerId)).Returns(new customer() { state = "Florida" });
			mockedRepositoryTrain.Setup(m => m.GetAll()).Returns(new List<train>() { train }.AsQueryable());
			mockedRepositorySource.Setup(m => m.GetAll()).Returns(new List<source>() { source }.AsQueryable());
			mockedRepositoryCustomerWater.Setup(m => m.GetAll()).Returns(new List<customer_water>() { customerWater }.AsQueryable());
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.GetSystemSettings();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.FindById(stringifiedCustomerId), Times.Once());
			mockedRepositoryTrain.Verify(m => m.GetAll(), Times.Once());
			mockedRepositorySource.Verify(m => m.GetAll(), Times.Once());
			mockedRepositoryCustomerWater.Verify(m => m.GetAll(), Times.Once());
		}

		/// <summary>
		/// Train Settings case
		/// </summary>
		[TestMethod]
		public void TrainSettings()
		{
			string data = "0";
			int trainId = 1;
			long customerId = 1;
			int resinProductId = 1;
			vessel vessel = new vessel() { with_degassifier = string.Empty, vessel_number = 1, Degasifier = true, with_polisher = string.Empty, Polisher = true };
			mockedRepositoryTrain.Setup(m => m.GetAll()).Returns(new List<train>().AsQueryable());
			mockedRepositoryCustomer.Setup(m => m.GetById(customerId)).Returns(new customer());
			mockedConductivityRepository.Setup(m => m.GetTrains(Convert.ToInt32(customerId))).Returns(customerId);
			mockedConductivityRepository.Setup(m => m.GetTrainIdByCsutomer(Convert.ToInt32(customerId))).Returns(customerId);
			mockedConductivityRepository.Setup(m => m.GetTrainDetailsByCustomerIdandTrainId(trainId, Convert.ToInt32(customerId))).Returns(new train() { num_beds_cation = 2, num_beds_anion = 2, using_manifold = "NO" });
			mockedVesselRepository.Setup(m => m.FetchVesselsList(trainId)).Returns(new List<vessel>() { vessel });
			mockedVesselRepository.Setup(m => m.GetResinById(resinProductId)).Returns(new resin_products() { model_number = string.Empty, primary_type = string.Empty, manufacturer = string.Empty });
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.TrainSettings(data);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.ViewName, "_bedsettings");
			mockedRepositoryCustomer.Verify(m => m.GetById(customerId), Times.Once());
			mockedConductivityRepository.Verify(m => m.GetTrains(Convert.ToInt32(customerId)), Times.Once());
		}

		/// <summary>
		/// Train Settings case
		/// </summary>
		[TestMethod]
		public void TrainSettingsNew()
		{
			string data = null;
			int trainId = 1;
			long customerId = 1;
			int resinProductId = 1;
			vessel vessel = new vessel() { with_degassifier = string.Empty, vessel_number = 1, Degasifier = true, with_polisher = string.Empty, Polisher = true };
			mockedRepositoryTrain.Setup(m => m.GetAll()).Returns(new List<train>().AsQueryable());
			mockedRepositoryCustomer.Setup(m => m.GetById(customerId)).Returns(new customer());
			mockedConductivityRepository.Setup(m => m.GetTrains(Convert.ToInt32(customerId))).Returns(customerId);
			mockedConductivityRepository.Setup(m => m.GetTrainIdByCsutomer(Convert.ToInt32(customerId))).Returns(customerId);
			mockedConductivityRepository.Setup(m => m.GetTrainDetailsByCustomerIdandTrainId(trainId, Convert.ToInt32(customerId))).Returns(new train() { num_beds_cation = 2, num_beds_anion = 2, using_manifold = "NO" });
			mockedVesselRepository.Setup(m => m.FetchVesselsList(trainId)).Returns(new List<vessel>() { vessel });
			mockedVesselRepository.Setup(m => m.GetResinById(resinProductId)).Returns(new resin_products() { model_number = string.Empty, primary_type = string.Empty, manufacturer = string.Empty });
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.TrainSettings(data);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.ViewName, "_TrainSettings");
			mockedRepositoryCustomer.Verify(m => m.GetById(customerId), Times.Once());
			mockedConductivityRepository.Verify(m => m.GetTrains(Convert.ToInt32(customerId)), Times.Once());
			mockedConductivityRepository.Verify(m => m.GetTrainIdByCsutomer(Convert.ToInt32(customerId)), Times.Once());
			mockedConductivityRepository.Verify(m => m.GetTrainDetailsByCustomerIdandTrainId(trainId, Convert.ToInt32(customerId)), Times.Once());
			mockedVesselRepository.Verify(m => m.FetchVesselsList(trainId), Times.Once());
		}

		/// <summary>
		/// All Resin Product Details By ProductName case
		/// </summary>
		[TestMethod]
		public void GetAllResinProductDetailsByProductName()
		{
			string productId = "1";
			mockedResinProductsRepository.Setup(item => item.GetAllResinProductDetailsByProductName(productId)).Returns(new List<resin_products>());
			mockedResinProductsRepository.Setup(item => item.GetAllResinProductNames()).Returns(new List<string>());
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			var returnObj = controller.GetAllResinProductDetailsByProductName(productId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "PartialResinLookupModal_Child");
			mockedResinProductsRepository.Verify(m => m.GetAllResinProductDetailsByProductName(productId), Times.Once());
			mockedResinProductsRepository.Verify(m => m.GetAllResinProductNames(), Times.Once());

		}

		/// <summary>
		/// Get Resin Lookup Modal case
		/// </summary>
		[TestMethod]
		public void GetResinLookupModal()
		{
			string bedType = "Cation";
			mockedResinProductsRepository.Setup(item => item.GetAllResinProductNames()).Returns(new List<string>());
			mockedResinProductsRepository.Setup(item => item.GetAllResinProductDetails()).Returns(new List<resin_products>());
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			var returnObj = controller.GetResinLookupModal(bedType, string.Empty, string.Empty);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.ViewName, "PartialResinLookupModal");
			mockedResinProductsRepository.Verify(m => m.GetAllResinProductNames(), Times.Once());
			mockedResinProductsRepository.Verify(m => m.GetAllResinProductDetails(), Times.Once());
		}

		/// <summary>
		/// Auto complete case
		/// </summary>
		[TestMethod]
		public void Autocomplete()
		{
			string term = "PFA";
			mockedTrainRepository.Setup(item => item.ResinProductModelsData(term)).Returns(new List<resin_products>());
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			var returnObj = controller.Autocomplete(term);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(JsonResult));
			var result = (JsonResult)returnObj;
			Assert.IsNotNull(result);
			mockedTrainRepository.Verify(m => m.ResinProductModelsData(term), Times.Once());
		}

		/// <summary>
		/// Update System Settings case
		/// </summary>
		[TestMethod]
		public void UpdateSystemSettingsExistingCustomer()
		{
			string customerId = "1";
			SystemSettings systemSettings = new SystemSettings();
			mockedConductivityRepository.Setup(m => m.AddSystemSettings(systemSettings, customerId)).Verifiable();
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["IsNewCustomer"] = "True";
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.UpdateSystemSettings(systemSettings);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedConductivityRepository.Verify(m => m.AddSystemSettings(systemSettings, customerId), Times.Once());
		}

		/// <summary>
		/// Update System Settings case
		/// </summary>
		[TestMethod]
		public void UpdateSystemSettings()
		{
			string customerId = "1";
			SystemSettings systemSettings = new SystemSettings();
			mockedConductivityRepository.Setup(m => m.UpdateSystemSettings(systemSettings, customerId)).Verifiable();
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["IsNewCustomer"] = "False";
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.UpdateSystemSettings(systemSettings);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedConductivityRepository.Verify(m => m.UpdateSystemSettings(systemSettings, customerId), Times.Once());
		}

		/// <summary>
		/// Train Settings Save case
		/// </summary>
		[TestMethod]
		public void TrainSettingsSave()
		{
			long customerId = 1;
			vessel vessel = new vessel() { UpdateVessel = "True", with_degassifier = string.Empty, Degasifier = true, with_polisher = string.Empty, Polisher = true, throughput = string.Empty, ResinModel = string.Empty, resin_data_product_id = 1, vessel_number = 1, bed_number = "1", vessel_customerID = "1", train_trainID = 1 };
			Trainsettings trainSettings = new Trainsettings() { Train = new train() { trainID = 1 }, VesslsList = new List<vessel>() { vessel } };
			mockedVesselRepository.Setup(m => m.FetchVesselsList(trainSettings.Train.trainID)).Returns(new List<vessel>() { new vessel() });
			mockedRepositoryTrain.Setup(m => m.Update(trainSettings.Train)).Verifiable();
			mockedRepositoryTrain.Setup(m => m.SubmitChanges()).Verifiable();
			mockedVesselRepository.Setup(m => m.GetResinId(trainSettings.VesslsList[0].ResinModel)).Verifiable();
			mockedVesselRepository.Setup(m => m.UpdateVessel(trainSettings.VesslsList[0])).Verifiable();
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.TrainSettings(trainSettings);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedVesselRepository.Verify(m => m.FetchVesselsList(trainSettings.Train.trainID), Times.Once());
			mockedVesselRepository.Verify(m => m.GetResinId(trainSettings.VesslsList[0].ResinModel), Times.Once());
			mockedVesselRepository.Verify(m => m.UpdateVessel(trainSettings.VesslsList[0]), Times.Once());
			mockedRepositoryTrain.Verify(m => m.SubmitChanges(), Times.Once());
		}

		/// <summary>
		/// Train Settings Save case
		/// </summary>
		[TestMethod]
		public void TrainSettingsWithoutVesselsSave()
		{
			long customerId = 1;
			vessel vessel = new vessel() { UpdateVessel = "True", with_degassifier = string.Empty, Degasifier = true, with_polisher = string.Empty, Polisher = true, throughput = string.Empty, ResinModel = string.Empty, resin_data_product_id = 1, vessel_number = 1, bed_number = "1", vessel_customerID = "1", train_trainID = 1 };
			Trainsettings trainSettings = new Trainsettings() { Train = new train(), VesslsList = new List<vessel>() { vessel } };
			mockedRepositoryTrain.Setup(m => m.Insert(trainSettings.Train)).Verifiable();
			mockedRepositoryTrain.Setup(m => m.SubmitChanges()).Verifiable();
			mockedVesselRepository.Setup(m => m.GetResinId(vessel.ResinModel)).Verifiable();
			mockedVesselRepository.Setup(m => m.InsertVessel(vessel)).Verifiable();
			mockedConductivityRepository.Setup(m => m.GetTrainDetailsByCustomerIdandTrainId((long)trainSettings.Train.trainID, (int)customerId)).Verifiable();
			var controller = new ClientDatabaseController(mockedConductivityRepository.Object, mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object, mockedRepositoryTrain.Object, mockedRepositoryCustomerWater.Object, mockedVesselRepository.Object, mockedRepositoryVessel.Object, mockedTrainRepository.Object, mockedResinProductsRepository.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ControllerContext.HttpContext.Session["CustomerId"] = customerId;
			var returnObj = controller.TrainSettings(trainSettings);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedVesselRepository.Verify(m => m.GetResinId(trainSettings.VesslsList[0].ResinModel), Times.Once());
			mockedVesselRepository.Verify(m => m.InsertVessel(trainSettings.VesslsList[0]), Times.Once());
			mockedRepositoryTrain.Verify(m => m.Insert(trainSettings.Train), Times.Once());
			mockedRepositoryTrain.Verify(m => m.SubmitChanges(), Times.Once());
		}

		#endregion Methods
	}
}