// -----------------------------------------------------------------------
// <copyright file="CostAnalyzerResult.cs" company="RTI">
// RTI
// </copyright>
// <summary>Cost Analyzer Result</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    public class CostAnalyzerResult
    {
        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        /// <value>
        /// The week number.
        /// </value>
        public string WeekNumber { get; set; }

        /// <summary>
        /// Gets or sets the conductivity.
        /// </summary>
        /// <value>
        /// The conductivity.
        /// </value>
        public string Conductivity { get; set; }

        /// <summary>
        /// Gets or sets the color of the conductivity.
        /// </summary>
        /// <value>
        /// The color of the conductivity.
        /// </value>
        public string ConductivityColor { get; set; }

        /// <summary>
        /// Gets or sets the salt split before.
        /// </summary>
        /// <value>
        /// The salt split before.
        /// </value>
        public string SaltSplitBefore { get; set; }

        /// <summary>
        /// Gets or sets the salt split after.
        /// </summary>
        /// <value>
        /// The salt split after.
        /// </value>
        public string SaltSplitAfter { get; set; }

        /// <summary>
        /// Gets or sets the throughput before.
        /// </summary>
        /// <value>
        /// The throughput before.
        /// </value>
        public string ThroughputBefore { get; set; }

        /// <summary>
        /// Gets or sets the throughput after.
        /// </summary>
        /// <value>
        /// The throughput after.
        /// </value>
        public string ThroughputAfter { get; set; }

        /// <summary>
        /// Gets or sets the total weekly cost before.
        /// </summary>
        /// <value>
        /// The total weekly cost before.
        /// </value>
        public string TotalWeeklyCostBefore { get; set; }

        /// <summary>
        /// Gets or sets the total weekly cost after.
        /// </summary>
        /// <value>
        /// The total weekly cost after.
        /// </value>
        public string TotalWeeklyCostAfter { get; set; }

        /// <summary>
        /// Gets or sets the regen weekly cost before.
        /// </summary>
        /// <value>
        /// The regen weekly cost before.
        /// </value>
        public string RegenWeeklyCostBefore { get; set; }

        /// <summary>
        /// Gets or sets the regen weekly cost after.
        /// </summary>
        /// <value>
        /// The regen weekly cost after.
        /// </value>
        public string RegenWeeklyCostAfter { get; set; }

        /// <summary>
        /// Gets or sets the cleaning cost before.
        /// </summary>
        /// <value>
        /// The cleaning cost before.
        /// </value>
        public string CleaningCostBefore { get; set; }

        /// <summary>
        /// Gets or sets the cleaning cost after.
        /// </summary>
        /// <value>
        /// The cleaning cost after.
        /// </value>
        public string CleaningCostAfter { get; set; }

        /// <summary>
        /// Gets or sets the replacement cost before.
        /// </summary>
        /// <value>
        /// The replacement cost before.
        /// </value>
        public string ReplacementCostBefore { get; set; }

        /// <summary>
        /// Gets or sets the replacement cost after.
        /// </summary>
        /// <value>
        /// The replacement cost after.
        /// </value>
        public string ReplacementCostAfter { get; set; }

        /// <summary>
        /// Gets or sets the total ops cost before.
        /// </summary>
        /// <value>
        /// The total ops cost before.
        /// </value>
        public string TotalOpsCostBefore { get; set; }

        /// <summary>
        /// Gets or sets the total ops cost after.
        /// </summary>
        /// <value>
        /// The total ops cost after.
        /// </value>
        public string TotalOpsCostAfter { get; set; }

        /// <summary>
        /// Gets or sets the average cost per gal before.
        /// </summary>
        /// <value>
        /// The average cost per gal before.
        /// </value>
        public string AvgCostPerGalBefore { get; set; }

        /// <summary>
        /// Gets or sets the average cost per gal after.
        /// </summary>
        /// <value>
        /// The average cost per gal after.
        /// </value>
        public string AvgCostPerGalAfter { get; set; }
    }
}
