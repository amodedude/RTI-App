// -----------------------------------------------------------------------
// <copyright file="ICostAnalyzerService.cs" company="RTI">
// RTI
// </copyright>
// <summary>ICostAnalyzer Service</summary>
// -----------------------------------------------------------------------

using RTI.ModelingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.ModelingSystem.Core.Interfaces.Services
{
	public interface ICostAnalyzerService
	{
		/// <summary>
		/// Gets the cost settings.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <param name="selectedTrain">The selected train.</param>
		/// <returns>Returns CostSettings</returns>
		CostSettings GetCostSettings(string customerId, string selectedTrain);

		/// <summary>
		/// Opens the cost window.
		/// </summary>
		/// <param name="dataToSend">The data_ to send.</param>
		/// <param name="currentTrain">The current train.</param>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns Open cost</returns>
        List<Tuple<int, double, double>> OpenCostWindow(PriceData dataToSend, int currentTrain, string customerId, bool isFirstLoad);

		/// <summary>
		/// Calculator_s the initializer.
		/// </summary>
		/// <param name="numberOfTrains">The number of trains.</param>
		/// <param name="numberRegensNormOps">The number regens norm ops.</param>
		/// <param name="numberRegensClean">The number regens clean.</param>
		/// <param name="throughputClean">The throughput clean.</param>
		/// <param name="throughpuReplace">The throughpu replace.</param>
		/// <param name="trainList">The train list.</param>
		/// <param name="currentTrain">The current train.</param>
		/// <param name="isFirstLoad">if set to <c>true</c> [is first load].</param>
		/// <param name="saltSplitData">The salt split data.</param>
		/// <param name="grains">The grains.</param>
		/// <param name="amountCation">The amount cation.</param>
		/// <param name="amountAnion">The amount anion.</param>
		/// <param name="lbsCaustic">The LBS caustic.</param>
		/// <param name="lbsAcid">The LBS acid.</param>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the open cost</returns>
		List<Tuple<int, double, double>> CalculatorInitializer(int numberOfTrains, List<Tuple<int, double>> numberRegensNormOps, List<Tuple<int, double>> numberRegensClean,
												 Dictionary<DateTime, Tuple<int, double, string>> throughputClean,
												 Dictionary<DateTime, Tuple<int, double, string>> throughpuReplace,
												 System.Data.DataTable trainList, int currentTrain, bool isFirstLoad, Dictionary<int, Tuple<double?, double?>> saltSplitData, Dictionary<int, double> grains,
                                                 double amountCation, double amountAnion, double lbsCaustic, double lbsAcid, string customerId, double causticPrice, double acidPrice);

		/// <summary>
		/// Price_s the calculator.
		/// </summary>
		/// <param name="numberOfTrains">The number of trains.</param>
		/// <param name="numberRegensNormOps">The number regens norm ops.</param>
		/// <param name="numberRegensClean">The number regens clean.</param>
		/// <param name="throughputClean">The throughput clean.</param>
		/// <param name="throughputNormOps">The throughput norm ops.</param>
		/// <param name="trainList">The train list.</param>
		/// <returns>Returns the open cost</returns>
		List<Price_Data> PriceCalculator(int numberOfTrains, List<Tuple<int, double>> numberRegensNormOps, List<Tuple<int, double>> numberRegensClean,
												 Dictionary<DateTime, Tuple<int, double, string>> throughputClean,
												 Dictionary<DateTime, Tuple<int, double, string>> throughputNormOps,
												 System.Data.DataTable trainList);

		/// <summary>
		/// Weekses the untill_ break even.
		/// </summary>
		/// <param name="weeklyBeforeCost">The wkly before cost.</param>
		/// <param name="weeklyAfterCost">The wkly after cost.</param>
		/// <param name="investmentCost">The investment cost.</param>
		/// <returns>Returns the WeeksUntillBreakEven</returns>
		double? WeeksUntillBreakEven(double weeklyBeforeCost, double weeklyAfterCost, double investmentCost);

		/// <summary>
		/// Gets the cumulative savings.
		/// </summary>
		/// <returns>Returns CumulativeSavings</returns>
		List<string> GetCumulativeSavings();

		/// <summary>
		/// Selecteds the week_ data_ finder.
		/// </summary>
		/// <param name="week">The week.</param>
		/// <returns>Returns SelectedWeekData</returns>
		double?[] SelectedWeekDataFinder(double week);

		/// <summary>
		/// Gets the cost analyzer results data.
		/// </summary>
		/// <returns>Returns Cost Analyzer Result</returns>
		CostAnalyzerResult GetCostAnalyzerResultsData();
	}
}
