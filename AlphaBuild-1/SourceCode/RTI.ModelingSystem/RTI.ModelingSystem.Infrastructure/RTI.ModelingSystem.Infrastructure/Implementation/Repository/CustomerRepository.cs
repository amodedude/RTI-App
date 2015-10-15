// -----------------------------------------------------------------------
// <copyright file="CustomerRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Customer Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
	#region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Infrastructure.Data;

	#endregion Usings

	/// <summary>
	/// CustomerRepository class
	/// </summary>
	public class CustomerRepository : Repository<customer>, ICustomerRepository
	{
		#region Properties

		/// <summary>
		/// Db Context
		/// </summary>
		RtiContext bContext;

		#endregion Properties

		#region Methods

		/// <summary>
		/// CustomerRepository constructor
		/// </summary>
		/// <param name="datacontext">datacontext parameter</param>
		public CustomerRepository(RtiContext datacontext)
			: base(datacontext)
		{
			this.bContext = datacontext;
		}

		/// <summary>
		/// Finds the customer By Id
		/// </summary>
		/// <param name="customerId">customerId parameter</param>
		/// <returns>Returns the customer</returns>
		public customer FindById(string customerId)
		{
			try
			{
				var customer = bContext.customers != null ? (from r in bContext.customers
															 where r.customerID.ToString() == customerId
															 select r).FirstOrDefault()
														 : new customer();

				return customer;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the States List
		/// </summary>
		/// <returns>Returns the list of source</returns>
		public List<source> GetStateList()
		{
			try
			{
				var stateList = bContext.sources != null ? (from r in bContext.sources
															select r).ToList()
														: new List<source>();
				return stateList;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Water SourceIds
		/// </summary>
		/// <param name="customerId">CustomerId aparameter</param>
		/// <returns>Returns the customer water</returns>
		public customer_water GetWaterSourceIds(long customerId)
		{
			try
			{
				var WatersourceIds = bContext.customer_water != null ? (from r in bContext.customer_water
																		where r.customer_customerID == customerId
																		select r).FirstOrDefault()
																	: new customer_water();
				return WatersourceIds;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Water Source Details
		/// </summary>
		/// <param name="sourceId">Source Identifier</param>
		/// <returns>Returns the source</returns>
		public source GetWaterSourceDetails(long sourceId)
		{
			try
			{
				var WatersourceIds = bContext.sources != null ? (from s in bContext.sources
																 where s.sources_sourceID == sourceId
																 select s).FirstOrDefault()
															 : new source();
				return WatersourceIds;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Customer Trains
		/// </summary>
		/// <param name="customerId">CustomerId parameter</param>
		/// <returns>Returns the list of trains</returns>
		public List<train> GetCustomerTrains(long customerId)
		{
			try
			{
				var CutomerTrains = bContext.trains != null ? (from Train in bContext.trains
															   where Train.customer_customerID == customerId
															   select Train).ToList()
														   : new List<train>();

				return CutomerTrains;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Updates the Customer
		/// </summary>
		/// <param name="customer">customer parameter</param>
		public void UpdateCustomer(customer customer)
		{
			try
			{
				var Customer = bContext.customers != null ? (from r in bContext.customers
															 where r.customerID == customer.customerID
															 select r).FirstOrDefault()
														 : new customer();

				Customer.plant = customer.plant;
				Customer.state = customer.state;
				Customer.city = customer.city;
				Customer.num_trains = customer.num_trains;
				bContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Checks For Duplicate of customers
		/// </summary>
		/// <param name="customer">customer parameter</param>
		/// <returns>Returns true if customer already exists in the database else false</returns>
		public bool CheckForDuplicates(customer customer)
		{
			try
			{
				var cust = bContext.customers != null ? (from r in bContext.customers
														 where r.name == customer.name && r.state == customer.state && r.city == customer.city
														 select r).FirstOrDefault()
													 : new customer();
				if (cust == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion Methods
	}
}
