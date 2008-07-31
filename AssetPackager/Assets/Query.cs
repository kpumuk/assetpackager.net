using System.Collections.Generic;

namespace AssetPackager.Assets
{
	/// <summary>
	/// Contains query data.
	/// </summary>
	public class Query
	{
		/// <summary>
		/// Gets or sets requested <see cref="AssetList" /> object.
		/// </summary>
		public AssetList AssetList { get; set; }

		/// <summary>
		/// Gets or sets a collection of requested <see cref="Asset" /> objects.
		/// </summary>
		public ICollection<Asset> Assets { get; private set; }

		/// <summary>
		/// Gets or sets value indicating whether debug scripts requested.
		/// </summary>
		public bool IsDebug { get; set; }

		/// <summary>
		/// Gets or sets requested scripts version.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Query" /> object.
		/// </summary>
		public Query()
		{
			Assets = new List<Asset>();
		}
	}
}
