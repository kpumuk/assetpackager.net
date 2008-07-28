using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using AssetPackager.Assets;
using AssetPackager.Configuration;
using AssetPackager.WebControls;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods for <see cref="AssetList" /> and <see cref="Asset" /> classes.
	/// </summary>
	public static class AssetsHelper
	{
		/// <summary>
		/// Load assets from <c>~/App_Data/AssetPackager.xml</c> XML file.
		/// </summary>
		/// <returns>List of <see cref="AssetList" /> objects.</returns>
		public static ICollection<AssetList> LoadAssets()
		{
			string configFile = HttpContext.Current.Server.MapPath("~/App_Data/AssetPackager.xml");
			CacheDependency dependency = new CacheDependency(configFile);
			return (List<AssetList>) CacheHelper.GetData(Settings.CacheKey, dependency, DateTime.Now.AddDays(30),
			                                             CacheItemPriority.NotRemovable,
			                                             delegate { return LoadAssetLists(configFile); });
		}

		/// <summary>
		/// Searches asset list in a collection of <see cref="AssetList" /> objects by name.
		/// </summary>
		/// <param name="assetLists">A collection of <see cref="AssetList" /> objects.</param>
		/// <param name="name">Asset list name.</param>
		/// <returns><see cref="AssetList" /> object or <c>null</c> when no one found.</returns>
		public static AssetList FindAssetList(ICollection<AssetList> assetLists, string name)
		{
			foreach (AssetList assetList in assetLists)
			{
				if (String.Compare(name, assetList.Name, StringComparison.OrdinalIgnoreCase) == 0)
					return assetList;
			}
			return null;
		}

		/// <summary>
		/// Gets a list of groups that are defined for URLs in the specified list.
		/// </summary>
		/// <param name="urls">URLs list.</param>
		/// <returns>List of groups.</returns>
		public static ICollection<string> GetGroups(ICollection<Asset> urls)
		{
			List<string> groups = new List<string>();
			foreach (Asset url in urls)
			{
				if (!String.IsNullOrEmpty(url.Group) && !groups.Contains(url.Group))
					groups.Add(url.Group);
			}
			return groups;
		}

		/// <summary>
		/// Serializes a list of <see cref="Asset" /> objects into a string using URL names.
		/// </summary>
		/// <param name="assets">List of <see cref="Asset" /> objects to serialize.</param>
		/// <returns>URL names, separated with comma.</returns>
		public static string SerializeAssets(ICollection<Asset> assets)
		{
			List<string> names = new List<string>();
			foreach (Asset asset in assets)
				names.Add(asset.Name);
			names.Sort();
			return String.Join(",", names.ToArray());
		}

		#region Private helper methods

		private static List<AssetList> LoadAssetLists(string configFile)
		{
			List<AssetList> sets = new List<AssetList>();
			using (XmlReader reader = new XmlTextReader(new StreamReader(configFile)))
			{
				reader.MoveToContent();
				while (reader.Read())
				{
					if (String.CompareOrdinal(reader.Name, "set") != 0) continue;
					AssetList assetList = ReadAssetList(reader);
					sets.Add(assetList);
				}
			}
			return sets;
		}

		private static AssetList ReadAssetList(XmlReader reader)
		{
			string setName = reader.GetAttribute("name");
			bool setForce = Boolean.Parse(reader.GetAttribute("force") ?? "false");
			string type = reader.GetAttribute("type");
			AssetList list = new AssetList(setName, setForce, type);

			while (reader.Read())
			{
				if (String.CompareOrdinal(reader.Name, "set") == 0) break;
				if (String.CompareOrdinal(reader.Name, "url") != 0) continue;

				Asset asset = ReadAsset(reader);

				list.Assets.Add(asset);
			}

			return list;
		}

		private static Asset ReadAsset(XmlReader reader)
		{
			string urlName = reader.GetAttribute("name");
			bool force = Boolean.Parse(reader.GetAttribute("force") ?? "false");
			string group = reader.GetAttribute("group");

			string assembly = reader.GetAttribute("assembly");
			string type = reader.GetAttribute("type");
			string resourceName = reader.GetAttribute("resourceName");
			string path = reader.GetAttribute("path");

			string scriptUrl = ResolveAssetUrl(resourceName, assembly, type, path);
			return new Asset(urlName, scriptUrl, group, force);
		}

		private static string ResolveAssetUrl(string resourceName, string assembly, string type, string path)
		{
			string url;
			if (type != null)
			{
				Type t = Type.GetType(type);
				url = ResourceUrlHelper.ResolveWebResourceUrl(GetPage(), t, resourceName);
			}
			else
				url = assembly != null
				      	? ResourceUrlHelper.ResolveScriptManagerUrl(GetPage(), resourceName, assembly)
				      	: UrlHelper.MakeRelativePath(path);
			return url;
		}

		private static Page GetPage()
		{
			Page page = HttpContext.Current.CurrentHandler as Page;
			if (page != null) return page;

			page = new Page();
			HtmlForm form = new HtmlForm();
			page.Controls.Add(form);
			AssetPackagerScriptManager sm = new AssetPackagerScriptManager();
			form.Controls.Add(sm);
			return page;
		}

		#endregion
	}
}
