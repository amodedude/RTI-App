// -----------------------------------------------------------------------
// <copyright file="customer.cs" company="RTI">
// RTI
// </copyright>
// <summary>Customer Model</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace RTI.ModelingSystem.Core.DBModels
{
    public partial class customer
    {
        public customer()
        {
            this.TrainList = new List<SelectListItem>();
            this.customerList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.TrainList = new List<SelectListItem>();
        }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        [Key]
        [Required]
        public long customerID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        public string company { get; set; }
        /// <summary>
        /// Gets or sets the plant.
        /// </summary>
        /// <value>
        /// The plant.
        /// </value>
        [Required]
        public string plant { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [Required]
        public string city { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [Required]
        public string state { get; set; }
        /// <summary>
        /// Gets or sets the acid_price.
        /// </summary>
        /// <value>
        /// The acid_price.
        /// </value>
        public Nullable<decimal> acid_price { get; set; }
        /// <summary>
        /// Gets or sets the caustic_price.
        /// </summary>
        /// <value>
        /// The caustic_price.
        /// </value>
        public Nullable<decimal> caustic_price { get; set; }
        /// <summary>
        /// Gets or sets the demand.
        /// </summary>
        /// <value>
        /// The demand.
        /// </value>
        public Nullable<int> demand { get; set; }
        /// <summary>
        /// Gets or sets the num_trains.
        /// </summary>
        /// <value>
        /// The num_trains.
        /// </value>
        [Required]
        public Nullable<long> num_trains { get; set; }
        /// <summary>
        /// Gets or sets the date_added.
        /// </summary>
        /// <value>
        /// The date_added.
        /// </value>
        public string date_added { get; set; }
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public string notes { get; set; }
        //public virtual ICollection<customer_water> customer_water { get; set; }
        /// <summary>
        /// Gets or sets the customer list.
        /// </summary>
        /// <value>
        /// The customer list.
        /// </value>
        [NotMapped]
        public List<SelectListItem> customerList { get; set; }
        /// <summary>
        /// Gets or sets the state list.
        /// </summary>
        /// <value>
        /// The state list.
        /// </value>
        [NotMapped]
        public List<SelectListItem> StateList { get; set; }
        /// <summary>
        /// Gets or sets the city list.
        /// </summary>
        /// <value>
        /// The city list.
        /// </value>
        [NotMapped]
        public List<SelectListItem> CityList { get; set; }
        /// <summary>
        /// Gets or sets the train list.
        /// </summary>
        /// <value>
        /// The train list.
        /// </value>
        [NotMapped]
        public List<SelectListItem> TrainList { get; set; }
    }
}
