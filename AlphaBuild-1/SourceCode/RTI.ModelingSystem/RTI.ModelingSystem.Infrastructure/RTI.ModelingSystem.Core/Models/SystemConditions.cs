// -----------------------------------------------------------------------
// <copyright file="SystemConditions.cs" company="RTI">
// RTI
// </copyright>
// <summary>System Conditions</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
	public class SystemConditions
	{
		/// <summary>
		/// The salt split today before
		/// </summary>
		public double SaltSplitTodayBefore { get; set; }

		/// <summary>
		/// The regen time average before
		/// </summary>
		public double RegenTimeAverageBefore { get; set; }

		/// <summary>
		/// The regens per week average before
		/// </summary>
		public double RegensPerWeekAverageBefore { get; set; }

		/// <summary>
		/// The hours per run average before
		/// </summary>
		public double HoursPerRunAverageBefore { get; set; }

		/// <summary>
		/// The throughputavg before
		/// </summary>
		public double ThroughputAverageBefore { get; set; }

		/// <summary>
		/// The salt split today after
		/// </summary>
		public double SaltSplitTodayAfter { get; set; }

		/// <summary>
		/// The regen time average after
		/// </summary>
		public double RegenTimeAverageAfter { get; set; }

		/// <summary>
		/// The regens per week average after
		/// </summary>
		public double RegensPerWeekAverageAfter { get; set; }

		/// <summary>
		/// The hours per run average after
		/// </summary>
		public double HoursPerRunAverageAfter { get; set; }

		/// <summary>
		/// The throughputavg after
		/// </summary>
		public double ThroughputAverageAfter { get; set; }

        /// <summary>
        /// The Currently Selected Train
        /// </summary>
        public string TrainDisplay { get; set; }
	}
}