// -----------------------------------------------------------------------
// <copyright file="ConductivityViewModel.cs" company="RTI">
// RTI
// </copyright>
// <summary>Conductivity ViewModel</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System.Collections.Generic;
    using RTI.ModelingSystem.Core.DBModels;

    #endregion Usings

    public class ConductivityViewModel
    {
        /// <summary>
        /// Gets or sets the source1waterdata.
        /// </summary>
        /// <value>
        /// The source1waterdata.
        /// </value>
        public List<water_data> Source1waterdata { get; set; }

        /// <summary>
        /// Gets or sets the source2waterdata.
        /// </summary>
        /// <value>
        /// The source2waterdata.
        /// </value>
        public List<water_data> Source2waterdata { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is female.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is female; otherwise, <c>false</c>.
        /// </value>
        public bool IsFemale { get; set; }
    }
}
