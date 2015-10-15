// -----------------------------------------------------------------------
// <copyright file="ForecastData.cs" company="RTI">
// RTI
// </copyright>
// <summary>Forecast Data</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    public class ForecastData
    {
        /// <summary>
        /// Gets or sets the average forecast data.
        /// </summary>
        /// <value>
        /// The average forecast data.
        /// </value>
        public List<water_data> AverageForecastData { get; set; }

        /// <summary>
        /// Gets or sets the maximum forecast data.
        /// </summary>
        /// <value>
        /// The maximum forecast data.
        /// </value>
        public List<water_data> MaximumForecastData { get; set; }

        public ForecastData()
        {
            this.AverageForecastData = new List<water_data>();
            this.MaximumForecastData = new List<water_data>();
        }
    }
}
