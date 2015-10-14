// -----------------------------------------------------------------------
// <copyright file="IPredictiveModelService.cs" company="RTI">
// RTI
// </copyright>
// <summary>IPredictiveModel Service</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Services
{
	#region Usings

	using RTI.ModelingSystem.Core.DBModels;
	using RTI.ModelingSystem.Core.Models;
	using System;
	using System.Collections.Generic;

	#endregion Usings

	/// <summary>
	/// Predictive Model Service interface
	/// </summary>
	public interface IPredictiveModelService
	{
		/// <summary>
		/// Computes the data points.
		/// </summary>
		/// <param name="numberWeeks">The number weeks.</param>
		/// <param name="startingSaltSplit">The starting salt split.</param>
		/// <param name="maximumDegradationSaltSplit">The maximum degradation salt split.</param>
		/// <returns></returns>
		Dictionary<double, double> ComputeDataPoints(double numberWeeks, double startingSaltSplit, double maximumDegradationSaltSplit);

		/// <summary>
		/// Calculates the minimum salt split.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="selectedTrainId">The selected train identifier.</param>
		/// <returns></returns>
		List<double> CalculateMinSaltSplit(long id, string selectedTrainId = "0");

		/// <summary>
		/// Currents the ss conditions.
		/// </summary>
		/// <param name="resinAge">The resin age.</param>
		/// <param name="cleaningEffectiveness">The cleaning effectiveness.</param>
		/// <param name="startingSaltSplit">The starting salt split.</param>
		/// <returns></returns>
		Dictionary<double, double> CurrentSSConditions(double resinAge, double cleaningEffectiveness, double startingSaltSplit);

		/// <summary>
		/// Thoughputcharts the specified customer identifier.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <param name="currentSaltSplit">The current salt split.</param>
		/// <param name="startingSaltSplit">The starting salt split.</param>
		/// <param name="resinLifeExpectancy">The resin life expectancy.</param>
		/// <param name="simulationConfidence">The simulation confidence.</param>
		/// <param name="numberSimulationIterations">The number simulation iterations.</param>
		/// <param name="simMethod">The sim method.</param>
		/// <param name="standardDeviationThreshold">The standard deviation threshold.</param>
		/// <param name="resinAge">The resin age.</param>
		/// <param name="replacementLevel">The replacement level.</param>
		/// <param name="rtiCleaningLevel">The rti cleaning level.</param>
		/// <param name="selectedTrain">The selected train.</param>
		/// <param name="donotReplaceResin">if set to <c>true</c> [donot replace resin].</param>
		/// <returns></returns>
		PriceData Thoughputchart(string customerId, double currentSaltSplit, double startingSaltSplit, double resinLifeExpectancy, int simulationConfidence, int numberSimulationIterations, string simMethod, int standardDeviationThreshold, double resinAge, double replacementLevel, double rtiCleaningLevel, string selectedTrain, bool donotReplaceResin);
	}
}
