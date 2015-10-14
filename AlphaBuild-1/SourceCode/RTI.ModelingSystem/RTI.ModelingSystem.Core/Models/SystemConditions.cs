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
        public double SaltSplitTodayBefore;

        /// <summary>
        /// The regen time average before
        /// </summary>
        public double RegenTimeAverageBefore;

        /// <summary>
        /// The regens per week average before
        /// </summary>
        public double RegensPerWeekAverageBefore;

        /// <summary>
        /// The hours per run average before
        /// </summary>
        public double HoursPerRunAverageBefore;

        /// <summary>
        /// The throughputavg before
        /// </summary>
        public double ThroughputAverageBefore;

        /// <summary>
        /// The salt split today after
        /// </summary>
        public double SaltSplitTodayAfter;

        /// <summary>
        /// The regen time average after
        /// </summary>
        public double RegenTimeAverageAfter;

        /// <summary>
        /// The regens per week average after
        /// </summary>
        public double RegensPerWeekAverageAfter;

        /// <summary>
        /// The hours per run average after
        /// </summary>
        public double HoursPerRunAverageAfter;

        /// <summary>
        /// The throughputavg after
        /// </summary>
        public double ThroughputAverageAfter;
    }
}