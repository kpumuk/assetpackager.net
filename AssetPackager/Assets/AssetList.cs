using System;
using System.Collections.Generic;
using AssetPackager;
using AssetPackager.Assets;
using AssetPackager.Helpers;

namespace AssetPackager.Assets
{
	/// <summary>
	/// Represents set of assets for <see cref="CombineScripts" />.
	/// </summary>
	public class AssetList
	{
		#region Public properties

		/// <summary>
		/// Gets or sets name of the assets list.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether all assets in list should be combined
		/// even if page does not require some ones.
		/// </summary>
		/// <remarks>When page does not require any assets from list, it will not be included.</remarks>
		/// <value>Default <c>false</c>.</value>
		public bool Force { get; set; }

		/// <summary>
		/// Gets or sets a type of the <see cref="AssetList" />.
		/// </summary>
		public AssetListType ListType { get; set; }

		/// <summary>
		/// Gets a list of assets in list.
		/// </summary>
		public ICollection<Asset> Assets { get; private set; }

		#endregion

		#region Initialization logic

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetList" /> class.
		/// </summary>
		/// <param name="name">Set name.</param>
		/// <param name="force">When <c>true</c> all URLs will be forced to combine (see <see cref="Force"/>).</param>
		/// <param name="listType">Short name of the asset list type (see <see cref="AssetListType" />).</param>
		public AssetList(string name, bool force, string listType)
		{
			Name = name;
			Force = force;
			ListType = AssetListTypeHelper.GetTypeByName(listType);
			Assets = new List<Asset>();
		}

		#endregion

		/// <summary>
		/// Adds forced assets to the <paramref name="assets" /> list, and then appends assets from
		/// groups, that are exists in <paramref name="assets" /> collection.
		/// </summary>
		/// <param name="assets">Assets collection.</param>
		/// <returns>Assets collection with forced and corresponding group assets added.</returns>
		public ICollection<Asset> AddForcedAssets(ICollection<Asset> assets)
		{
			// Add URLs with Force == true.
			foreach (Asset url in Assets)
				if ((Force || url.Force) && !assets.Contains(url)) assets.Add(url);

			// Get all URL groups.
			ICollection<string> groups = AssetsHelper.GetGroups(assets);

			// Add all URLs from specifiс groups.
			foreach (Asset asset in Assets)
			{
				if (groups.Contains(asset.Group) && !assets.Contains(asset))
					assets.Add(asset);
			}

			return assets;
		}

		/// <summary>
		/// Gets an <see cref="Asset" /> object for specified relative path.
		/// </summary>
		/// <param name="relativePath">Relative asset path to look for.</param>
		/// <returns><see cref="Asset" /> object or <c>null</c> when no <see cref="Asset" /> found.</returns>
		public Asset FindAsset(string relativePath)
		{
			foreach (Asset asset in Assets)
			{
				if (String.Compare(relativePath, asset.RelativePath, StringComparison.OrdinalIgnoreCase) == 0)
					return asset;
			}
			return null;
		}

		/// <summary>
		/// Gets an <see cref="Asset" /> object for specified relative path whoose name is in the
		/// <paramref name="assetNames" /> list.
		/// </summary>
		/// <param name="relativePath">Relative asset path to look for.</param>
		/// <param name="assetNames">A list of asset names to look where.</param>
		/// <returns><see cref="Asset" /> object or <c>null</c> when no <see cref="Asset" /> found.</returns>
		public Asset FindAsset(string relativePath, string[] assetNames)
		{
			foreach (Asset asset in Assets)
			{
				if (String.CompareOrdinal(relativePath, asset.RelativePath) == 0 && Array.IndexOf(assetNames, asset.Name) >= 0)
					return asset;
			}
			return null;
		}

		/// <summary>
		/// Gets a collection of <see cref="Asset" /> objects using names list.
		/// </summary>
		/// <param name="assetNames">A list of asset names.</param>
		/// <returns>A list of <see cref="Asset" /> objects.</returns>
		public ICollection<Asset> FindAssets(string[] assetNames)
		{
			List<Asset> assets = new List<Asset>();
			Array.Sort(assetNames);
			foreach (Asset asset in Assets)
			{
				if (Array.BinarySearch(assetNames, asset.Name) >= 0)
					assets.Add(asset);
			}
			return assets;
		}
	}
}