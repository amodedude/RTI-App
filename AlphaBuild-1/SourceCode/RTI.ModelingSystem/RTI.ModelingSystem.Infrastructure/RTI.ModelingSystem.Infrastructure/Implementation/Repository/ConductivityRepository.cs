// -----------------------------------------------------------------------
// <copyright file="ConductivityRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Conductivity Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
	#region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Interfaces.Repository;
    using RTI.ModelingSystem.Core.Models;
    using RTI.ModelingSystem.Infrastructure.Data;

	#endregion Usings

	/// <summary>
	/// ConductivityRepository class
	/// </summary>
	public class ConductivityRepository : Repository<source>, IConductivityRepository
	{
		#region Properties

		/// <summary>
		/// db Context
		/// </summary>
		private RtiContext dbContext;

		#endregion Properties

		#region Methods

		/// <summary>
		/// ConductivityRepository constructor
		/// </summary>
		/// <param name="datacontext">datacontext parameter</param>
		public ConductivityRepository(RtiContext datacontext)
			: base(datacontext)
		{
			this.dbContext = datacontext;
		}

		/// <summary>
		/// Gets the Agent Id based on source id
		/// </summary>
		/// <param name="sourceId">SourceID parameter</param>
		/// <returns>Returns the agent id</returns>
		public long GetAgentId(int sourceId)
		{
			try
			{
				var agencyId = (from r in rtiContext.sources
								where r.sources_sourceID == sourceId
								select r.agency_id).FirstOrDefault();

				long ID = Convert.ToInt64(agencyId);
				return ID;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the water source data.
		/// </summary>
		/// <param name="usgsId">The usgsid.</param>
		/// <returns>Returns the list of water source</returns>
		public List<water_data> GetWaterSourceData(long usgsId)
		{
			try
			{
				var waterdata = (from r in rtiContext.water_data
								 where r.sourceID == usgsId
								 select r).ToList();

				return waterdata;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Trains
		/// </summary>
		/// <param name="customerId">CustId paramter</param>
		/// <returns>Returns the customer id</returns>
		public long GetTrains(int customerId)
		{
			try
			{
				var CustomerId = (from r in rtiContext.trains
								  where r.customer_customerID == customerId
								  select r.trainID).FirstOrDefault();

				long ID = Convert.ToInt64(CustomerId);
				return ID;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the TrainId by customer
		/// </summary>
		/// <param name="customerId">CustId paramter</param>
		/// <returns>Returns the Train id</returns>
		public long GetTrainIdByCsutomer(int customerId)
		{
			try
			{
				var agencyId = (from r in rtiContext.trains
								where r.customer_customerID == customerId
								select r.trainID).FirstOrDefault();

				long ID = Convert.ToInt64(agencyId);
				return ID;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the Train Details by CustomerId and TrainID
		/// </summary>
		/// <param name="trainId">TrainId parameter</param>
		/// <param name="customerId">CustomerId parameter</param>
		/// <returns>Returns the train</returns>
		public train GetTrainDetailsByCustomerIdandTrainId(long trainId, int customerId)
		{
			try
			{
				train trainObj = (from r in rtiContext.trains
								  where r.customer_customerID == customerId && r.trainID == trainId
								  select r).FirstOrDefault();

				return trainObj;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Updates the System Settings
		/// </summary>
		/// <param name="systemSettings">system settings</param>
		/// <param name="customerId">customerId paramter</param>
		public void UpdateSystemSettings(SystemSettings systemSettings, string customerId)
		{
			try
			{
				int custId = Convert.ToInt32(customerId);

				using (TransactionScope scope = new TransactionScope())
				{
					var Customer = (from r in rtiContext.customers
									where r.customerID == custId
									select r).FirstOrDefault();

					if (Customer != null)
					{
						Customer.acid_price = systemSettings.acid_price;
						Customer.caustic_price = systemSettings.caustic_price;
						Customer.demand = Convert.ToInt32(systemSettings.demand);
					}

					var trains = (from r in rtiContext.trains
								  where r.customer_customerID == custId
								  select r);

					foreach (var train in trains)
					{
                        if (systemSettings.isManifold == false)
                        {
                            train.using_manifold = "NO";
                        }
                        else
                        {
                            train.using_manifold = "YES";
                        }
					}

					var custWater = (from r in rtiContext.customer_water
									 where r.customer_customerID == custId
									 select r).FirstOrDefault();

					rtiContext.customer_water.Remove(custWater);
					rtiContext.SaveChanges();

					if (systemSettings.HasTwoSources)
					{
						custWater.second_sourceID = int.Parse(systemSettings.secondWaterSource);
						custWater.firstSourcePercentage = systemSettings.firstWSPercentage;
					}
					else
					{
						custWater.second_sourceID = 0;
						custWater.firstSourcePercentage = 100;
					}

					custWater.first_sourceID = int.Parse(systemSettings.firstWaterSource);

					rtiContext.customer_water.Add(custWater);
					rtiContext.SaveChanges();
					scope.Complete();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Adds the System Settings
		/// </summary>
		/// <param name="systemSettings">system settings</param>
		/// <param name="customerId">customerId paramter</param>
		public void AddSystemSettings(SystemSettings systemSettings, string customerId)
		{
			try
			{
				int custId = Convert.ToInt32(customerId);
				using (TransactionScope scope = new TransactionScope())
				{
					var Customer = (from r in rtiContext.customers
									where r.customerID == custId
									select r).FirstOrDefault();
					if (Customer != null)
					{
						Customer.acid_price = systemSettings.acid_price;
						Customer.caustic_price = systemSettings.caustic_price;
						Customer.demand = Convert.ToInt32(systemSettings.demand);
						for (int i = 0; i < Customer.num_trains; i++)
						{
							train train = new train();
							if (systemSettings.isManifold == false)
							{
								train.using_manifold = "NO";
							}
							else
							{
								train.using_manifold = "YES";
							}
							train.regen_duration = "0";
							train.gpm = "0";
							train.regens_per_month = "0";
							train.customer_customerID = Convert.ToInt32(Customer.customerID);
							rtiContext.trains.Add(train);
						}
					}

					var custWater = new customer_water();
					if (systemSettings.HasTwoSources)
					{
						custWater.second_sourceID = int.Parse(systemSettings.secondWaterSource);
						custWater.firstSourcePercentage = systemSettings.firstWSPercentage;
					}
					else
					{
						custWater.second_sourceID = 0;
						custWater.firstSourcePercentage = 100;
					}
					custWater.first_sourceID = int.Parse(systemSettings.firstWaterSource);
					custWater.customer_customerID = Customer.customerID;
					rtiContext.customer_water.Add(custWater);
					rtiContext.SaveChanges();
					scope.Complete();
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