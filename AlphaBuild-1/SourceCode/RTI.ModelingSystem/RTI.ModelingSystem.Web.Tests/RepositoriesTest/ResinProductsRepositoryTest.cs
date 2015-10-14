
namespace RTI.ModelingSystem.Web.Tests.RepositoriesTest
{
	#region Usings

	using System.Collections.Generic;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Data;
	using RTI.ModelingSystem.Infrastructure.Implementation.Repository;

	#endregion Usings

	/// <summary>
	/// ResinProductsRepositoryTest class
	/// </summary>
	[TestClass]
	public class ResinProductsRepositoryTest
	{
		#region Properties

		/// <summary>
		/// mocked rtiContext
		/// </summary>
		Mock<RtiContext> mockedDbContext = new Mock<RtiContext>();

		#endregion Properties		
	}
}
