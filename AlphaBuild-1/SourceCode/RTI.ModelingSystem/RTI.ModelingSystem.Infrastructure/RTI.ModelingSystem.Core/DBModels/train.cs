// -----------------------------------------------------------------------
// <copyright file="train.cs" company="RTI">
// RTI
// </copyright>
// <summary>Train Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class train
    {
        /// <summary>
        /// Gets or sets the train identifier.
        /// </summary>
        /// <value>
        /// The train identifier.
        /// </value>
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int trainID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public Nullable<int> number { get; set; }
        /// <summary>
        /// Gets or sets the GPM.
        /// </summary>
        /// <value>
        /// The GPM.
        /// </value>
        [DisplayName("Gal / min. per train")]
        [Required]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Gal / min. per train must be an integer")]
        public string gpm { get; set; }
        /// <summary>
        /// Gets or sets the num_beds_cation.
        /// </summary>
        /// <value>
        /// The num_beds_cation.
        /// </value>
        public Nullable<int> num_beds_cation { get; set; }
        /// <summary>
        /// Gets or sets the num_beds_anion.
        /// </summary>
        /// <value>
        /// The num_beds_anion.
        /// </value>
        public Nullable<int> num_beds_anion { get; set; }
        /// <summary>
        /// Gets or sets the using_manifold.
        /// </summary>
        /// <value>
        /// The using_manifold.
        /// </value>
        public string using_manifold { get; set; }
        /// <summary>
        /// Gets or sets the regens_per_month.
        /// </summary>
        /// <value>
        /// The regens_per_month.
        /// </value>
        [DisplayName("Regens / mo")]
        [Required]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Regens / mo must be an integer")]
        public string regens_per_month { get; set; }
        /// <summary>
        /// Gets or sets the regen_duration.
        /// </summary>
        /// <value>
        /// The regen_duration.
        /// </value>
        [DisplayName("hrs for regeneration")]
        [Required]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "hrs for regeneration must be an integer")]
        public string regen_duration { get; set; }
        /// <summary>
        /// Gets or sets the has_mixed_bed.
        /// </summary>
        /// <value>
        /// The has_mixed_bed.
        /// </value>
        public string has_mixed_bed { get; set; }
        /// <summary>
        /// Gets or sets the has_historical_data.
        /// </summary>
        /// <value>
        /// The has_historical_data.
        /// </value>
        public string has_historical_data { get; set; }
        /// <summary>
        /// Gets or sets the customer_customer identifier.
        /// </summary>
        /// <value>
        /// The customer_customer identifier.
        /// </value>
        [Key]
        [Column(Order = 1)]
        public int customer_customerID { get; set; }
    }
}
