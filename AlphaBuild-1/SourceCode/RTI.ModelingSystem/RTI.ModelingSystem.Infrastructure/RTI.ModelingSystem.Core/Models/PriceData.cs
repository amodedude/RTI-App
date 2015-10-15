// -----------------------------------------------------------------------
// <copyright file="PriceData.cs" company="RTI">
// RTI
// </copyright>
// <summary>Price Data</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Data;

	#endregion Usings

	/// <summary>
	/// Price Data model
	/// </summary>
	public class PriceData
	{
        public double numDaysMeetingDemand_Clean { get; set; }

        public double numDaysMeetingDemand_NormalOps { get; set; }
        /// <summary>
        /// Gets or sets the clean throughput.
        /// </summary>
        /// <value>
        /// The clean throughput.
        /// </value>
		public Dictionary<DateTime, Tuple<int, double, string>> CleanThroughput { get; set; }

        /// <summary>
        /// Gets or sets the normal ops throughput.
        /// </summary>
        /// <value>
        /// The normal ops throughput.
        /// </value>
        public Dictionary<DateTime, Tuple<int, double, string>> NormalOpsThroughput { get; set; }

        /// <summary>
        /// Gets or sets the regens per week clean.
        /// </summary>
        /// <value>
        /// The regens per week clean.
        /// </value>
        public List<Tuple<int, double>> RegensPerWeekClean { get; set; }

        /// <summary>
        /// Gets or sets the regens per week normal ops.
        /// </summary>
        /// <value>
        /// The regens per week normal ops.
        /// </value>
        public List<Tuple<int, double>> RegensPerWeekNormalOps { get; set; }

        /// <summary>
        /// Gets or sets the train list.
        /// </summary>
        /// <value>
        /// The train list.
        /// </value>
        public DataTable TrainList { get; set; }

        /// <summary>
        /// Gets or sets the number trains.
        /// </summary>
        /// <value>
        /// The number trains.
        /// </value>
        public int NumberTrains { get; set; }

        /// <summary>
        /// Gets or sets the number regens.
        /// </summary>
        /// <value>
        /// The number regens.
        /// </value>
        public double NumberRegens { get; set; }

        /// <summary>
        /// Gets or sets the acid price.
        /// </summary>
        /// <value>
        /// The acid price.
        /// </value>
        public double AcidPrice { get; set; }

        /// <summary>
        /// Gets or sets the caustic price.
        /// </summary>
        /// <value>
        /// The caustic price.
        /// </value>
        public double CausticPrice { get; set; }

        /// <summary>
        /// Gets or sets the grain forcast.
        /// </summary>
        /// <value>
        /// The grain forcast.
        /// </value>
        public Dictionary<int, double> GrainForcast { get; set; }

        /// <summary>
        /// Gets or sets the salt split.
        /// </summary>
        /// <value>
        /// The salt split.
        /// </value>
        public Dictionary<int, Tuple<double?, double?>> SaltSplit { get; set; }

        /// <summary>
        /// Gets or sets the amount cation.
        /// </summary>
        /// <value>
        /// The amount cation.
        /// </value>
        public double AmountCation { get; set; }

        /// <summary>
        /// Gets or sets the amount anion.
        /// </summary>
        /// <value>
        /// The amount anion.
        /// </value>
        public double AmountAnion { get; set; }

        /// <summary>
        /// Gets or sets the acid usage.
        /// </summary>
        /// <value>
        /// The acid usage.
        /// </value>
        public double AcidUsage { get; set; }

        /// <summary>
        /// Gets or sets the caustic usage.
        /// </summary>
        /// <value>
        /// The caustic usage.
        /// </value>
        public double CausticUsage { get; set; }

        /// <summary>
        /// Gets or sets the number weeks.
        /// </summary>
        /// <value>
        /// The number weeks.
        /// </value>
        public double NumberWeeks { get; set; }

        /// <summary>
        /// Gets or sets the regen time average before.
        /// </summary>
        /// <value>
        /// The regen time average before.
        /// </value>
        public double RegenTimeAverageBefore { get; set; }

        /// <summary>
        /// Gets or sets the regen time average after.
        /// </summary>
        /// <value>
        /// The regen time average after.
        /// </value>
        public double RegenTimeAverageAfter { get; set; }

        /// <summary>
        /// Gets or sets the regens per week average before.
        /// </summary>
        /// <value>
        /// The regens per week average before.
        /// </value>
        public double RegensPerWeekAverageBefore { get; set; }

        /// <summary>
        /// Gets or sets the regens per week average after.
        /// </summary>
        /// <value>
        /// The regens per week average after.
        /// </value>
        public double RegensPerWeekAverageAfter { get; set; }

        /// <summary>
        /// Gets or sets the hours per run average before.
        /// </summary>
        /// <value>
        /// The hours per run average before.
        /// </value>
        public double HoursPerRunAverageBefore { get; set; }

        /// <summary>
        /// Gets or sets the hours per run average after.
        /// </summary>
        /// <value>
        /// The hours per run average after.
        /// </value>
        public double HoursPerRunAverageAfter { get; set; }

        /// <summary>
        /// Gets or sets the throughput average before.
        /// </summary>
        /// <value>
        /// The throughput average before.
        /// </value>
        public double ThroughputAverageBefore { get; set; }

        /// <summary>
        /// Gets or sets the throughput average after.
        /// </summary>
        /// <value>
        /// The throughput average after.
        /// </value>
        public double ThroughputAverageAfter { get; set; }

        public double cleaningPriceCation { get; set; }

        public double cleaningPriceAnion { get; set; }

        public double cationDiscountPercent { get; set; }

        public double anionDiscountPercent { get; set; }

        public double replacePriceCation { get; set; }

        public double replacePirceAnion { get; set; }

        public double causticConcentration { get; set; }

        public double acidConcentratoin { get; set; }
	}
}