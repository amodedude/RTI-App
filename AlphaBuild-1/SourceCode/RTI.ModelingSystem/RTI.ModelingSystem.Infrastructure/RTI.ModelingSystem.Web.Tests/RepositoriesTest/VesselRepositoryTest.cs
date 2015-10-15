
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
	/// VesselRepositoryTest class
	/// </summary>
	[TestClass]
	public class VesselRepositoryTest
	{
		#region Properties

		/// <summary>
		/// mocked rtiContext
		/// </summary>
		Mock<RtiContext> mockedDbContext = new Mock<RtiContext>();

		#endregion Properties

	}
}