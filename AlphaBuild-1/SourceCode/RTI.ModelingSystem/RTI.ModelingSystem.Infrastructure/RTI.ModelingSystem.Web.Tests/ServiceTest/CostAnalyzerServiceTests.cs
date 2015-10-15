
namespace RTI.ModelingSystem.Web.Tests.ServiceTest
{
	#region Usings

	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using RTI.ModelingSystem.Core.DBModels;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Interfaces.Services;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Implementation.Services;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	#endregion Usings

	/// <summary>
	/// CostAnalyzerServiceTests class
	/// </summary>
	[TestClass]
	public class CostAnalyzerServiceTests
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
		/// mocked Train Repository
		/// </summary>
        Mock<ITrainRepository> mockedTrainRepository = new Mock<ITrainRepository>();
        Mock<IRepository<vessel>> mockedVesselRepository = new Mock<IRepository<vessel>>();

		#endregion Properties

		#region Methods

		/// <summary>
		/// Get Cost Settings Test case
		/// </summary>
		[TestMethod]
		public void GetCostSettings()
		{
			string selectedTrain = "2";
			string customerId = "1";
			mockedCustRepository.Setup(m => m.FindById(customerId)).Returns(new customer());
			mockedTrainRepository.Setup(m => m.FindById(selectedTrain)).Returns(new train());
            mockedVesselRepository.Setup(m => m.GetById(selectedTrain)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                        mockedTrainRepository.Object,
                                                        mockedVesselRepository.Object);
			var returnObj = service.GetCostSettings(customerId, selectedTrain);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(CostSettings));
			var result = (CostSettings)returnObj;
			Assert.IsNotNull(result);
            //mockedCustRepository.Verify(m => m.FindById(customerId), Times.Once(), "Exception occured");
            //mockedTrainRepository.Verify(m => m.FindById(selectedTrain), Times.Once(), "Exception occured");
            //mockedVesselRepository.Verify(m => m.GetById(selectedTrain), Times.Once(), "Exception occured");
		}

		/// <summary>
		/// Get Cost Settings Test case
		/// </summary>
		[TestMethod]
		public void GetCostSettingsForAllTrains()
		{
			string selectedTrain = "2";
			string customerId = "1";
			mockedCustRepository.Setup(m => m.FindById(customerId)).Returns(new customer());
            mockedVesselRepository.Setup(m => m.GetById(selectedTrain)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                                mockedTrainRepository.Object,
                                                                mockedVesselRepository.Object);
			var returnObj = service.GetCostSettings(customerId, selectedTrain);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(CostSettings));
			var result = (CostSettings)returnObj;
			Assert.IsNotNull(result);
			mockedCustRepository.Verify(m => m.FindById(customerId), Times.Once());
		}

		/// <summary>
		/// Open Cost Window Test case
		/// </summary>
		[TestMethod]
		public void OpenCostWindow()
		{
			List<Tuple<int, double>> RegensPerWeekNormalOps = new List<Tuple<int, double>>();
			RegensPerWeekNormalOps.Add(new Tuple<int, double>(1, 2));
			List<Tuple<int, double>> RegensPerWeekClean = new List<Tuple<int, double>>();
			RegensPerWeekClean.Add(new Tuple<int, double>(1, 2));
			Dictionary<DateTime, Tuple<int, double, string>> NormalOpsThroughput = new Dictionary<DateTime, Tuple<int, double, string>>();
			NormalOpsThroughput.Add(DateTime.Now, new Tuple<int, double, string>(1, 2, "Replace"));
			Dictionary<DateTime, Tuple<int, double, string>> CleanThroughput = new Dictionary<DateTime, Tuple<int, double, string>>();
			CleanThroughput.Add(DateTime.Now, new Tuple<int, double, string>(1, 2, "Replace"));
			PriceData dataToSend = new PriceData() { RegensPerWeekNormalOps = RegensPerWeekNormalOps, RegensPerWeekClean = RegensPerWeekClean, NormalOpsThroughput = NormalOpsThroughput, CleanThroughput = CleanThroughput };
			int currentTrain = 1;
			string customerId = "1";
			mockedCustRepository.Setup(m => m.FindById(customerId)).Returns(new customer() { acid_price = 1, caustic_price = 1 });
            mockedVesselRepository.Setup(m => m.GetById(currentTrain)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                                    mockedTrainRepository.Object,
                                                                   mockedVesselRepository.Object );
			var returnObj = service.OpenCostWindow(dataToSend, currentTrain, customerId, true);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(List<Tuple<int, double, double>>));
			var result = (List<Tuple<int, double, double>>)returnObj;
			Assert.IsNotNull(result);
			mockedCustRepository.Verify(m => m.FindById(customerId), Times.Once());
		}
		
		/// <summary>
		/// Gets Cumulative Savings Test case
		/// </summary>
		[TestMethod]
		public void GetCumulativeSavings()
		{
            mockedVesselRepository.Setup(m => m.GetById(1)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                            mockedTrainRepository.Object,
                                                            mockedVesselRepository.Object);
			var returnObj = service.GetCumulativeSavings();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(List<string>));
			var result = (List<string>)returnObj;
			Assert.IsNotNull(result);
		}

		/// <summary>
		/// Selected Week Data Finder Test case
		/// </summary>
		[TestMethod]
		public void SelectedWeekDataFinder()
		{
			double week = 1;
            mockedVesselRepository.Setup(m => m.GetById(1)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                            mockedTrainRepository.Object,
                                                            mockedVesselRepository.Object);
			var returnObj = service.SelectedWeekDataFinder(week);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(double?[]));
			var result = (double?[])returnObj;
			Assert.IsNotNull(result);
		}
		
		/// <summary>
		/// Get Cost Analyzer Results Data Test case
		/// </summary>
		[TestMethod]
		public void GetCostAnalyzerResultsData()
		{
			//double week = 1;
            mockedVesselRepository.Setup(m => m.GetById(1)).Returns(new vessel());
			ICostAnalyzerService service = new CostAnalyzerService(mockedCustRepository.Object, 
                                                                    mockedTrainRepository.Object,
                                                                    mockedVesselRepository.Object);
			var returnObj = service.GetCostAnalyzerResultsData();
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(CostAnalyzerResult));
			var result = (CostAnalyzerResult)returnObj;
			Assert.IsNotNull(result);
		}

		#endregion Methods
	}
}
