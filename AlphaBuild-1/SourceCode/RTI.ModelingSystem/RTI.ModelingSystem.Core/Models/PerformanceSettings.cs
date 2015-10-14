﻿// -----------------------------------------------------------------------
// <copyright file="PerformanceSettings.cs" company="RTI">
// RTI
// </copyright>
// <summary>Performance Settings</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System.Collections.Generic;
    using System.Web.Mvc;

    #endregion Usings

    public class PerformanceSettings
    {
        /// <summary>
        /// Gets or sets the train list.
        /// </summary>
        /// <value>
        /// The train list.
        /// </value>
        public List<SelectListItem> trainList { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the GPM.
        /// </summary>
        /// <value>
        /// The GPM.
        /// </value>
        public int GPM { get; set; }

        /// <summary>
        /// Gets or sets the selected train.
        /// </summary>
        /// <value>
        /// The selected train.
        /// </value>
        public int selectedTrain { get; set; }
    }
}
