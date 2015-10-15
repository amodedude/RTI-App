// -----------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="RTI">
// RTI
// </copyright>
// <summary>System Settings</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Web.Mvc;

    #endregion Usings

    public partial class SystemSettings
    {
        public SystemSettings()
        {
            this.WaterSourceList1 = new List<SelectListItem>();
            this.WaterSourceList2 = new List<SelectListItem>();
        }

        /// <summary>
        /// Gets or sets the system settings identifier.
        /// </summary>
        /// <value>
        /// The system settings identifier.
        /// </value>
        [Key]
        public int systemSettingsID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is manifold.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is manifold; otherwise, <c>false</c>.
        /// </value>
        public bool isManifold { get; set; }

        /// <summary>
        /// Gets or sets the acid_price.
        /// </summary>
        /// <value>
        /// The acid_price.
        /// </value>
        [Required]
        [RegularExpression("(^[0-9][0-9]?(\\.[0-9][0-9]?)?$)", ErrorMessage = "Acid Price must be with in the range.(0.00 - 99.99)")]
        public Nullable<decimal> acid_price { get; set; }

        /// <summary>
        /// Gets or sets the caustic_price.
        /// </summary>
        /// <value>
        /// The caustic_price.
        /// </value>
        [Required]
        [RegularExpression("(^[0-9][0-9]?(\\.[0-9][0-9]?)?$)", ErrorMessage = "Caustic Price must be with in the range.(0.00 - 99.99)")]
        public Nullable<decimal> caustic_price { get; set; }

        /// <summary>
        /// Gets or sets the demand.
        /// </summary>
        /// <value>
        /// The demand.
        /// </value>
        [Required]
        [RegularExpression("[0-9]+(,[0-9]+)*", ErrorMessage = "Water Demand must be an integer.")]
        public string demand { get; set; }

        /// <summary>
        /// Gets or sets the first water source.
        /// </summary>
        /// <value>
        /// The first water source.
        /// </value>
        [Required]
        public string firstWaterSource { get; set; }

        /// <summary>
        /// Gets or sets the second water source.
        /// </summary>
        /// <value>
        /// The second water source.
        /// </value>
        [Required]
        public string secondWaterSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has two sources.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has two sources; otherwise, <c>false</c>.
        /// </value>
        public bool HasTwoSources { get; set; }

        /// <summary>
        /// Gets or sets the first ws percentage.
        /// </summary>
        /// <value>
        /// The first ws percentage.
        /// </value>
        public int firstWSPercentage { get; set; }

        /// <summary>
        /// Gets or sets the water source list1.
        /// </summary>
        /// <value>
        /// The water source list1.
        /// </value>
        [NotMapped]
        public List<SelectListItem> WaterSourceList1 { get; set; }

        /// <summary>
        /// Gets or sets the water source list2.
        /// </summary>
        /// <value>
        /// The water source list2.
        /// </value>
        [NotMapped]
        public List<SelectListItem> WaterSourceList2 { get; set; }
    }
}
