// -----------------------------------------------------------------------
// <copyright file="source.cs" company="RTI">
// RTI
// </copyright>
// <summary>Source Model</summary>
// -----------------------------------------------------------------------

namespace RTI.ModelingSystem.Core.DBModels
{
	#region Usings

	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	#endregion Usings

	/// <summary>
	/// source entity
	/// </summary>
	[Table("sources")]
	public class source
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="source" /> class
		/// </summary>
		public source()
		{
		}

		#region Properties

		/// <summary>
		/// sources sourceID
		/// </summary>
		[Key]
		[Required]
		public int sources_sourceID { get; set; }
		/// <summary>
		/// river property
		/// </summary>
		public string river { get; set; }
		/// <summary>
		/// city property
		/// </summary>
		public string city { get; set; }
		/// <summary>
		/// state property
		/// </summary>
		public string state { get; set; }
		/// <summary>
		/// country property
		/// </summary>
		public string country { get; set; }
		/// <summary>
		/// region property
		/// </summary>
		public string region { get; set; }
		/// <summary>
		/// measurement location
		/// </summary>
		public string measurement_location { get; set; }
		/// <summary>
		/// exact lat property
		/// </summary>
		public string exact_lat { get; set; }
		/// <summary>
		/// exact lng property
		/// </summary>
		public string exact_lng { get; set; }
		/// <summary>
		/// street number property
		/// </summary>
		public string street_number { get; set; }
		/// <summary>
		/// street name property
		/// </summary>
		public string street_name { get; set; }
		/// <summary>
		/// street lat property
		/// </summary>
		public string street_lat { get; set; }
		/// <summary>
		///  property
		/// </summary>
		public string street_lng { get; set; }
		/// <summary>
		/// feature class property
		/// </summary>
		public string feature_class { get; set; }
		/// <summary>
		/// miles from site property
		/// </summary>
		public string miles_from_site { get; set; }
		/// <summary>
		/// post code property
		/// </summary>
		public string post_code { get; set; }
		/// <summary>
		/// place name property
		/// </summary>
		public string place_name { get; set; }
		/// <summary>
		/// county number property
		/// </summary>
		public string county_number { get; set; }
		/// <summary>
		/// county name property
		/// </summary>
		public string county_name { get; set; }
		/// <summary>
		/// state name property
		/// </summary>
		public string state_name { get; set; }
		/// <summary>
		/// agency property
		/// </summary>
		public string agency { get; set; }
		/// <summary>
		/// agency id property
		/// </summary>
		public string agency_id { get; set; }
		/// <summary>
		/// full site name property
		/// </summary>
		public string full_site_name { get; set; }
		/// <summary>
		/// unique site name property
		/// </summary>
		public string unique_site_name { get; set; }

		#endregion Properties
	}
}
