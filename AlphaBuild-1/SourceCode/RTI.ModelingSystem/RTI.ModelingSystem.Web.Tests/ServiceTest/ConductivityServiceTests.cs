
namespace RTI.ModelingSystem.Web.Tests.ServiceTest
{
	#region Usings

	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Interfaces.Services;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Implementation.Services;
    using RTI.ModelingSystem.Core.DBModels;

	#endregion Usings

	/// <summary>
	/// ConductivityServiceTests class
	/// </summary>
	[TestClass]
	public class ConductivityServiceTests
	{
		#region Properties

		/// <summary>
		/// mocked Repository WaterData
		/// </summary>
		Mock<IRepository<water_data>> mockedRepositoryWaterData = new Mock<IRepository<water_data>>();

		/// <summary>
		/// mocked Repository Source
		/// </summary>
		Mock<IRepository<source>> mockedRepositorySource = new Mock<IRepository<source>>();

        /// <summary>
        /// mocked Repository Source
        /// </summary>
        Mock<IConductivityRepository> mockedCondRepository = new Mock<IConductivityRepository>();
        

		#endregion Properties

		#region Methods

		/// <summary>
		/// Get the Conductivity data Test case
		/// </summary>
		[TestMethod]
		public void GetConductivitydata()
		{
			long usgsId = 5;
			water_data waterData = new water_data() { dataID = 1 };
			mockedRepositoryWaterData.Setup(m => m.GetById(usgsId)).Returns(waterData);
			IConductivityService service = new ConductivityService(mockedRepositorySource.Object, mockedRepositoryWaterData.Object,mockedCondRepository.Object);
			var returnObj = service.GetConductivitydata(usgsId);
		}

		/// <summary>
		/// Calculates the Statistics Test case
		/// </summary>
		[TestMethod]
		public void CalculateStatistics()
		{
			int usgsId = 5;
			List<water_data> lstWaterData = new List<water_data>() { new water_data() { sourceID = 1 } };
			mockedRepositoryWaterData.Setup(m => m.GetAll()).Returns(lstWaterData.AsQueryable());
            IConductivityService service = new ConductivityService(mockedRepositorySource.Object, mockedRepositoryWaterData.Object, mockedCondRepository.Object);
			var returnObj = service.CalculateStatistics(usgsId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ConductivityStatistics));
			var result = (ConductivityStatistics)returnObj;
		}

		/// <summary>
		/// Gets the Forecast data Test case
		/// </summary>
		[TestMethod]
		public void GetForecastdata()
		{
			long usgsId = 5;
			List<water_data> lstWaterData = new List<water_data>() { new water_data() { sourceID = 1 } };
			mockedRepositoryWaterData.Setup(m => m.GetAll()).Returns(lstWaterData.AsQueryable());
            IConductivityService service = new ConductivityService(mockedRepositorySource.Object, mockedRepositoryWaterData.Object, mockedCondRepository.Object);
			var returnObj = service.GetForecastdata(usgsId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(ForecastData));
			var result = (ForecastData)returnObj;
		}

		#endregion Methods
	}
}
