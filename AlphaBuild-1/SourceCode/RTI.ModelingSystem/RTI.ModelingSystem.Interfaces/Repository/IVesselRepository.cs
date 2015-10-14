// -----------------------------------------------------------------------
// <copyright file="IVesselRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>IVessel Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

	public interface IVesselRepository
	{
		/// <summary>
		/// Fetches the cation1 details.
		/// </summary>
		/// <param name="trainID">The train identifier.</param>
		/// <returns>Returns the vessel</returns>
		vessel FetchCation1Details(int trainID);

		/// <summary>
		/// Fetches the vessels list.
		/// </summary>
		/// <param name="trainID">The train identifier.</param>
		/// <returns>Returns the list of vessels</returns>
		List<vessel> FetchVesselsList(long trainID);

		/// <summary>
		/// Gets the new vessel identifier.
		/// </summary>
		/// <returns>Returns the new vessel id</returns>
		int GetNewVesselId();

		/// <summary>
		/// Inserts the vessel.
		/// </summary>
		/// <param name="vesselObj">The vessel object.</param>
		void InsertVessel(vessel vesselObj);

		/// <summary>
		/// Gets the resin identifier.
		/// </summary>
		/// <param name="resinModel">The resin model.</param>
		/// <returns>Returns the new resin id</returns>
		int GetResinId(string resinModel);

		/// <summary>
		/// Updates the vessel.
		/// </summary>
		/// <param name="vesselObj">The vessel object.</param>
		void UpdateVessel(vessel vesselObj);

		/// <summary>
		/// Gets the resin by identifier.
		/// </summary>
		/// <param name="resinProductId">The resin_product_id.</param>
		/// <returns>Returns the resin</returns>
		resin_products GetResinById(int resinProductId);

		/// <summary>
		/// Deletes the vessel.
		/// </summary>
		/// <param name="vesselObj">The vessel object.</param>
		void DeleteVessel(vessel vesselObj);
	}
}
