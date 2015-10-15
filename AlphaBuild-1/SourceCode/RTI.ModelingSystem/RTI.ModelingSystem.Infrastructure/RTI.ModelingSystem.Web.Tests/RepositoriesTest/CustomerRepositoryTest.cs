
namespace RTI.ModelingSystem.Web.Tests.RepositoriesTest
{
	#region Usings

	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Data;
	using RTI.ModelingSystem.Infrastructure.Implementation.Repository;

	#endregion Usings

	/// <summary>
	/// CustomerRepositoryTest class
	/// </summary>
	[TestClass]
	public class CustomerRepositoryTest
	{
		#region Properties

		/// <summary>
		/// mocked rtiContext
		/// </summary>
		Mock<RtiContext> mockedDbContext = new Mock<RtiContext>();

		#endregion Properties

		#region Methods

		///// <summary>
		///// FindById Test case
		///// </summary>
		//[TestMethod]
		//public void FindById()
		//{
		//	string customerId = "1";
		//	ICustomerRepository customerRepository = new CustomerRepository(mockedDbContext.Object);
		//	customer returnObj = customerRepository.FindById(customerId);
		//	Assert.IsNotNull(returnObj);
		//	Assert.IsInstanceOfType(returnObj, typeof(customer));
		//	var result = (customer)returnObj;
		//	Assert.IsNotNull(result);
		//}

		#endregion Methods
	}
}
