// -----------------------------------------------------------------------
// <copyright file="SystemSummaryViewModel.cs" company="RTI">
// RTI
// </copyright>
// <summary>System Summary ViewModel</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using RTI.ModelingSystem.Core.DBModels;
    using System.Collections.Generic;

    #endregion Usings

    public class SystemSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the water source one.
        /// </summary>
        /// <value>
        /// The water source one.
        /// </value>
        public source WaterSourceOne { get; set; }

        /// <summary>
        /// Gets or sets the water source two.
        /// </summary>
        /// <value>
        /// The water source two.
        /// </value>
        public source WaterSourceTwo { get; set; }

        /// <summary>
        /// Gets or sets the customer details.
        /// </summary>
        /// <value>
        /// The customer details.
        /// </value>
        public customer CustomerDetails { get; set; }

        /// <summary>
        /// Gets or sets the trains.
        /// </summary>
        /// <value>
        /// The trains.
        /// </value>
        public List<train> Trains { get; set; }

        /// <summary>
        /// Gets or sets the type of the customer.
        /// </summary>
        /// <value>
        /// The type of the customer.
        /// </value>
        public string CustomerType { get; set; }

        /// <summary>
        /// Gets or sets the has train details.
        /// </summary>
        /// <value>
        /// The has train details.
        /// </value>
        public string HasTrainDetails { get; set; }

        /// <summary>
        /// Gets or sets the water statistics view modeldetails.
        /// </summary>
        /// <value>
        /// The water statistics view modeldetails.
        /// </value>
        public WaterStatisticsViewModel waterStatisticsViewModeldetails { get; set; }

        /// <summary>
        /// Gets or sets the source1 statistics.
        /// </summary>
        /// <value>
        /// The source1 statistics.
        /// </value>
        public ConductivityStatistics Source1Statistics { get; set; }

        /// <summary>
        /// Gets or sets the source2 statistics.
        /// </summary>
        /// <value>
        /// The source2 statistics.
        /// </value>
        public ConductivityStatistics Source2Statistics { get; set; }

    }
}
