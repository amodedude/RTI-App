// -----------------------------------------------------------------------
// <copyright file="ResinProductsRepository.cs" company="RTI">
// RTI
// </copyright>
// <summary>Resin Products Repository</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Infrastructure.Implementation.Repository
{
	#region Usings

	using System.Collections.Generic;
	using System.Linq;
	using RTI.ModelingSystem.Core.Interfaces.Repository;
	using RTI.ModelingSystem.Core.Models;
	using RTI.ModelingSystem.Infrastructure.Data;
	using RTI.ModelingSystem.Core.DBModels;
	using System;

	#endregion Usings

	/// <summary>
	/// Resin Products Repository class
	/// </summary>
	public class ResinProductsRepository : Repository<resin_products>, IResinProductsRepository
	{
		#region Properties

		/// <summary>
		/// Db Context
		/// </summary>
		RtiContext bContext;

		#endregion Properties

		#region Constructor

		public ResinProductsRepository(RtiContext datacontext)
			: base(datacontext)
		{
			this.bContext = datacontext;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Gets All Resin Product Names
		/// </summary>
		/// <returns>Returns the list of strings</returns>
		public List<string> GetAllResinProductNames()
		{
			try
			{
				List<string> strResinProductModels;
				strResinProductModels = bContext.resin_products != null ? (from r in bContext.resin_products
																		   select r.name).Distinct().ToList<string>()
																		   : new List<string>();

				return strResinProductModels;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets All Resin Product Details
		/// </summary>
		/// <returns>Returns list of resin products</returns>
		public List<resin_products> GetAllResinProductDetails()
		{
			try
			{
				var strResinProductModels = bContext.resin_products != null ? (from r in bContext.resin_products
																			   select r).ToList<resin_products>()
																				   : new List<resin_products>();
				return strResinProductModels;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Get All Resin Product Details By ProductName
		/// </summary>
		/// <param name="productName">product name</param>
		/// <returns>Returns the list of resin products</returns>
		public List<resin_products> GetAllResinProductDetailsByProductName(string productName)
		{
			try
			{
				List<resin_products> strResinProductModels;
				strResinProductModels = bContext.resin_products != null ? (from r in bContext.resin_products.Where(p => p.name == productName)
																		   select r).ToList<resin_products>()
																		   : new List<resin_products>();
				return strResinProductModels;
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion Methods
	}
}
