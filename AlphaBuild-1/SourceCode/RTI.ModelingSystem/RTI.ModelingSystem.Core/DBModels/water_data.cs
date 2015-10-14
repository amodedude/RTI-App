// -----------------------------------------------------------------------
// <copyright file="water_data.cs" company="RTI">
// RTI
// </copyright>
// <summary>Water Data Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class water_data
    {
        /// <summary>
        /// Gets or sets the data identifier.
        /// </summary>
        /// <value>
        /// The data identifier.
        /// </value>
        [Key]
        [Required]
        public int dataID { get; set; }
        /// <summary>
        /// Gets or sets the cond.
        /// </summary>
        /// <value>
        /// The cond.
        /// </value>
        public Nullable<int> cond { get; set; }
        /// <summary>
        /// Gets or sets the temporary.
        /// </summary>
        /// <value>
        /// The temporary.
        /// </value>
        public Nullable<int> temp { get; set; }
        /// <summary>
        /// Gets or sets the measurment_date.
        /// </summary>
        /// <value>
        /// The measurment_date.
        /// </value>
        public string measurment_date { get; set; }
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        public Nullable<int> sourceID { get; set; }
    }
}
