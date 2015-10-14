// -----------------------------------------------------------------------
// <copyright file="IConductivityService.cs" company="RTI">
// RTI
// </copyright>
// <summary>IConductivity Service</summary>
// -----------------------------------------------------------------------

using RTI.ModelingSystem.Core.DBModels;
using RTI.ModelingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.ModelingSystem.Core.Interfaces.Services
{
	public interface IConductivityService
	{
		/// <summary>
		/// Gets the conductivitydata.
		/// </summary>
		/// <param name="usgsId">The usgsid.</param>
		/// <returns>Returns the water conductivity data</returns>
		water_data GetConductivitydata(long usgsId);

		/// <summary>
		/// Calculates the statistics.
		/// </summary>
		/// <param name="usgsId">The usgsid.</param>
		/// <returns>Returns the Conductivity Statistics</returns>
		ConductivityStatistics CalculateStatistics(int usgsId);

		/// <summary>
		/// Gets the forecastdata.
		/// </summary>
		/// <param name="usgsId">The usgsid.</param>
		/// <returns>ReturnsForecast Data</returns>
		ForecastData GetForecastdata(long usgsId);
	}
}
