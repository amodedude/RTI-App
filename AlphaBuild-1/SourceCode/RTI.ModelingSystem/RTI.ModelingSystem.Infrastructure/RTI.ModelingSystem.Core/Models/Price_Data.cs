// -----------------------------------------------------------------------
// <copyright file="Price_Data.cs" company="RTI">
// RTI
// </copyright>
// <summary>Price Data</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System;

    #endregion Usings

    // Class to hold each instance of the price timeseries data elements
    public class Price_Data
    {
        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        /// <value>
        /// The week number.
        /// </value>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the before price.
        /// </summary>
        /// <value>
        /// The before price.
        /// </value>
        public double BeforePrice { get; set; }

        /// <summary>
        /// Gets or sets the after price.
        /// </summary>
        /// <value>
        /// The after price.
        /// </value>
        public double AfterPrice { get; set; }
    }
}
