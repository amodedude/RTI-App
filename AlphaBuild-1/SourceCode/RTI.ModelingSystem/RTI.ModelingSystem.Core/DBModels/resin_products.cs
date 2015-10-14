// -----------------------------------------------------------------------
// <copyright file="resin_products.cs" company="RTI">
// RTI
// </copyright>
// <summary>Resin Products Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class resin_products
    {
        public resin_products()
        {
        }

        /// <summary>
        /// Gets or sets the resin_product_id.
        /// </summary>
        /// <value>
        /// The resin_product_id.
        /// </value>
        [Key]
        [Required]
        public int resin_product_id { get; set; }
        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string manufacturer { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the model_number.
        /// </summary>
        /// <value>
        /// The model_number.
        /// </value>
        public string model_number { get; set; }
        /// <summary>
        /// Gets or sets the resin_type.
        /// </summary>
        /// <value>
        /// The resin_type.
        /// </value>
        public string resin_type { get; set; }
        /// <summary>
        /// Gets or sets the primary_type.
        /// </summary>
        /// <value>
        /// The primary_type.
        /// </value>
        public string primary_type { get; set; }
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string group { get; set; }
        /// <summary>
        /// Gets or sets the teir.
        /// </summary>
        /// <value>
        /// The teir.
        /// </value>
        public string teir { get; set; }
        /// <summary>
        /// Gets or sets the chemical_structure.
        /// </summary>
        /// <value>
        /// The chemical_structure.
        /// </value>
        public string chemical_structure { get; set; }
        /// <summary>
        /// Gets or sets the physical_structure.
        /// </summary>
        /// <value>
        /// The physical_structure.
        /// </value>
        public string physical_structure { get; set; }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string color { get; set; }
        /// <summary>
        /// Gets or sets the total_capacity.
        /// </summary>
        /// <value>
        /// The total_capacity.
        /// </value>
        public string total_capacity { get; set; }
        /// <summary>
        /// Gets or sets the salt_split_ cap.
        /// </summary>
        /// <value>
        /// The salt_split_ cap.
        /// </value>
        public string salt_split_CAP { get; set; }
        /// <summary>
        /// Gets or sets the price_per_cuft.
        /// </summary>
        /// <value>
        /// The price_per_cuft.
        /// </value>
        public string price_per_cuft { get; set; }
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string comments { get; set; } 
    }
}
