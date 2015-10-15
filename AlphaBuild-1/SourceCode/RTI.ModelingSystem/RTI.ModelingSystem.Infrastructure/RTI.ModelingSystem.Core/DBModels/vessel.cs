// -----------------------------------------------------------------------
// <copyright file="vessel.cs" company="RTI">
// RTI
// </copyright>
// <summary>Vessel Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class vessel
    {
        /// <summary>
        /// Gets or sets the vessel identifier.
        /// </summary>
        /// <value>
        /// The vessel identifier.
        /// </value>
        [Key]
        [Required]
        [Column(Order = 0)]
        public int vesselID { get; set; }
        /// <summary>
        /// Gets or sets the vessel_number.
        /// </summary>
        /// <value>
        /// The vessel_number.
        /// </value>
        public Nullable<int> vessel_number { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DisplayName("Vessel name")]
        [Required]
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [DisplayName("(ft3) of Resin")]
        [Required]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "(ft3) of Resin must be decimal")]
        public string size { get; set; }
        /// <summary>
        /// Gets or sets the bed_number.
        /// </summary>
        /// <value>
        /// The bed_number.
        /// </value>
        public string bed_number { get; set; }
        /// <summary>
        /// Gets or sets the lbs_chemical.
        /// </summary>
        /// <value>
        /// The lbs_chemical.
        /// </value>
        [DisplayName("lb/ (ft3) of Acid")]
        [Required]
        [Range(0, 20)]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "lb/ (ft3) of Acid must be decimal")]
        public string lbs_chemical { get; set; }
        /// <summary>
        /// Gets or sets the date_replaced.
        /// </summary>
        /// <value>
        /// The date_replaced.
        /// </value>
        [DisplayName("Purchase date")]
        [Required]
        [DataType(DataType.Date)]
        [RegularExpression("(^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/][0-9]{4}$)", ErrorMessage = "Purchase date must be in format MM/DD/YYYY")]
        public string date_replaced { get; set; }
        /// <summary>
        /// Gets or sets the replacement_plan.
        /// </summary>
        /// <value>
        /// The replacement_plan.
        /// </value>
        [DisplayName("Replace")]
        [Required]
        [Range(0, 20)]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Replace must be an integer")]
        public string replacement_plan { get; set; }
        /// <summary>
        /// Gets or sets the throughput.
        /// </summary>
        /// <value>
        /// The throughput.
        /// </value>
        [DisplayName("Throughput")]
        [Required]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "Throughput must be decimal")]
        public string throughput { get; set; }
        /// <summary>
        /// Gets or sets the num_regens.
        /// </summary>
        /// <value>
        /// The num_regens.
        /// </value>
        [DisplayName("Regens/mo.")]
        [Required]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Regens/mo. must be an integer")]
        public string num_regens { get; set; }
        /// <summary>
        /// Gets or sets the toc.
        /// </summary>
        /// <value>
        /// The toc.
        /// </value>
        [DisplayName("Total Organic Carbon")]
        [Required]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "Total Organic Carbon must be decimal")]
        public string toc { get; set; }
        /// <summary>
        /// Gets or sets the with_degassifier.
        /// </summary>
        /// <value>
        /// The with_degassifier.
        /// </value>
        public string with_degassifier { get; set; }
        /// <summary>
        /// Gets or sets the with_polisher.
        /// </summary>
        /// <value>
        /// The with_polisher.
        /// </value>
        public string with_polisher { get; set; }
        /// <summary>
        /// Gets or sets the vessel_customer identifier.
        /// </summary>
        /// <value>
        /// The vessel_customer identifier.
        /// </value>
        public string vessel_customerID { get; set; }
        /// <summary>
        /// Gets or sets the salt_ split.
        /// </summary>
        /// <value>
        /// The salt_ split.
        /// </value>
        [DisplayName("Salt Split")]
        [Required]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "Salt Split must be decimal")]
        public double Salt_Split { get; set; }
        /// <summary>
        /// Gets or sets the train_train identifier.
        /// </summary>
        /// <value>
        /// The train_train identifier.
        /// </value>
        [Key]
        [Column(Order = 1)]
        public int train_trainID { get; set; }
        /// <summary>
        /// Gets or sets the resin_data_product_id.
        /// </summary>
        /// <value>
        /// The resin_data_product_id.
        /// </value>
        [Key]
        [Column(Order = 2)]
        public int resin_data_product_id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="vessel"/> is degasifier.
        /// </summary>
        /// <value>
        ///   <c>true</c> if degasifier; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool Degasifier { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="vessel"/> is polisher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if polisher; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool Polisher { get; set; }
        /// <summary>
        /// Gets or sets the price_per_cuft.
        /// </summary>
        /// <value>
        /// The price_per_cuft.
        /// </value>
        [DisplayName("Resin Price")]
        [Column("price_cuft_resin")]
        [Required]
        [Range(0, 300)]
        [RegularExpression("(^[0-9][0-9]*(\\.[0-9][0-9]*)?$)", ErrorMessage = "Resin Price must be decimal")]
        public decimal price_per_cuft { get; set; }
        /// <summary>
        /// Gets or sets the salt_split_ cap.
        /// </summary>
        /// <value>
        /// The salt_split_ cap.
        /// </value>
        [NotMapped]
        public string salt_split_CAP { get; set; }
        /// <summary>
        /// Gets or sets the update vessel.
        /// </summary>
        /// <value>
        /// The update vessel.
        /// </value>
        [NotMapped]
        public string UpdateVessel { get; set; }
        /// <summary>
        /// Gets or sets the resin model.
        /// </summary>
        /// <value>
        /// The resin model.
        /// </value>
        [NotMapped]
        public string ResinModel { get; set; }
        /// <summary>
        /// Gets or sets the resin data.
        /// </summary>
        /// <value>
        /// The resin data.
        /// </value>
        [NotMapped]
        public string ResinData { get; set; }
    }
}
