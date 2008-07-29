using System.Collections.Generic;

namespace AssetPackager.Assets
{
	public class Query
	{
		public AssetList AssetList { get; set; }
		public ICollection<Asset> Assets { get; private set; }
		public bool IsDebug { get; set; }
		public string Version { get; set; }

		public Query()
		{
			Assets = new List<Asset>();
		}
	}
}
