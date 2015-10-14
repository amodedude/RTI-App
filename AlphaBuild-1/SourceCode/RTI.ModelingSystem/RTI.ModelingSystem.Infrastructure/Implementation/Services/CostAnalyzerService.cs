// -----------------------------------------------------------------------
// <copyright file="CostAnalyzerService.cs" company="RTI">
// RTI
// </copyright>
// <summary>Cost Analyzer Service</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Services
{
	#region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Interfaces.Services;
    using RTI.ModelingSystem.Core.Models;

	#endregion Usings

	/// <summary>
	/// Cost Analyzer Service
	/// </summary>
	public class CostAnalyzerService : ICostAnalyzerService
	{
		#region Properties

		/// <summary>
		/// modified Customer Repository
		/// </summary>
		private ICustomerRepository modifiedCustRepository;

		/// <summary>
		/// modified Train Repository
		/// </summary>
		private ITrainRepository modifiedTrainRepository;

		/// <summary>
		/// calculation Parameters
		/// </summary>
		private static Calculation_Paramerters calculationParameters = new Calculation_Paramerters();

		/// <summary>
		/// cumulative Savings
		/// </summary>
		private static string cumulativeSavings;

		/// <summary>
		/// roi Global value
		/// </summary>
		private static string roiGlobal;

		/// <summary>
		/// cost Analyzer Result Data
		/// </summary>
		private static CostAnalyzerResult costAnalyzerResultData = new CostAnalyzerResult();

		#endregion Properties

		#region Constructor

		/// <summary>
		/// CostAnalyzerService constructor
		/// </summary>
		/// <param name="customerRepository">modified Cust Repository</param>
		/// <param name="trainRepository">modified Train Repository</param>
		public CostAnalyzerService(ICustomerRepository customerRepository, ITrainRepository trainRepository)
		{
			this.modifiedCustRepository = customerRepository;
			this.modifiedTrainRepository = trainRepository;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Gets the settings for CostAnalyzer chart
		/// </summary>
		/// <param name="customerId">customer Identifier</param>
		/// <param name="selectedTrain">selected Train</param>
		/// <returns>Returns the cost settings</returns>
		public CostSettings GetCostSettings(string customerId, string selectedTrain)
		{
			CostSettings settings = new CostSettings();
			try
			{
				if (int.Parse(selectedTrain) == 0)
				{
					settings.SelectedTrain = "All Trains";
				}
				else
				{
					train train = modifiedTrainRepository.FindById(selectedTrain);
					settings.SelectedTrain = train.number.ToString();
				}
				customer customer = this.modifiedCustRepository.FindById(customerId);
				settings.AcidPrice = customer.acid_price;
				settings.CausticPrice = customer.caustic_price;
			}
			catch (Exception)
			{
				throw;
			}
			return settings;
		}

		/// <summary>
		/// Depicts the Cost analyzer chart
		/// </summary>
		/// <param name="dataToSend">data To Send</param>
		/// <param name="currentTrain">current Train</param>
		/// <param name="customerId">customer Identifier</param>
		/// <returns>Returns the cost</returns>
		public List<Tuple<int, double, double>> OpenCostWindow(PriceData dataToSend, int currentTrain, string customerId)
		{
			try
			{
				var numTrains = dataToSend.NumberTrains;
				var numRegensNormOps = dataToSend.RegensPerWeekNormalOps;
				var numRegensClean = dataToSend.RegensPerWeekClean;
				var cleanTP = dataToSend.CleanThroughput;
				var normOpsTP = dataToSend.NormalOpsThroughput;
				var trainList = dataToSend.TrainList;
				var grains = dataToSend.GrainForcast;
				var saltSplit = dataToSend.SaltSplit;
				var amtCation = dataToSend.AmountCation;
				var amtAnion = dataToSend.AmountAnion;
				var lbsCaustic = dataToSend.CausticUsage;
				var lbsAcid = dataToSend.AcidUsage;
				List<Tuple<int, double, double>> data = CalculatorInitializer(numTrains, numRegensNormOps, numRegensClean, cleanTP, normOpsTP, trainList, currentTrain, true, saltSplit, grains, amtCation, amtAnion, lbsCaustic, lbsAcid, customerId);
				return data;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Initialize and Run Calculations for cost analyzer chart
		/// </summary> 
		public List<Tuple<int, double, double>> CalculatorInitializer(int numberOfTrains, List<Tuple<int, double>> numberRegensNormOps, List<Tuple<int, double>> numberRegensClean, Dictionary<DateTime, Tuple<int, double, string>> throughputClean, Dictionary<DateTime, Tuple<int, double, string>> throughpuReplace, System.Data.DataTable trainList, int currentTrain, bool isFirstLoad, Dictionary<int, Tuple<double?, double?>> saltSplitData, Dictionary<int, double> grains, double amountCation, double amountAnion, double lbsCaustic, double lbsAcid, string customerId)
		{
			try
			{
				// Set variables in variable class for acess by any method            
				calculationParameters.WithCleaningTP = throughputClean;
				calculationParameters.WithOutCleaningTP = throughpuReplace;
				calculationParameters.TrainList = trainList;
				calculationParameters.NumberRegenerationsNormalOps = numberRegensNormOps;
				calculationParameters.NumberRegenerationsClean = numberRegensClean;
				calculationParameters.NumberOfTrains = numberOfTrains;
				calculationParameters.SaltSplit = saltSplitData;
				calculationParameters.GrainForcast = grains;

				// Load the appropriate number of trains
                if (isFirstLoad)
                {
                    calculationParameters.AcidConcentration = 100;
                    calculationParameters.CausticConcentration = 100;
                    customer customer = this.modifiedCustRepository.FindById(customerId);
                    calculationParameters.AcidPrice = double.Parse(customer.acid_price.ToString());
                    calculationParameters.CausticPrice = double.Parse(customer.caustic_price.ToString());
                    calculationParameters.AcidPriceConversion = 1;
                    calculationParameters.CausticPriceConversion = 1;
                    calculationParameters.CationCleaningPrice = 32;
                    calculationParameters.AnionCleaningPrice = 52;
                    calculationParameters.ReplacementPriceAnion = 90;
                    calculationParameters.ReplacemtntPriceCation = 70;
                    calculationParameters.AnionAmount = amountAnion;
                    calculationParameters.CationAmount = amountCation;
                    calculationParameters.AmountOfAcid = lbsAcid;
                    calculationParameters.AmountOfCaustic = lbsCaustic;

                }

				// Run the calculation
				List<Price_Data> price = PriceCalculator(numberOfTrains, numberRegensNormOps, numberRegensClean, throughputClean, throughpuReplace, trainList);
				calculationParameters.PriceData = price;

				// Add the data into a list for plotting 
				List<Tuple<int, double, double>> data = new List<Tuple<int, double, double>>();
				foreach (var week in price)
				{

					Tuple<int, double, double> weekData = new Tuple<int, double, double>(week.WeekNumber, week.BeforePrice, week.AfterPrice);
					data.Add(weekData);
				}

				return data;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Run Calculations for cost analyzer chart
		/// </summary>
		public List<Price_Data> PriceCalculator(int numberOfTrains, List<Tuple<int, double>> numberRegensNormOps, List<Tuple<int, double>> numberRegensClean, Dictionary<DateTime, Tuple<int, double, string>> throughputClean, Dictionary<DateTime, Tuple<int, double, string>> throughputNormOps, System.Data.DataTable trainList)
		{
			try
			{
				// Load calculation parameters
				double causticPrice = calculationParameters.CausticPrice;
				double acidPrice = calculationParameters.AcidPrice;
				double percentCaustic = calculationParameters.CausticConcentration * 0.01;
				double percentAcid = calculationParameters.AcidConcentration * 0.01;
				double causticUsed = calculationParameters.AmountOfCaustic;
				double acidUsed = calculationParameters.AmountOfAcid;
				double acidPriceFactor = calculationParameters.AcidPriceConversion;
				double causticPriceFactor = calculationParameters.CausticPriceConversion;
				double amountAnion = calculationParameters.AnionAmount;
				double amountCation = calculationParameters.CationAmount;

				// Account for dividing by zero for chemical concentration by making value arbitrarily small
				if (percentCaustic == 0)
				{
					percentCaustic = 0.0001;
				}
				if (percentAcid == 0)
				{
					percentAcid = 0.0001;
				}
				// Variable to hold entire price timeseries data
				List<Price_Data> price = new List<Price_Data>();

				// Holds the cost to regenerate data
				List<double> regenerationCostBefore = new List<double>();
				List<double> regenerationCostAfter = new List<double>();
				List<double> weeklyRegenerationsNormOps = new List<double>();
				List<double> weeklyRegenerationsClean = new List<double>();
				List<double> cleaningCostBefore = new List<double>();
				List<double> replacementCostBefore = new List<double>();
				List<double> cleaningCostAfter = new List<double>();
				List<double> replacementCostAfter = new List<double>();
				List<double> opsCostBefore = new List<double>();
				List<double> opsCostAfter = new List<double>();

				// Calculate the per regeneration before price
				int wk = 0;
				foreach (var week in numberRegensNormOps)
				{
					Price_Data eachPrice_before = new Price_Data();
					double costToRegenerate = (((((causticPrice * causticPriceFactor) / percentCaustic) * (causticUsed)) * amountAnion) + ((((acidPrice * acidPriceFactor) / percentAcid) * (acidUsed)) * amountCation)) * week.Item2;
					regenerationCostBefore.Add(costToRegenerate);
					// Check if the resin was either cleaned or replaced
					string cleanOrReplace;
					if (wk <= throughputNormOps.Count - 1)
					{
						cleanOrReplace = throughputNormOps.ElementAt(wk).Value.Item3;
						if (cleanOrReplace == "Replace")
						{
							double replacmentCost = (calculationParameters.ReplacementPriceAnion + calculationParameters.ReplacemtntPriceCation) * (calculationParameters.AnionAmount + calculationParameters.CationAmount);
							costToRegenerate = costToRegenerate + replacmentCost;
							replacementCostBefore.Add(replacmentCost);
						}
						else if (cleanOrReplace == "Clean")
						{
							double cleaningCost = ((calculationParameters.CationCleaningPrice - (calculationParameters.CationCleaningPrice * calculationParameters.CationDiscount)) + (calculationParameters.AnionCleaningPrice - (calculationParameters.AnionCleaningPrice * calculationParameters.AnionDiscount))) * (calculationParameters.AnionAmount + calculationParameters.CationAmount);
							costToRegenerate = costToRegenerate + cleaningCost;
							cleaningCostBefore.Add(cleaningCost);
						}
						else
						{
							replacementCostBefore.Add(0);
							cleaningCostBefore.Add(0);
						}
					}
					opsCostBefore.Add(costToRegenerate);
					eachPrice_before.BeforePrice = costToRegenerate;
					eachPrice_before.WeekNumber = week.Item1;
					price.Add(eachPrice_before);
					weeklyRegenerationsNormOps.Add(week.Item2);

					wk++;
				}

				int count = 0;
				wk = 0;
				// Calculate the per regeneration for the after price
				foreach (var week in numberRegensClean)
				{
					Price_Data eachPrice_after = new Price_Data();
					double costToRegenerate = (((((causticPrice * causticPriceFactor) / percentCaustic) * (causticUsed)) * amountAnion) + ((((acidPrice * acidPriceFactor) / percentAcid) * (acidUsed)) * amountCation)) * week.Item2;
					regenerationCostAfter.Add(costToRegenerate);
					// Check if the resin was either cleaned or replaced
					if (wk <= throughputClean.Count - 1)
					{
						string cleanOrReplace = throughputClean.ElementAt(wk).Value.Item3;
						if (cleanOrReplace == "Replace")
						{
							double replacmentCost = (calculationParameters.ReplacementPriceAnion + calculationParameters.ReplacemtntPriceCation) * (calculationParameters.AnionAmount + calculationParameters.CationAmount);
							costToRegenerate = costToRegenerate + replacmentCost;
							replacementCostAfter.Add(replacmentCost);
						}
						else if (cleanOrReplace == "Clean")
						{
							double cleaningCost = ((calculationParameters.CationCleaningPrice - (calculationParameters.CationCleaningPrice * calculationParameters.CationDiscount)) + (calculationParameters.AnionCleaningPrice - (calculationParameters.AnionCleaningPrice * calculationParameters.AnionDiscount))) * (calculationParameters.AnionAmount + calculationParameters.CationAmount);
							costToRegenerate = costToRegenerate + cleaningCost;
							cleaningCostAfter.Add(cleaningCost);
						}
						else
						{
							replacementCostAfter.Add(0);
							cleaningCostAfter.Add(0);
						}

						wk++;
					}
					opsCostAfter.Add(costToRegenerate);
					eachPrice_after.AfterPrice = costToRegenerate;
					eachPrice_after.WeekNumber = week.Item1;
					price.ElementAt(count).AfterPrice = eachPrice_after.AfterPrice;
					count++;
				}

				// Average the cost to regenerate and update the display
				double regenCostBefore = regenerationCostBefore.Average();
				double regenCostAfter = regenerationCostAfter.Average();

				costAnalyzerResultData.RegenWeeklyCostAfter = "$" + string.Format("{0:n}", Math.Round(regenCostAfter, 2));

				// Average the cost to clean or the cost to replace and update display
				double cleanCostBefore = cleaningCostBefore.Sum();
				double cleanCostAfter = cleaningCostAfter.Sum();
				double replaceCostBefore = replacementCostBefore.Sum();
				double replaceCostAfter = replacementCostAfter.Sum();
				costAnalyzerResultData.CleaningCostBefore = "$" + string.Format("{0:n}", Math.Round(cleanCostBefore));
				costAnalyzerResultData.CleaningCostAfter = "$" + string.Format("{0:n}", Math.Round(cleanCostAfter));
				costAnalyzerResultData.ReplacementCostBefore = "$" + string.Format("{0:n}", Math.Round(replaceCostBefore));
				costAnalyzerResultData.ReplacementCostAfter = "$" + string.Format("{0:n}", Math.Round(replaceCostAfter));

				// Sum the operations cost to calculate total cost for given duration
				double absoluteTotalCostBefore = opsCostBefore.Sum();
				double absoluteTotalCostAfter = opsCostAfter.Sum();
				double averageOpsBefore = opsCostBefore.Average();
				double averageOpsAfter = opsCostAfter.Average();
				double regenSavings = (averageOpsBefore - averageOpsAfter) * throughputClean.Count;
				double cumSavings = (absoluteTotalCostBefore - absoluteTotalCostAfter) + regenSavings;
				double cumInvestment = cleanCostAfter;
				costAnalyzerResultData.TotalOpsCostBefore = "$" + string.Format("{0:n0}", Math.Round(absoluteTotalCostBefore, 0));
				costAnalyzerResultData.TotalOpsCostAfter = "$" + string.Format("{0:n0}", Math.Round(absoluteTotalCostAfter, 0));
				costAnalyzerResultData.TotalWeeklyCostBefore = "$" + string.Format("{0:n}", Math.Round(averageOpsBefore, 2));
				costAnalyzerResultData.TotalWeeklyCostAfter = "$" + string.Format("{0:n}", Math.Round(averageOpsAfter, 2));
				cumulativeSavings = "$" + string.Format("{0:n}", Math.Round(cumSavings, 2));
				
				// Calculate the average cost per gallon of water produced
				double totalOpsAverageBefore = opsCostBefore.Average();
				double totalOpsAverageAfter = opsCostAfter.Average();
				double beforeTPAverage = throughputNormOps.Average(x => x.Value.Item2);
				double afterTPAverage = throughputClean.Average(x => x.Value.Item2);
				double costPerGAL_Before = totalOpsAverageBefore / (beforeTPAverage / 1000);
				double costPerGAL_After = totalOpsAverageAfter / (afterTPAverage / 1000);
				costAnalyzerResultData.AvgCostPerGalBefore = "$" + string.Format("{0:n}", Math.Round(costPerGAL_Before, 2));
				costAnalyzerResultData.AvgCostPerGalAfter = "$" + string.Format("{0:n}", Math.Round(costPerGAL_After, 2));

				// Calculate the average amount of time before the customer breaks even from cleanings
				double averageCleaningCost;
                if (cleaningCostAfter != null && cleaningCostAfter.Count > 0 && cleaningCostAfter.Max() != 0)
                {
                    averageCleaningCost = cleaningCostAfter.Where(x => x != 0).Average();
                }
                else
                {
                    averageCleaningCost = 0;
                }

				double? weeksToRecoupe = WeeksUntillBreakEven(averageOpsBefore, averageOpsAfter, averageCleaningCost);

				double averageReplacingCost;
                if (replacementCostBefore.Max() != 0)
                {
                    averageReplacingCost = replacementCostBefore.Where(x => x != 0).Average();
                }
                else
                {
                    averageReplacingCost = 0;
                }
				weeksToRecoupe = WeeksUntillBreakEven(averageOpsBefore, averageOpsAfter, averageReplacingCost);
				double ROI = Math.Round((((cumSavings) - cleanCostAfter) / cleanCostAfter) * 100);
				roiGlobal = string.Format("{0:n0}", ROI) + "%";
				return price;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Calculate the average number of weeks until investment is recouped
		/// </summary>
		public double? WeeksUntillBreakEven(double weeklyBeforeCost, double weeklyAfterCost, double investmentCost)
		{
			try
			{
				double? weeksUntillRecoupe;

				// Determine how long it takes to break even 
				if (weeklyBeforeCost < weeklyAfterCost)
				{
					weeksUntillRecoupe = null;
				}
				else
				{
					bool amIBrokenEven = false;
					int numberOfWeeks = 0;
					List<double> weeklySavings = new List<double>();
					while (amIBrokenEven == false)
					{
						double savings = weeklyBeforeCost - weeklyAfterCost;
						weeklySavings.Add(savings);
						double totalSavings = weeklySavings.Sum();
						if (totalSavings >= investmentCost)
						{
							amIBrokenEven = true;
						}
						numberOfWeeks++;
					}
					weeksUntillRecoupe = numberOfWeeks;
				}
				return weeksUntillRecoupe;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Cumulative Savings
		/// </summary>
		/// <returns>Returns the Cumulative Savings</returns>
		public List<string> GetCumulativeSavings()
		{
			try
			{
				List<string> CumulativeSavingsData = new List<string>();
				CumulativeSavingsData.Add(cumulativeSavings);
				CumulativeSavingsData.Add(roiGlobal);

				return CumulativeSavingsData;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// selected Week Data Finder
		/// </summary>
		/// <param name="week">week parameter</param>
		/// <returns>Returns the week data</returns>
		public double?[] SelectedWeekDataFinder(double week)
		{
			try
			{
				var saltSplit = calculationParameters.SaltSplit;
				var throughputClean = calculationParameters.WithCleaningTP;
				var throughputNoClean = calculationParameters.WithOutCleaningTP;
				var grains = calculationParameters.GrainForcast;
				double?[] data = new double?[6];

				// Search for nearest SaltSPlit datapoint
				// Search for nearest Throughput datapoint
				double? _saltSplitBefore = null;
				double? _saltSplitAfter = null;
				if (saltSplit != null)
				{
					foreach (var entry in saltSplit)
					{
						int SS = entry.Key;
						if (SS == week)
						{
                            if (entry.Value.Item1 != null)
                            {
                                double value = Convert.ToDouble(entry.Value.Item1);
                                _saltSplitBefore = Math.Round(value, 2);
                            }
                            else
                            {
                                _saltSplitBefore = null;
                            }

                            if (entry.Value.Item2 != null)
                            {
                                double value = Convert.ToDouble(entry.Value.Item2);
                                _saltSplitAfter = Math.Round(value, 2);
                            }
                            else
                            {
                                _saltSplitAfter = null;
                            }

							break;
						}
					}
				}
				// Search for nearest Throughput datapoint
				double? _cleanTP = null;
				if (throughputClean != null)
				{
					foreach (var entry in throughputClean)
					{
						int TPweek = entry.Value.Item1;
						if (TPweek == week)
						{
							_cleanTP = Math.Round(entry.Value.Item2, 2);
							break;
						}
					}
				}
				// Search for nearest Throughput datapoint
				double? _noCleanTP = null;
				if (throughputNoClean != null)
				{
					foreach (var entry in throughputNoClean)
					{
						int TPweek = entry.Value.Item1;
						if (TPweek == week)
						{
							_noCleanTP = Math.Round(entry.Value.Item2, 2);
							break;
						}
					}
				}

				// Search for nearest grain datapoint
				double? _conductivity = null;
				if (grains != null)
				{
					foreach (var entry in grains)
					{
						int GRweek = entry.Key;
						if (GRweek == week)
						{
							_conductivity = Math.Round((entry.Value) * (17 / 0.7), 2);
							break;
						}
					}
				}
				// Find the median conductivity
				var max = grains != null ? grains.Max(s => s.Value) : 0;
				var min = grains != null ? grains.Min(s => s.Value) : 0;
				double cond_median = ((max - min) / 2) * (17 / 0.7);

				data[0] = _conductivity;
				data[1] = _saltSplitBefore;
				data[2] = _saltSplitAfter;
				data[3] = _cleanTP;
				data[4] = _noCleanTP;
				data[5] = cond_median;

				return data;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Cost Analyzer Results Data
		/// </summary>
		/// <returns>Returns the Cost Analyzer Result</returns>
		public CostAnalyzerResult GetCostAnalyzerResultsData()
		{
			return costAnalyzerResultData;
		}
		#endregion Methods
	}
}
