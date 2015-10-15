// -----------------------------------------------------------------------
// <copyright file="ResinModel.cs" company="RTI">
// RTI
// </copyright>
// <summary>Resin Model</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    public class ResinModel
    {
        public ResinModel() {
            this.ProductNamesList = new List<string>();
            this.ResinProductsList = new List<resin_products>();
        }

        /// <summary>
        /// Gets or sets the resin products list.
        /// </summary>
        /// <value>
        /// The resin products list.
        /// </value>
        public List<resin_products> ResinProductsList { get; set; }

        /// <summary>
        /// Gets or sets the product names list.
        /// </summary>
        /// <value>
        /// The product names list.
        /// </value>
        public List<string> ProductNamesList { get; set; }

        /// <summary>
        /// Gets or sets the selected product.
        /// </summary>
        /// <value>
        /// The selected product.
        /// </value>
		public string SelectedProduct { get; set; }
    }
}
