// -----------------------------------------------------------------------
// <copyright file="IPredictiveModelRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>IPredictiveModel Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Models;

    #endregion Usings

	public interface IPredictiveModelRepository
	{
		/// <summary>
		/// Gets the water sourcedata.
		/// </summary>
		/// <param name="usgsId">The usgsid.</param>
		/// <returns>Returns WaterSource</returns>
		List<water_data> GetWaterSourcedata(long usgsId);

		/// <summary>
		/// Gets the average data by week.
		/// </summary>
		/// <param name="agencyId">The agency identifier.</param>
		/// <param name="order">The order.</param>
		void GetAverageDataByWeek(string agencyId, int order);

		/// <summary>
		/// Gets the grains weight total.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns GrainsWeightTotal</returns>
		double GetGrainsWeightTotal(string customerId);

		/// <summary>
		/// Gets the water demand.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns water demand</returns>
		int GetWaterDemand(long customerId);

		/// <summary>
		/// Gets the customer trains.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the customer trains</returns>
		List<train> GetCustomerTrains(long customerId);

		/// <summary>
		/// Gets the customer vessels.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the customer vessel</returns>
		List<vessel> GetCustomerVessels(long customerId);

		/// <summary>
		/// Fetches the source identifier tp.
		/// </summary>
		/// <param name="CustomerId">The customer identifier.</param>
		/// <returns>Returns the source id</returns>
		double[] FetchSourceIdTP(string CustomerId);

		/// <summary>
		/// Gets the water statistics.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the water statistics</returns>
		WaterStatisticsViewModel GetWaterStatistics(long customerId);

        /// <summary>
        /// Gets first source percentage
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns></returns>
        int GetFirstSourcePercentage(string customerId);
	}
}
