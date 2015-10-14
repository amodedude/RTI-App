// -----------------------------------------------------------------------
// <copyright file="IResinProductsRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>IResinProducts Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Interfaces.Repository
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;
    using RTI.ModelingSystem.Core.Models;

    #endregion Usings

	public interface IResinProductsRepository
	{
		/// <summary>
		/// Gets all resin product names.
		/// </summary>
		/// <returns>Returns the product names</returns>
		List<string> GetAllResinProductNames();

		/// <summary>
		/// Gets all resin product details.
		/// </summary>
		/// <returns>Returns the product details</returns>
		List<resin_products> GetAllResinProductDetails();

		/// <summary>
		/// Gets the name of all resin product details by product.
		/// </summary>
		/// <param name="productname">The product_name.</param>
		/// <returns>Returns the resin product details</returns>
		List<resin_products> GetAllResinProductDetailsByProductName(string productname);
	}
}
