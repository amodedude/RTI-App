// -----------------------------------------------------------------------
// <copyright file="ThroughputChart.cs" company="RTI">
// RTI
// </copyright>
// <summary>Throughput Chart</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;

    #endregion Usings

    /// <summary>
	/// ThroughputChart class
	/// </summary>
	public class ThroughputChart
	{
		/// <summary>
		/// data To Send
		/// </summary>
		private PriceData dataToSend = new PriceData();

		/// <summary>
		/// calculation Run
		/// </summary>
		private bool calculationRun = false;

		/// <summary>
		/// train Resin Amount
		/// </summary>
		private double trainResinAmount;

		/// <summary>
		/// selected Train GPM
		/// </summary>
		private double selectedTrainGPM;

		/// <summary>
		/// current Salt Split Value
		/// </summary>
		private double currentSaltSplitValue;

		/// <summary>
		/// with Cleaning
		/// </summary>
		private bool withCleaning = false;

		/// <summary>
		/// with And Without Cleaning
		/// </summary>
		private bool withAndWithoutCleaning = true;

		/// <summary>
		/// train Gpm value
		/// </summary>
		private double[] trainGPM;

		/// <summary>
		/// train GPM Values
		/// </summary>
		public string[] trainGPMValues;

		/// <summary>
		/// Run Model which calculates the required computations
		/// </summary>
		/// <param name="startingSaltSplit">starting Salt Split</param>
		/// <param name="currentSaltSplit">current Salt Split</param>
		/// <param name="resinLifeExpectancy">resin Life Expectancy</param>
		/// <param name="simulationConfidence">simulation Confidence</param>
		/// <param name="numberSimulationIterations">number Simulation Iterations</param>
		/// <param name="simulationMethod">simulation Method</param>
		/// <param name="standardDeviationThreshold">standard Deviation Threshold</param>
		/// <param name="resinAge">resin Age parameter</param>
		/// <param name="replacementLevel">replacement Level</param>
		/// <param name="rtiCleaningLevel">rti Cleaning Level</param>
		/// <param name="donotReplaceResin">dont Replace Resin</param>
		/// <returns>Returns the Price data</returns>
		public PriceData RunModel(double startingSaltSplit, double currentSaltSplit, double resinLifeExpectancy, int simulationConfidence, int numberSimulationIterations, string simulationMethod, int standardDeviationThreshold, double resinAge, double replacementLevel, double rtiCleaningLevel, bool donotReplaceResin)
		{
			try
			{
				var trainList = dataToSend.TrainList;
				var numberOfTrains = dataToSend.NumberTrains;
				if (calculationRun)
				{
					dataToSend = new PriceData();
				}
				dataToSend.TrainList = trainList;
				dataToSend.NumberTrains = numberOfTrains;
				double numberOfWeeks = Convert.ToDouble(resinLifeExpectancy);
				var standardDevData = ProcessData.WeeklyWeightedAverageCondstdDev;
				RandomNumberSimulation conductivity = new RandomNumberSimulation();
				double roundedAgeValues = Convert.ToDouble(string.Format("{0:0.00}", resinAge));
				Dictionary<int, double> grainForeCast = conductivity.BeginCalculation(standardDevData, simulationConfidence, numberSimulationIterations, simulationMethod, standardDeviationThreshold, roundedAgeValues);
				int grainCount = grainForeCast.Count;
				int numberOfLoops = Convert.ToInt16(resinLifeExpectancy);
				if (numberOfLoops >= grainCount)
				{
					int weeksToAdd = Convert.ToInt32(numberOfLoops - grainCount);
					int lastGrainWeek = 0;
					if (grainForeCast != null && grainForeCast.Count > 0)
					{
						lastGrainWeek = grainForeCast.Last().Key + 1;
					}
					int grainIndx = 0;
					for (int loop = 0; loop <= weeksToAdd; loop++)
					{
						double grainToAdd = 0;
						if (grainForeCast != null && grainForeCast.Count > 0)
						{
							grainToAdd = grainForeCast.ElementAt(grainIndx).Value;
							grainForeCast.Add(lastGrainWeek, grainToAdd);
							lastGrainWeek++;
							grainIndx++;
						}
					}
				}
				dataToSend.GrainForcast = grainForeCast;
				double rtiCleaningeffectivness = 28.0;
				ThroughputBuilder tp = new ThroughputBuilder();
				Dictionary<DateTime, Tuple<int, double, string>> throughputCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, true, donotReplaceResin);
				var afterConditions = tp.AfterSystemConditions;
				Dictionary<DateTime, Tuple<int, double, string>> throughputNoCleanPrediction = tp.ThroughputBuild(replacementLevel, rtiCleaningLevel, currentSaltSplit, grainForeCast, trainResinAmount, rtiCleaningeffectivness, selectedTrainGPM, resinAge, startingSaltSplit, numberOfWeeks, false, donotReplaceResin);
				var beforeConditions = tp.BeforeSystemConditions;
				List<double> reginWeek = new List<double>();
				List<double> hoursWeek = new List<double>();
				List<double> reginTime = new List<double>();
				List<double> tpWeek = new List<double>();
				List<double> ssWeek = new List<double>();
				List<double> beforeSaltSplit = new List<double>();
				List<double> afterSaltSplit = new List<double>();
				List<int> weeks = new List<int>();
				List<Tuple<int, double>> regweekToSendNormal = new List<Tuple<int, double>>();
				List<Tuple<int, double>> regweekToSendClean = new List<Tuple<int, double>>();
				int weekNumber = 0;
				foreach (var week in beforeConditions)
				{
					weekNumber++;
					weeks.Add(weekNumber);
					reginWeek.Add(week.Item2);
					hoursWeek.Add(week.Item3);
					reginTime.Add(week.Item4);
					tpWeek.Add(week.Item5);
					ssWeek.Add(week.Item6);
					beforeSaltSplit.Add(week.Item6);
					Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
					regweekToSendNormal.Add(reginsPerWeeK);
				}
				dataToSend.RegensPerWeekNormalOps = regweekToSendNormal;
				dataToSend.CleanThroughput = throughputCleanPrediction;
				dataToSend.NormalOpsThroughput = throughputNoCleanPrediction;
				double averageTPBefore = 0;
				if (tpWeek != null && tpWeek.Count > 0)
				{
					averageTPBefore = Math.Round(tpWeek.Average(), 0);
				}
				if (reginWeek != null && reginWeek.Count > 0)
				{
					dataToSend.RegensPerWeekAverageBefore = Math.Round(reginWeek.Average(), 2);
				}
				if (hoursWeek != null && hoursWeek.Count > 0)
				{
					dataToSend.HoursPerRunAverageBefore = Math.Round(hoursWeek.Average(), 2);
				}
				if (reginTime != null && reginTime.Count > 0)
				{
					dataToSend.RegenTimeAverageBefore = Math.Round(reginTime.Average(), 2);
				}
				dataToSend.ThroughputAverageBefore = averageTPBefore;
				if (ssWeek != null && ssWeek.Count > 0)
				{
					dataToSend.NumberRegens = Math.Round(ssWeek.Average());
				}
				if (withCleaning || withAndWithoutCleaning)
				{
					reginWeek.Clear();
					hoursWeek.Clear();
					reginTime.Clear();
					tpWeek.Clear();
					ssWeek.Clear();
					foreach (var week in afterConditions)
					{
						reginWeek.Add(week.Item2);
						hoursWeek.Add(week.Item3);
						reginTime.Add(week.Item4);
						tpWeek.Add(week.Item5);
						ssWeek.Add(week.Item6);
					}
					if (reginWeek != null && reginWeek.Count > 0)
					{
						dataToSend.RegensPerWeekAverageAfter = Math.Round(reginWeek.Average(), 2);
					}
					if (hoursWeek != null && hoursWeek.Count > 0)
					{
						dataToSend.HoursPerRunAverageAfter = Math.Round(hoursWeek.Average(), 2);
					}
					if (reginTime != null && reginTime.Count > 0)
					{
						dataToSend.RegenTimeAverageAfter = Math.Round(reginTime.Average(), 2);
					}
					if (tpWeek != null && tpWeek.Count > 0)
					{
						dataToSend.ThroughputAverageAfter = Math.Round(tpWeek.Average(), 0);
					}
				}
				weekNumber = 0;
				foreach (var week in afterConditions)
				{
					weekNumber++;
					Tuple<int, double> reginsPerWeeK = new Tuple<int, double>(weekNumber, week.Item2);
					afterSaltSplit.Add(week.Item6);
					regweekToSendClean.Add(reginsPerWeeK);
				}
				Dictionary<int, Tuple<double?, double?>> saltSplitData = new Dictionary<int, Tuple<double?, double?>>();
				int length;
				if (beforeSaltSplit.Count < afterSaltSplit.Count)
				{
					length = afterSaltSplit.Count;
				}
				else
				{
					length = beforeSaltSplit.Count;
				}
				for (int i = 0; i < length; i++)
				{
					int aftercount = afterSaltSplit.Count;
					int beforecount = beforeSaltSplit.Count;
					double? before = 0;
					double? after = 0;
					int week = 0;
					bool shouldAdd = true;

					if (beforecount > aftercount)
					{
						if (i < aftercount)
						{
							before = beforeSaltSplit.ElementAt(i);
							after = afterSaltSplit.ElementAt(i);
							week = i + 1;
						}
						else
						{
							shouldAdd = false;
						}
					}
					else if (beforecount < aftercount)
					{
						if (i < beforecount)
						{
							before = beforeSaltSplit.ElementAt(i);
							after = afterSaltSplit.ElementAt(i);
							week = i + 1;
						}
						else
						{
							shouldAdd = false;
						}
					}
					else if (beforecount == aftercount)
					{
						before = beforeSaltSplit.ElementAt(i);
						after = afterSaltSplit.ElementAt(i);
						week = i + 1;
					}
					Tuple<double?, double?> temp = new Tuple<double?, double?>(before, after);
					if (shouldAdd)
					{
						saltSplitData.Add(week, temp);
					}
				}
				dataToSend.RegensPerWeekClean = regweekToSendClean;
				dataToSend.CleanThroughput = throughputCleanPrediction;
				dataToSend.NormalOpsThroughput = throughputNoCleanPrediction;
				dataToSend.SaltSplit = saltSplitData;
				dataToSend.NumberWeeks = numberOfWeeks;
				return dataToSend;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Loads the Train Data
		/// </summary>
		/// <param name="numberTrains">number of Trains</param>
		/// <param name="dbTrainGpm">db Train Gpm</param>
		public void LoadTrainData(int numberTrains, string[] dbTrainGpm)
		{
			try
			{
				int size = dbTrainGpm.Count();
				trainGPM = new double[size];
				for (int i = 0; i < size; i++)
				{
					trainGPM[i] = Convert.ToDouble(dbTrainGpm[i]);
				}
				DataTable trainList = new DataTable();
				trainList.Columns.Add("Train#", typeof(long));
				trainList.Columns.Add("Name", typeof(string));
				for (int max = 0; max < numberTrains; max++)
				{
					DataRow row = trainList.NewRow();
					string trainName;
					if (max != 0)
					{
						trainName = "Train# " + max.ToString();
					}
					else
					{
						trainName = "All Trains (System)";
					}
					row["Train#"] = max;
					row["Name"] = trainName;
					trainList.Rows.Add(row);
				}
				dataToSend.TrainList = trainList;
				dataToSend.NumberTrains = numberTrains;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Loads the Model
		/// </summary>
		/// <param name="customerId">customer Identifier</param>
		/// <param name="currentSaltSplit">current Salt Split</param>
		/// <param name="trainRepository">train Repository</param>
		/// <param name="predictiveRepository">predictive Repository</param>
		/// <param name="conductivityS1">conductivity S1</param>
		/// <param name="conductivityS2">conductivity S2</param>
		/// <param name="sourceOnePercent">source One Percent</param>
		/// <param name="startingSaltSplit">starting Salt Split</param>
		/// <param name="resinLifeExpectancy">resin Life Expectancy</param>
		/// <param name="simulationConfidence">simulation Confidence</param>
		/// <param name="numberSimulationIterations">number of Simulation Iterations</param>
		/// <param name="simulationMethod">simulation Method</param>
		/// <param name="standardDevThreshold">standardDev Threshold</param>
		/// <param name="resinAge">resin Age parameter</param>
		/// <param name="replacementLevel">replacement Level</param>
		/// <param name="rtiCleaningLevel">rti Cleaning Level</param>
		/// <param name="selectedTrain">selected Train</param>
		/// <param name="donotReplaceResin">donot Replace Resin</param>
		/// <returns>Returns the price data</returns>
		public PriceData LoadModel(string customerId, double currentSaltSplit, IRepository<train> trainRepository, IPredictiveModelRepository predictiveRepository, List<string>[] conductivityS1, List<string>[] conductivityS2, double sourceOnePercent, double startingSaltSplit, double resinLifeExpectancy, int simulationConfidence, int numberSimulationIterations, string simulationMethod, int standardDevThreshold, double resinAge, double replacementLevel, double rtiCleaningLevel, string selectedTrain, bool donotReplaceResin)
		{
			try
			{
				Dictionary<string, string> vessel = new Dictionary<string, string>();
				Dictionary<int, Dictionary<string, string>> allVesselData = new Dictionary<int, Dictionary<string, string>>();
				ProcessData process = new ProcessData();
				process.Compute(conductivityS1, conductivityS2, sourceOnePercent);
				currentSaltSplitValue = currentSaltSplit;
				List<vessel> lstCustomerVessels = new List<vessel>();
				lstCustomerVessels = predictiveRepository.GetCustomerVessels(Convert.ToInt64(customerId));
				List<TimeSpan> intervalList = new List<TimeSpan>();
				List<double> vesselSizeList = new List<double>();
				List<double> lbsChemicalList = new List<double>();
				TimeSpan intervalSum = new TimeSpan();
				List<double> replacementPlan = new List<double>();
				int selectedTrainId;
				if (selectedTrain == "0")
				{
					selectedTrainId = 0;
				}
				else
				{
					selectedTrainId = Convert.ToInt16(selectedTrain);
					lstCustomerVessels = lstCustomerVessels.Where(item => selectedTrain == Convert.ToString(item.train_trainID)).ToList();
				}
				if (lstCustomerVessels != null)
				{
					foreach (var vesselList in lstCustomerVessels)
					{
						int trainID = Convert.ToInt32(vesselList.train_trainID);
						if (trainID == selectedTrainId)
						{
							DateTime purchase_date = Convert.ToDateTime(vesselList.date_replaced, new CultureInfo("en-US", true));
							TimeSpan interval = DateTime.Today - purchase_date;
							intervalList.Add(interval);
							replacementPlan.Add(Convert.ToInt32(vesselList.replacement_plan));
							double lbsChemical = Convert.ToDouble(vesselList.lbs_chemical);
							double vesselSize = Convert.ToDouble(vesselList.size);
							vesselSizeList.Add(vesselSize);
							lbsChemicalList.Add(lbsChemical);
						}
						if (selectedTrainId == 0)
						{
							DateTime purchase_date = Convert.ToDateTime(vesselList.date_replaced, new CultureInfo("en-US", true));
							TimeSpan interval = DateTime.Today - purchase_date;
							intervalList.Add(interval);
							replacementPlan.Add(Convert.ToInt32(vesselList.replacement_plan));
							double lbsChemical = Convert.ToDouble(vesselList.lbs_chemical);
							double vesselSize = Convert.ToDouble(vesselList.size);
							vesselSizeList.Add(vesselSize);
							lbsChemicalList.Add(lbsChemical);
						}
					}
				}
				foreach (var span in intervalList)
				{
					intervalSum += span;
				}
				double average = intervalSum.TotalMilliseconds / intervalList.Count;
				TimeSpan averageResinAge = new TimeSpan();
				if (!double.IsNaN(average))
				{
					averageResinAge = TimeSpan.FromMilliseconds(average);
				}
				double age = (averageResinAge.TotalDays) / 7;
				double size = 0.0;
				if (vesselSizeList != null && vesselSizeList.Count > 0)
				{
					size = vesselSizeList.Sum();
				}

				trainResinAmount = size;
				bool cation = true;
				foreach (var vesselSize in vesselSizeList)
				{
					if (cation)
					{
						dataToSend.AmountCation = vesselSize;
					}
					else
					{
						dataToSend.AmountAnion = vesselSize;
					}
					cation = !cation;
				}
				cation = true;

				foreach (var chemAmt in lbsChemicalList)
				{
					if (cation)
					{
						dataToSend.AcidUsage = chemAmt;
					}
					else
					{
						dataToSend.CausticUsage = chemAmt;
					}
					cation = !cation;
				}
				int customerCustomerId = Convert.ToInt16(customerId);
				List<train> trains = trainRepository.GetAll().Where(p => p.customer_customerID == customerCustomerId).ToList();
				trainGPMValues = new string[trains.Count];
				trainGPM = new double[trains.Count];
				for (int i = 0; i < trains.Count; i++)
				{
					trainGPMValues[i] = trains[i].gpm;
					trainGPM[i] = Convert.ToDouble(trains[i].gpm);
				}

				if (selectedTrainId == 0 && trainGPM != null && trainGPM.Length > 0)
				{
					selectedTrainGPM = trainGPM.Average();
				}
				else if (trainGPM != null && trainGPM.Length > 0)
				{
					selectedTrainGPM = trainGPM[0];
				}

				this.LoadTrainData(2, trainGPMValues);
				PriceData priceData = this.RunModel(startingSaltSplit, currentSaltSplit, resinLifeExpectancy, simulationConfidence, numberSimulationIterations, simulationMethod, standardDevThreshold, resinAge, replacementLevel, rtiCleaningLevel, donotReplaceResin);
				return priceData;
			}
			catch
			{
				throw;
			}
		}

	}
}
