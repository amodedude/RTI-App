
namespace RTI.ModelingSystem.Web.Tests
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
	/// HttpSessionMock class
	/// </summary>
	internal sealed class HttpSessionMock : HttpSessionStateBase
	{
		private readonly Dictionary<string, object> objects = new Dictionary<string, object>();

		public override object this[string name]
		{
			get { return (objects.ContainsKey(name)) ? objects[name] : null; }
			set { objects[name] = value; }
		}
	}

	/// <summary>
	/// CustomerControllerTests class
	/// </summary>
	[TestClass]
	public class CustomerControllerTests
	{
		#region Properties

		/// <summary>
		/// mocked Repository Customer
		/// </summary>
		Mock<IRepository<customer>> mockedRepositoryCustomer = new Mock<IRepository<customer>>();

		/// <summary>
		/// mocked Customer Repository
		/// </summary>
		Mock<ICustomerRepository> mockedCustomerRepository = new Mock<ICustomerRepository>();

		/// <summary>
		/// mocked Repository Source
		/// </summary>
		Mock<IRepository<source>> mockedRepositorySource = new Mock<IRepository<source>>();

		#endregion Properties

		#region Methods

		/// <summary>
		/// Index Test case
		/// </summary>
		[TestMethod]
		public void IndexTest()
		{
			mockedRepositoryCustomer.Setup(m => m.GetAll()).Returns(new List<customer>().AsQueryable());
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Index();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
		}

		/// <summary>
		/// Details Test
		/// </summary>
		[TestMethod]
		public void DetailsTest()
		{
			var mockedRepositoryCustomer = new Mock<IRepository<customer>>();
			var mockedCustomerRepository = new Mock<ICustomerRepository>();
			var mockedRepositorySource = new Mock<IRepository<source>>();
			mockedCustomerRepository.Setup(m => m.FindById(It.IsAny<string>())).Returns(new customer());
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var result = controller.Details("234");
			mockedCustomerRepository.Verify(m => m.FindById(It.IsAny<string>()), Times.Once());
		}

		/// <summary>
		/// Create Without Existing CustomerId Without Duplicate
		/// </summary>
		[TestMethod]
		public void CreateWithoutExistingCustomerIdWithoutDuplicate()
		{
			var customer = new customer { customerID = (long)1, name = "Test customer", state = "Florida" };
			customer.customerList = new List<SelectListItem>();
			customer.StateList = new List<SelectListItem>();
			customer.TrainList = new List<SelectListItem>();
			customer.CityList = new List<SelectListItem>();
			mockedCustomerRepository.Setup(m => m.FindById(It.IsAny<string>())).Verifiable();
			var customerList = new List<customer> { customer };
			mockedRepositoryCustomer.Setup(m => m.GetAll()).Returns(customerList.AsQueryable());
			var stateList = new List<source>() { new source { state = "FL", state_name = "Florida", city = "Florida" } };
			mockedCustomerRepository.Setup(m => m.GetStateList()).Returns(stateList);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			long? customerId = null;
			var returnObj = controller.Create(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.ViewName, "_Create");
			mockedCustomerRepository.Verify(m => m.FindById(Convert.ToString(It.IsAny<string>())), Times.Never());
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
			mockedCustomerRepository.Verify(m => m.GetStateList(), Times.Once());
		}

		/// <summary>
		/// Create Without Existing CustomerId With Duplicate
		/// </summary>
		[TestMethod]
		public void CreateWithoutExistingCustomerIdWithDuplicate()
		{
			var customer = new customer { customerID = (long)1, name = "Test customer", state = "Florida" };
			customer.customerList = new List<SelectListItem>();
			customer.StateList = new List<SelectListItem>();
			customer.TrainList = new List<SelectListItem>();
			customer.CityList = new List<SelectListItem>();
			mockedCustomerRepository.Setup(m => m.FindById(It.IsAny<string>())).Verifiable();
			var customerList = new List<customer> { customer };
			mockedRepositoryCustomer.Setup(m => m.GetAll()).Returns(customerList.AsQueryable());
			var stateList = new List<source>() { new source { state = "FL", state_name = "Florida", city = "Florida" } };
			mockedCustomerRepository.Setup(m => m.GetStateList()).Returns(stateList);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedSessionState["IsDuplicate"] = "True";
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			long? customerId = null;
			var returnObj = controller.Create(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.ViewName, "_Create");
			Assert.AreEqual(result.ViewData.ModelState.IsValid, false);
			Assert.AreEqual(result.ViewData.ModelState["notes"].Errors[0].ErrorMessage, "Customer Already exists");
			mockedCustomerRepository.Verify(m => m.FindById(Convert.ToString(It.IsAny<string>())), Times.Never());
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
			mockedCustomerRepository.Verify(m => m.GetStateList(), Times.Once());
		}

		/// <summary>
		/// Create With ExistingCustomerId
		/// </summary>
		[TestMethod]
		public void CreateWithExistingCustomerId()
		{
			long? existingcustomerId = 1;
			string stringifiedexistingCustomerId = Convert.ToString(existingcustomerId.Value);
			var customer = new customer { customerID = existingcustomerId.Value, name = "Test customer", state = "Florida" };
			customer.customerList = new List<SelectListItem>();
			customer.StateList = new List<SelectListItem>();
			customer.TrainList = new List<SelectListItem>();
			customer.CityList = new List<SelectListItem>();
			mockedCustomerRepository.Setup(m => m.FindById(stringifiedexistingCustomerId)).Returns(customer);
			var customerList = new List<customer> { customer };
			mockedRepositoryCustomer.Setup(m => m.GetAll()).Returns(customerList.AsQueryable());
			var stateList = new List<source>() { new source { state = "FL", state_name = "Florida", city = "Florida" } };
			mockedCustomerRepository.Setup(m => m.GetStateList()).Returns(stateList);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			var returnObj = controller.Create(existingcustomerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PartialViewResult));
			var result = (PartialViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.FindById(Convert.ToString(existingcustomerId)), Times.Once());
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
			mockedCustomerRepository.Verify(m => m.GetStateList(), Times.Once());
		}

		/// <summary>
		/// Saves the customer
		/// </summary>
		[TestMethod]
		public void SaveCustomer()
		{
			customer customer = new customer() { customerID = -1 };
			long newCustomerId = 0;
			mockedRepositoryCustomer.Setup(item => item.InsertAndGetID(customer)).Returns(newCustomerId);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			var returnObj = controller.Create(customer);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedRepositoryCustomer.Verify(m => m.InsertAndGetID(customer), Times.Once());
		}

		/// <summary>
		/// Saves the customer
		/// </summary>
		[TestMethod]
		public void UpdateCustomer()
		{
			customer customer = new customer() { customerID = 1 };
			bool IsDuplicate = false;
			mockedCustomerRepository.Setup(item => item.CheckForDuplicates(customer)).Returns(IsDuplicate);
			mockedCustomerRepository.Setup(item => item.UpdateCustomer(customer)).Verifiable();
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			var returnObj = controller.Create(customer);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.CheckForDuplicates(customer), Times.Never());
			mockedCustomerRepository.Verify(m => m.UpdateCustomer(customer), Times.Once());
		}

		/// <summary>
		/// Saves the customer
		/// </summary>
		[TestMethod]
		public void UpdateCustomerWithErrors()
		{
			customer customer = new customer() { customerID = 1 };
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ModelState.AddModelError("Error", "ErrorMessage"); ;
			var returnObj = controller.Create(customer);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.CheckForDuplicates(customer), Times.Never());
			mockedCustomerRepository.Verify(m => m.UpdateCustomer(customer), Times.Never());
		}

		/// <summary>
		/// Edits the customer
		/// </summary>
		[TestMethod]
		public void EditNonExistingCustomer()
		{
			string customerId = null;
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Edit(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(HttpStatusCodeResult));
			var result = (HttpStatusCodeResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.StatusCode, 400);
		}

		/// <summary>
		/// Edits the customer
		/// </summary>
		[TestMethod]
		public void EditCustomer()
		{
			string customerId = "1";
			List<customer> lstCustomer = new List<customer>();
			customer customer = new customer() { CityList = new List<SelectListItem>(), customerList = new List<SelectListItem>() };
			lstCustomer.Add(customer);
			mockedCustomerRepository.Setup(item => item.FindById(customerId)).Returns(customer);
			mockedRepositoryCustomer.Setup(item => item.GetAll()).Returns(lstCustomer.AsQueryable());
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Edit(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.FindById(customerId), Times.Once());
			mockedRepositoryCustomer.Verify(m => m.GetAll(), Times.Once());
		}

		/// <summary>
		/// Saves edited customer
		/// </summary>
		[TestMethod]
		public void EditCustomerSave()
		{
			customer customer = new customer();
			mockedCustomerRepository.Setup(item => item.UpdateCustomer(customer)).Verifiable();
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Edit(customer);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(RedirectToRouteResult));
			var result = (RedirectToRouteResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.UpdateCustomer(customer), Times.Once());
		}
		
		/// <summary>
		/// Saves edited customer
		/// </summary>
		[TestMethod]
		public void EditCustomerSaveFail()
		{
			customer customer = new customer();
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			controller.ControllerContext = new ControllerContext();
			var mockedHttpContext = new Mock<HttpContextBase>();
			var mockedSessionState = new HttpSessionMock();
			mockedHttpContext.SetupGet(ctx => ctx.Session).Returns(mockedSessionState);
			controller.ControllerContext.HttpContext = mockedHttpContext.Object;
			controller.ModelState.AddModelError("Error", "ErrorMessage"); ;
			var returnObj = controller.Edit(customer);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
		}

		/// <summary>
		/// Deletes the customer
		/// </summary>
		[TestMethod]
		public void Deletevalid()
		{
			string customerId = "0";
			customer customer = new customer();
			mockedCustomerRepository.Setup(item => item.FindById(customerId)).Returns(customer);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Delete(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ViewResult));
			var result = (ViewResult)returnObj;
			Assert.IsNotNull(result);
			mockedCustomerRepository.Verify(m => m.FindById(customerId), Times.Once());
		}

		/// <summary>
		/// Deletes the customer
		/// </summary>
		[TestMethod]
		public void DeleteInvalid()
		{
			string customerId = null;
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.Delete(customerId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(HttpStatusCodeResult));
			var result = (HttpStatusCodeResult)returnObj;
			Assert.IsNotNull(result);
			Assert.AreEqual(result.StatusCode, 400);
		}

		/// <summary>
		/// Deletes the customer
		/// </summary>
		[TestMethod]
		public void GetCityListvalid()
		{
			string State = "Florida";
			List<SelectListItem> lstCity = new List<SelectListItem>();
			List<source> lstsource = new List<source>();
			lstsource.Add(new source() { state_name = "Florida" });
			mockedCustomerRepository.Setup(item => item.GetStateList()).Returns(lstsource);
			var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
			var returnObj = controller.GetCityList(State);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(JsonResult));
			var result = (JsonResult)returnObj;
			Assert.IsNotNull(result);
		}

        /// <summary>
		/// Checks For Duplicate
		/// </summary>
        [TestMethod]
        public void CheckForDuplicate()
        {
            string name = string.Empty;
            string state = string.Empty;
            string city = string.Empty;
            var controller = new CustomerController(mockedRepositoryCustomer.Object, mockedCustomerRepository.Object, mockedRepositorySource.Object);
            var returnObj = controller.CheckForDuplicate(name, state, city);
            Assert.IsNotNull(returnObj);
            Assert.IsInstanceOfType(returnObj, typeof(bool));
            var result = (bool)returnObj;
            Assert.IsNotNull(result);
        }

		#endregion Methods
	}
}