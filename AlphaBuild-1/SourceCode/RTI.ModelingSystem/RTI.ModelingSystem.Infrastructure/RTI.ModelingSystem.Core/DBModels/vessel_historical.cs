// -----------------------------------------------------------------------
// <copyright file="vessel_historical.cs" company="RTI">
// RTI
// </copyright>
// <summary>Vessel Historical Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class vessel_historical
    {
        public vessel_historical()
        {
        }

        /// <summary>
        /// Gets or sets the sample identifier.
        /// </summary>
        /// <value>
        /// The sample identifier.
        /// </value>
        [Key]
        [Required]
        public int sampleID { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public string date { get; set; }
        /// <summary>
        /// Gets or sets the lbs_chemical.
        /// </summary>
        /// <value>
        /// The lbs_chemical.
        /// </value>
        public Nullable<int> lbs_chemical { get; set; }
        /// <summary>
        /// Gets or sets the throughput.
        /// </summary>
        /// <value>
        /// The throughput.
        /// </value>
        public string throughput { get; set; }
        /// <summary>
        /// Gets or sets the num_regens.
        /// </summary>
        /// <value>
        /// The num_regens.
        /// </value>
        public string num_regens { get; set; }
        /// <summary>
        /// Gets or sets the toc.
        /// </summary>
        /// <value>
        /// The toc.
        /// </value>
        public string toc { get; set; }
        /// <summary>
        /// Gets or sets the vessel_historical_customer identifier.
        /// </summary>
        /// <value>
        /// The vessel_historical_customer identifier.
        /// </value>
        public string vessel_historical_customerID { get; set; } 
    }
}
