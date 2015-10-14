// -----------------------------------------------------------------------
// <copyright file="customer_water.cs" company="RTI">
// RTI
// </copyright>
// <summary>Customer Water Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class customer_water
    {
        /// <summary>
        /// Gets or sets the customer_customer identifier.
        /// </summary>
        /// <value>
        /// The customer_customer identifier.
        /// </value>
        [Key]
        [Required]
        [Column(Order = 0)]
        public long customer_customerID { get; set; }
        /// <summary>
        /// Gets or sets the first_source identifier.
        /// </summary>
        /// <value>
        /// The first_source identifier.
        /// </value>
        [Key] 
        [Column (Order = 1)]
        public int first_sourceID { get; set; }
        /// <summary>
        /// Gets or sets the second_source identifier.
        /// </summary>
        /// <value>
        /// The second_source identifier.
        /// </value>
        [Key]
        [Column(Order = 2)]
        public int second_sourceID { get; set; }
        /// <summary>
        /// Gets or sets the first source percentage.
        /// </summary>
        /// <value>
        /// The first source percentage.
        /// </value>
        public int firstSourcePercentage { get; set; }
    }
}
