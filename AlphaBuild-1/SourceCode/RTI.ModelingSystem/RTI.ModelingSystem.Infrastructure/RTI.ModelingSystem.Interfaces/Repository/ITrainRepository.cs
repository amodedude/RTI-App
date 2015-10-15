// -----------------------------------------------------------------------
// <copyright file="ITrainRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>ITrain Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

	public interface ITrainRepository
	{
		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <param name="trainId">The train identifier.</param>
		/// <returns>Returns the train</returns>
		train FindById(string trainId);

		/// <summary>
		/// Gets the new train identifier.
		/// </summary>
		/// <returns>Returns the new train id</returns>
		int GetNewTrainId();

		/// <summary>
		/// Inserts the train.
		/// </summary>
		/// <param name="trainObj">The train object.</param>
		void InsertTrain(train trainObj);

		/// <summary>
		/// Resins the product models data.
		/// </summary>
		/// <param name="slug">The slug.</param>
		/// <returns>Returns the resin products</returns>
		List<resin_products> ResinProductModelsData(string slug);

		/// <summary>
		/// Updates the train.
		/// </summary>
		/// <param name="train">The train.</param>
		void UpdateTrain(train train);
	}
}
