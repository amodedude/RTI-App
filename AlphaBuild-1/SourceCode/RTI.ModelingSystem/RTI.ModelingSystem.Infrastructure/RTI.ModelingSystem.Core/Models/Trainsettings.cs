// -----------------------------------------------------------------------
// <copyright file="Trainsettings.cs" company="RTI">
// RTI
// </copyright>
// <summary>Train Settings</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.Models
{
    #region Usings

    using System.Web.Mvc;
    using RTI.ModelingSystem.Core.DBModels;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion Usings

    public class Trainsettings
    {
        public Trainsettings()
        {
            TrianNoList = new List<SelectListItem>();
        }

        /// <summary>
        /// Gets or sets the train.
        /// </summary>
        /// <value>
        /// The train.
        /// </value>
        public train Train { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>
        /// The customer.
        /// </value>
        public customer customer { get; set; }

        /// <summary>
        /// Gets or sets the vessls list.
        /// </summary>
        /// <value>
        /// The vessls list.
        /// </value>
        public List<vessel> VesslsList { get; set; }

        /// <summary>
        /// Gets or sets the resin list.
        /// </summary>
        /// <value>
        /// The resin list.
        /// </value>
        public List <resin_products> ResinList{ get; set; }

        /// <summary>
        /// Gets or sets the train list.
        /// </summary>
        /// <value>
        /// The train list.
        /// </value>
        public List<train> TrainList { get; set; }

        /// <summary>
        /// Gets or sets the trian no list.
        /// </summary>
        /// <value>
        /// The trian no list.
        /// </value>
        [NotMapped]
        public List<SelectListItem> TrianNoList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is cation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cation; otherwise, <c>false</c>.
        /// </value>
        public bool cation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is anion.
        /// </summary>
        /// <value>
        ///   <c>true</c> if anion; otherwise, <c>false</c>.
        /// </value>
        public bool anion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is manifold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if manifold; otherwise, <c>false</c>.
        /// </value>
        public bool manifold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is degasifier.
        /// </summary>
        /// <value>
        ///   <c>true</c> if degasifier; otherwise, <c>false</c>.
        /// </value>
        public bool Degasifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is polisher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if polisher; otherwise, <c>false</c>.
        /// </value>
        public bool Polisher { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Trainsettings"/> is redirect.
        /// </summary>
        /// <value>
        ///   <c>true</c> if redirect; otherwise, <c>false</c>.
        /// </value>
        public bool Redirect { get; set; }
    }
}