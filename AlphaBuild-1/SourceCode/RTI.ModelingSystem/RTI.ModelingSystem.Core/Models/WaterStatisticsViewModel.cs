// -----------------------------------------------------------------------
// <copyright file="WaterStatisticsViewModel.cs" company="RTI">
// RTI
// </copyright>
// <summary>Water Statistics ViewModel</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    public class WaterStatisticsViewModel
    {
        /// <summary>
        /// Gets or sets the wt average source one diaplay.
        /// </summary>
        /// <value>
        /// The wt average source one diaplay.
        /// </value>
        public double WtAvgSourceOneDiaplay { get; set; }

        /// <summary>
        /// Gets or sets the grains cu ft source one display.
        /// </summary>
        /// <value>
        /// The grains cu ft source one display.
        /// </value>
        public double GrainsCuFtSourceOneDisplay { get; set; }

        /// <summary>
        /// Gets or sets the wt standard dev source one display.
        /// </summary>
        /// <value>
        /// The wt standard dev source one display.
        /// </value>
        public double WtStdDevSourceOneDisplay { get; set; }

        /// <summary>
        /// Gets or sets the wt average source two display.
        /// </summary>
        /// <value>
        /// The wt average source two display.
        /// </value>
        public double WtAvgSourceTwoDisplay { get; set; }

        /// <summary>
        /// Gets or sets the grains cu ft source two display.
        /// </summary>
        /// <value>
        /// The grains cu ft source two display.
        /// </value>
        public double GrainsCuFtSourceTwoDisplay { get; set; }

        /// <summary>
        /// Gets or sets the wt standard dev source two display.
        /// </summary>
        /// <value>
        /// The wt standard dev source two display.
        /// </value>
        public double WtStdDevSourceTwoDisplay { get; set; }

        /// <summary>
        /// Gets or sets the wt average total display.
        /// </summary>
        /// <value>
        /// The wt average total display.
        /// </value>
        public double WtAvgTotalDisplay { get; set; }

        /// <summary>
        /// Gets or sets the grains cu ft total display.
        /// </summary>
        /// <value>
        /// The grains cu ft total display.
        /// </value>
        public double GrainsCuFtTotalDisplay { get; set; }

        /// <summary>
        /// Gets or sets the wt standard dev total display.
        /// </summary>
        /// <value>
        /// The wt standard dev total display.
        /// </value>
        public double WtStdDevTotalDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [using two sources].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [using two sources]; otherwise, <c>false</c>.
        /// </value>
        public bool usingTwoSources { get; set; }
    }
}
