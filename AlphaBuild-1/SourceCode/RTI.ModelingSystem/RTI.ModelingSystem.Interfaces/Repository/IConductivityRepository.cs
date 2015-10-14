// -----------------------------------------------------------------------
// <copyright file="IConductivityRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>IConductivity Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
	#region Usings

	using RTI.ModelingSystem.Core.DBModels;
	using RTI.ModelingSystem.Core.Models;
	using System.Collections.Generic;

	#endregion Usings

	/// <summary>
	/// Conductivity Repository interface
	/// </summary>
	public interface IConductivityRepository
	{
		/// <summary>
		/// Gets the agent identifier.
		/// </summary>
		/// <param name="sourceId">The source identifier.</param>
		/// <returns>Returns the agent id</returns>
		long GetAgentId(int sourceId);

		/// <summary>
		/// Gets the trains.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the train</returns>
		long GetTrains(int customerId);

		/// <summary>
		/// Gets the train details by customer idand train identifier.
		/// </summary>
		/// <param name="trainId">The train identifier.</param>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the train</returns>
		train GetTrainDetailsByCustomerIdandTrainId(long trainId, int customerId);

		/// <summary>
		/// Gets the train identifier by csutomer.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the train id</returns>
		long GetTrainIdByCsutomer(int customerId);

		/// <summary>
		/// Gets the water source data.
		/// </summary>
		/// <param name="usgsId">The usgs identifier.</param>
		/// <returns>Returns the water data</returns>
		List<water_data> GetWaterSourceData(long usgsId);

		/// <summary>
		/// Updates the system settings.
		/// </summary>
		/// <param name="systemSettings">The system settings.</param>
		/// <param name="customerId">The customer identifier.</param>
		void UpdateSystemSettings(SystemSettings systemSettings, string customerId);

		/// <summary>
		/// Adds the system settings.
		/// </summary>
		/// <param name="systemSettings">The system settings.</param>
		/// <param name="customerId">The customer identifier.</param>
		void AddSystemSettings(SystemSettings systemSettings, string customerId);
	}
}
