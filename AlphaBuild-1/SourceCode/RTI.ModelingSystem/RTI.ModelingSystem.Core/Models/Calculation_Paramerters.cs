// -----------------------------------------------------------------------
// <copyright file="Calculation_Paramerters.cs" company="RTI">
// RTI
// </copyright>
// <summary>Calculation Paramerters</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
	#region Usings

	using System;
	using System.Collections.Generic;
    using System.Data;

	#endregion

	/// <summary>
	/// Initializes a new instance of the <see cref="Calculation_Paramerters" /> class
	/// </summary>
	public class Calculation_Paramerters
	{
		#region Properties

        /// <summary>
        /// Gets or sets the acid concentration.
        /// </summary>
        /// <value>
        /// The acid concentration.
        /// </value>
		public double AcidConcentration { get; set; }

        /// <summary>
        /// Gets or sets the caustic concentration.
        /// </summary>
        /// <value>
        /// The caustic concentration.
        /// </value>
		public double CausticConcentration { get; set; }

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
        /// Gets or sets the amount of caustic.
        /// </summary>
        /// <value>
        /// The amount of caustic.
        /// </value>
		public double AmountOfCaustic { get; set; }

        /// <summary>
        /// Gets or sets the amount of acid.
        /// </summary>
        /// <value>
        /// The amount of acid.
        /// </value>
		public double AmountOfAcid { get; set; }

        /// <summary>
        /// Gets or sets the cation amount.
        /// </summary>
        /// <value>
        /// The cation amount.
        /// </value>
		public double CationAmount { get; set; }

        /// <summary>
        /// Gets or sets the anion amount.
        /// </summary>
        /// <value>
        /// The anion amount.
        /// </value>
		public double AnionAmount { get; set; }

        /// <summary>
        /// Gets or sets the acid price conversion.
        /// </summary>
        /// <value>
        /// The acid price conversion.
        /// </value>
		public double AcidPriceConversion { get; set; }

        /// <summary>
        /// Gets or sets the caustic price conversion.
        /// </summary>
        /// <value>
        /// The caustic price conversion.
        /// </value>
		public double CausticPriceConversion { get; set; }

        /// <summary>
        /// Gets or sets the cation cleaning price.
        /// </summary>
        /// <value>
        /// The cation cleaning price.
        /// </value>
		public double CationCleaningPrice { get; set; }

        /// <summary>
        /// Gets or sets the anion cleaning price.
        /// </summary>
        /// <value>
        /// The anion cleaning price.
        /// </value>
		public double AnionCleaningPrice { get; set; }

        /// <summary>
        /// Gets or sets the cation discount.
        /// </summary>
        /// <value>
        /// The cation discount.
        /// </value>
		public double CationDiscount { get; set; }

        /// <summary>
        /// Gets or sets the anion discount.
        /// </summary>
        /// <value>
        /// The anion discount.
        /// </value>
		public double AnionDiscount { get; set; }

        /// <summary>
        /// Gets or sets the replacement price anion.
        /// </summary>
        /// <value>
        /// The replacement price anion.
        /// </value>
		public double ReplacementPriceAnion { get; set; }

        /// <summary>
        /// Gets or sets the replacemtnt price cation.
        /// </summary>
        /// <value>
        /// The replacemtnt price cation.
        /// </value>
		public double ReplacemtntPriceCation { get; set; }

        /// <summary>
        /// Gets or sets the number of trains.
        /// </summary>
        /// <value>
        /// The number of trains.
        /// </value>
		public int NumberOfTrains { get; set; }

        /// <summary>
        /// Gets or sets the number regenerations normal ops.
        /// </summary>
        /// <value>
        /// The number regenerations normal ops.
        /// </value>
		public List<Tuple<int, double>> NumberRegenerationsNormalOps { get; set; }

        /// <summary>
        /// Gets or sets the number regenerations clean.
        /// </summary>
        /// <value>
        /// The number regenerations clean.
        /// </value>
		public List<Tuple<int, double>> NumberRegenerationsClean { get; set; }

        /// <summary>
        /// Gets or sets the with cleaning tp.
        /// </summary>
        /// <value>
        /// The with cleaning tp.
        /// </value>
		public Dictionary<DateTime, Tuple<int, double, string>> WithCleaningTP { get; set; }

        /// <summary>
        /// Gets or sets the with out cleaning tp.
        /// </summary>
        /// <value>
        /// The with out cleaning tp.
        /// </value>
		public Dictionary<DateTime, Tuple<int, double, string>> WithOutCleaningTP { get; set; }

        /// <summary>
        /// Gets or sets the train list.
        /// </summary>
        /// <value>
        /// The train list.
        /// </value>
		public DataTable TrainList { get; set; }

        /// <summary>
        /// Gets or sets the salt split.
        /// </summary>
        /// <value>
        /// The salt split.
        /// </value>
		public Dictionary<int, Tuple<double?, double?>> SaltSplit { get; set; }

        /// <summary>
        /// Gets or sets the grain forcast.
        /// </summary>
        /// <value>
        /// The grain forcast.
        /// </value>
		public Dictionary<int, double> GrainForcast { get; set; }

        /// <summary>
        /// Gets or sets the price data.
        /// </summary>
        /// <value>
        /// The price data.
        /// </value>
		public List<Price_Data> PriceData { get; set; }
		
		#endregion Properties
	}
}
