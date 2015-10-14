// -----------------------------------------------------------------------
// <copyright file="ICustomerRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>ICustomer Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{

    #region Usings

    using RTI.ModelingSystem.Core.DBModels;
    using System.Collections.Generic;

    #endregion Usings

	public interface ICustomerRepository
	{
		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the customer</returns>
		customer FindById(string customerId);

		/// <summary>
		/// Gets the state list.
		/// </summary>
		/// <returns>Returns the sources</returns>
		List<source> GetStateList();

		/// <summary>
		/// Gets the water source ids.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the customer water</returns>
		customer_water GetWaterSourceIds(long customerId);

		/// <summary>
		/// Gets the water source details.
		/// </summary>
		/// <param name="sourceId">The source identifier.</param>
		/// <returns>Returns the source</returns>
		source GetWaterSourceDetails(long sourceId);

		/// <summary>
		/// Gets the customer trains.
		/// </summary>
		/// <param name="customerId">The customer identifier.</param>
		/// <returns>Returns the train list</returns>
		List<train> GetCustomerTrains(long customerId);

		/// <summary>
		/// Checks for duplicates.
		/// </summary>
		/// <param name="customer">The customer.</param>
		/// <returns>Returns true if customer exists in the database</returns>
		bool CheckForDuplicates(customer customer);

		/// <summary>
		/// Updates the customer.
		/// </summary>
		/// <param name="cust">The customer.</param>
		void UpdateCustomer(customer cust);
	}
}
