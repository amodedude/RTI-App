// -----------------------------------------------------------------------
// <copyright file="ConductivityStatistics.cs" company="RTI">
// RTI
// </copyright>
// <summary>Conductivity Statistics</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    public class ConductivityStatistics
    {
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        public string SourceId { get; set; }

        /// <summary>
        /// Gets or sets the current conductivity.
        /// </summary>
        /// <value>
        /// The current conductivity.
        /// </value>
        public string CurrentConductivity { get; set; }

        /// <summary>
        /// Gets or sets the percent above mean.
        /// </summary>
        /// <value>
        /// The percent above mean.
        /// </value>
        public string PercentAboveMean { get; set; }

        /// <summary>
        /// Gets or sets the standard deviation.
        /// </summary>
        /// <value>
        /// The standard deviation.
        /// </value>
        public string StandardDeviation { get; set; }

        /// <summary>
        /// Gets or sets the arithmetic mean.
        /// </summary>
        /// <value>
        /// The arithmetic mean.
        /// </value>
        public string ArithmeticMean { get; set; }

        /// <summary>
        /// Gets or sets the maximum conductivity.
        /// </summary>
        /// <value>
        /// The maximum conductivity.
        /// </value>
        public string MaxConductivity { get; set; }

        /// <summary>
        /// Gets or sets the minimum conductivity.
        /// </summary>
        /// <value>
        /// The minimum conductivity.
        /// </value>
        public string MinConductivity { get; set; }

        /// <summary>
        /// Gets or sets the median value.
        /// </summary>
        /// <value>
        /// The median value.
        /// </value>
        public string MedianValue { get; set; }

        /// <summary>
        /// Gets or sets the modal value.
        /// </summary>
        /// <value>
        /// The modal value.
        /// </value>
        public string ModalValue { get; set; }
    }
}
