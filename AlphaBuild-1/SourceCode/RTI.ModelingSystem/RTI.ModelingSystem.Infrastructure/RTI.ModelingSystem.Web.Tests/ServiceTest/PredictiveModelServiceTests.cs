
namespace RTI.ModelingSystem.Web.Tests.ServiceTest
{
	#region Usings

	using System;
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
	/// PredictiveModelServiceTests class
	/// </summary>
	[TestClass]
	public class PredictiveModelServiceTests
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
		/// mocked Repository train
		/// </summary>
		Mock<IRepository<train>> mockedRepositoryTrain = new Mock<IRepository<train>>();

		/// <summary>
		/// mocked PredictiveModel Repository
		/// </summary>
		Mock<IPredictiveModelRepository> mockedPredictiveModelRepository = new Mock<IPredictiveModelRepository>();

		/// <summary>
		/// mocked Vessel Repository
		/// </summary>
		Mock<IVesselRepository> mockedVesselRepository = new Mock<IVesselRepository>();

		#endregion Properties

		#region Methods

		/// <summary>
		/// Compute Data Points Test case
		/// </summary>
		[TestMethod]
		public void ComputeDataPoints()
		{
			double numWeeks = 5;
			double startingss = 100;
			double maxDegSss = 10;
			SystemSummaryViewModel systemSummaryViewModel = new SystemSummaryViewModel();
			IPredictiveModelService service = new PredictiveModelService(mockedRepositoryTrain.Object, mockedRepositoryWaterData.Object, mockedPredictiveModelRepository.Object, mockedVesselRepository.Object);
			var returnObj = service.ComputeDataPoints(numWeeks, startingss, maxDegSss);
			Assert.IsNotNull(returnObj);
		}

		/// <summary>
		/// Calculates Min SaltSplit For Train Test case
		/// </summary>
		[TestMethod]
		public void CalculateMinSaltSplit()
		{
			long customerId = 0;
			string selectedTrainId = "0";
            double degredation = 0;
			double grainsWeightTotal = 100;
			List<train> lstTrains = new List<train>() { new train() { } };
			List<vessel> lstVessels = new List<vessel>() { new vessel() };
			mockedPredictiveModelRepository.Setup(m => m.GetCustomerTrains(customerId)).Returns(lstTrains);
			mockedPredictiveModelRepository.Setup(m => m.GetCustomerVessels(customerId)).Returns(lstVessels);
			mockedPredictiveModelRepository.Setup(m => m.GetGrainsWeightTotal(Convert.ToString(customerId))).Returns(grainsWeightTotal);
			IPredictiveModelService service = new PredictiveModelService(mockedRepositoryTrain.Object, mockedRepositoryWaterData.Object, mockedPredictiveModelRepository.Object, mockedVesselRepository.Object);
            var returnObj = service.CalculateMinSaltSplit(customerId, degredation, selectedTrainId);
			Assert.IsNotNull(returnObj);
		}

		/// <summary>
		/// Calculates Min SaltSplit For Train Test case
		/// </summary>
		[TestMethod]
		public void CalculateMinSaltSplitForTrain()
		{
			long customerId = 0;
			string selectedTrainId = "1";
            double degredation = 0;
			double grainsWeightTotal = 100;
			List<train> lstTrains = new List<train>() { new train() { } };
			List<vessel> lstVessels = new List<vessel>() { new vessel() { train_trainID = 1 } };
			mockedPredictiveModelRepository.Setup(m => m.GetCustomerTrains(customerId)).Returns(lstTrains);
			mockedPredictiveModelRepository.Setup(m => m.GetCustomerVessels(customerId)).Returns(lstVessels);
			mockedPredictiveModelRepository.Setup(m => m.GetGrainsWeightTotal(Convert.ToString(customerId))).Returns(grainsWeightTotal);
			IPredictiveModelService service = new PredictiveModelService(mockedRepositoryTrain.Object, mockedRepositoryWaterData.Object, mockedPredictiveModelRepository.Object, mockedVesselRepository.Object);
            var returnObj = service.CalculateMinSaltSplit(customerId, degredation, selectedTrainId);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(List<double>));
			var result = (List<double>)returnObj;
			Assert.IsNotNull(result);
			mockedPredictiveModelRepository.Verify(m => m.GetCustomerTrains(customerId), Times.Once());
			mockedPredictiveModelRepository.Verify(m => m.GetCustomerVessels(customerId), Times.Once());
			mockedPredictiveModelRepository.Verify(m => m.GetGrainsWeightTotal(Convert.ToString(customerId)), Times.Once());
		}
		
		/// <summary>
		/// Current SSConditions Test case
		/// </summary>
		[TestMethod]
		public void CurrentSSConditions()
		{
			double ResinAge = 30;
			double CleaningEffectiveness = 15;
			double startingSS = 10;
            double lifeExpectancy = 156;
			IPredictiveModelService service = new PredictiveModelService(mockedRepositoryTrain.Object, mockedRepositoryWaterData.Object, mockedPredictiveModelRepository.Object, mockedVesselRepository.Object);
			var returnObj = service.CurrentSSConditions(ResinAge,CleaningEffectiveness, startingSS, lifeExpectancy);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(Dictionary<double, double>));
			var result = (Dictionary<double, double>)returnObj;
			Assert.IsNotNull(result);
		}
		
		/// <summary>
		/// Current SSConditions Test case
		/// </summary>
		[TestMethod]
		public void Thoughputchart()
		{
            string CustomerId = "1";
            double Cur_ss = 0;
            long customerId = 0;
            double[] trains = new double[2];
            trains[0] = 0.0;
            trains[1] = 0.0;
			string selectedTrainId = "1";
			double startingSS = 99.75, resinLifeExpectancy = 3;
			int simulationconfidence = 10;
            int num_simulation_iterations =10;
            string simMethod = "Min";
            int stdDev_threshold =10;
            bool DontReplaceResin = false;
			double resinAge = 3, Replacement_Level = 10, RTIcleaning_Level = 10;
			string SelectedTrain = "0";
			List<vessel>  lstVessels = new List<vessel>() { new vessel() };
            mockedPredictiveModelRepository.Setup(m => m.FetchSourceIdTP(selectedTrainId)).Returns(trains);
			mockedRepositoryWaterData.Setup(m => m.GetAll()).Returns((new List<water_data>() { new water_data() { sourceID = 0 } }).AsQueryable());
			mockedRepositoryTrain.Setup(m => m.GetAll()).Returns(new List<train>().AsQueryable());
			mockedPredictiveModelRepository.Setup(m => m.GetCustomerVessels(customerId)).Returns(lstVessels);
			IPredictiveModelService service = new PredictiveModelService(mockedRepositoryTrain.Object, mockedRepositoryWaterData.Object, mockedPredictiveModelRepository.Object, mockedVesselRepository.Object);
            var returnObj = service.Thoughputchart(CustomerId, Cur_ss, startingSS, resinLifeExpectancy, simulationconfidence, num_simulation_iterations, simMethod, stdDev_threshold, resinAge, Replacement_Level, RTIcleaning_Level, SelectedTrain, DontReplaceResin);
			Assert.IsNotNull(returnObj);
			Assert.IsInstanceOfType(returnObj, typeof(PriceData));
			var result = (PriceData)returnObj;
			Assert.IsNotNull(result);
			mockedPredictiveModelRepository.Verify(m => m.FetchSourceIdTP(selectedTrainId), Times.Once());			
			mockedRepositoryTrain.Verify(m => m.GetAll(), Times.Once());
		}

		#endregion Methods
	}
}