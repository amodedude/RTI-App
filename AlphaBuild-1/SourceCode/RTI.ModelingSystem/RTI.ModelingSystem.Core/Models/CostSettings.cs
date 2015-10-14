// -----------------------------------------------------------------------
// <copyright file="CostSettings.cs" company="RTI">
// RTI
// </copyright>
// <summary>Cost Settings</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System;

    #endregion Usings

    public class CostSettings
    {
        /// <summary>
        /// Gets or sets the selected train.
        /// </summary>
        /// <value>
        /// The selected train.
        /// </value>
        public string SelectedTrain { get; set; }

        /// <summary>
        /// Gets or sets the acid price.
        /// </summary>
        /// <value>
        /// The acid price.
        /// </value>
        public Nullable<decimal> AcidPrice { get; set; }

        /// <summary>
        /// Gets or sets the caustic price.
        /// </summary>
        /// <value>
        /// The caustic price.
        /// </value>
        public Nullable<decimal> CausticPrice { get; set; }
    }
}
