// -----------------------------------------------------------------------
// <copyright file="TrainRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Train Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
	#region Usings

	using RTI.ModelingSystem.Core.DBModels;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Infrastructure.Data;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	#endregion Usings

	/// <summary>
	/// TrainRepository class
	/// </summary>
	public class TrainRepository : Repository<train>, ITrainRepository
	{
		#region Properties

		/// <summary>
		/// Db Context
		/// </summary>
		private RtiContext bContext;

		#endregion Properties

		#region Constructor

		/// <summary>
		/// TrainRepository constructor
		/// </summary>
		/// <param name="datacontext">datacontext parameter</param>
		public TrainRepository(RtiContext datacontext)
			: base(datacontext)
		{
			this.bContext = datacontext;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Finds the train By Id
		/// </summary>
		/// <param name="trainId">trainId parameter</param>
		/// <returns>Returns the train</returns>
		public train FindById(string trainId)
		{
			try
			{
				var train = bContext.trains != null ? (from r in bContext.trains where r.trainID.ToString() == trainId select r).FirstOrDefault() : new train();
				return train;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the New TrainId
		/// </summary>
		/// <returns>Returns the train id</returns>
		int ITrainRepository.GetNewTrainId()
		{
			return bContext.trains != null ? (bContext.trains.OrderByDescending(t => t.trainID).FirstOrDefault().trainID) + 1 : 1;
		}

		/// <summary>
		/// Inserts the Train
		/// </summary>
		/// <param name="trainObj">Train parameter</param>
		public void InsertTrain(train trainObj)
		{
			try
			{
				train NewTrain = new train();
				NewTrain.trainID = trainObj.trainID;
				NewTrain.name = trainObj.name;
				NewTrain.number = trainObj.number;
				NewTrain.gpm = trainObj.gpm;
				NewTrain.num_beds_cation = trainObj.num_beds_cation;
				NewTrain.num_beds_anion = trainObj.num_beds_anion;
				NewTrain.using_manifold = trainObj.using_manifold;
				NewTrain.regens_per_month = trainObj.regens_per_month;
				NewTrain.regen_duration = trainObj.regen_duration;
				NewTrain.has_mixed_bed = trainObj.has_mixed_bed;
				NewTrain.has_historical_data = trainObj.has_historical_data;
				NewTrain.customer_customerID = 2;
				if (rtiContext.trains != null)
				{
					rtiContext.trains.Add(NewTrain);
					rtiContext.SaveChanges();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Resin ProductModels Data
		/// </summary>
		/// <param name="slug">slug parameter</param>
		/// <returns>Returns the list of resin products</returns>
		public List<resin_products> ResinProductModelsData(string slug)
		{
			try
			{
				List<resin_products> strResinProductModels;
				strResinProductModels = bContext.resin_products != null ? (from r in bContext.resin_products where r.model_number.ToString().Contains(slug) select r).ToList<resin_products>() : new List<resin_products>();
				return strResinProductModels;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Updates the Train
		/// </summary>
		/// <param name="train">Train parameter</param>
		public void UpdateTrain(train train)
		{
			try
			{
				var TrainDB = bContext.trains != null ? (from T in bContext.trains where T.trainID == train.trainID && T.customer_customerID == train.customer_customerID select T).FirstOrDefault() : new train();
				if (train.number != null)
				{
					TrainDB.number = train.number;
				}
				if (train.gpm != null)
				{
					TrainDB.gpm = train.gpm;
				}
				if (train.num_beds_cation != null)
				{
					TrainDB.num_beds_cation = train.num_beds_cation;
				}
				if (train.num_beds_anion != null)
				{
					TrainDB.num_beds_anion = train.num_beds_anion;
				}
				if (train.using_manifold != null)
				{
					TrainDB.using_manifold = train.using_manifold;
				}
				if (train.regens_per_month != null)
				{
					TrainDB.regens_per_month = train.regens_per_month;
				}
				if (train.regen_duration != null)
				{
					TrainDB.regen_duration = train.regen_duration;
				}
				bContext.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion Methods
	}
}