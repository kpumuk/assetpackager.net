using System;
using System.Collections.Generic;
using System.Web;
using AssetPackager.Assets;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods to work with query string.
	/// </summary>
	public static class QueryHelper
	{
		/// <summary>
		/// Parses query string and returns an initialized <see cref="Query" /> object.
		/// </summary>
		/// <remarks>When query string is encrypted, it also decrypts it.</remarks>
		/// <returns>A <see cref="Query" /> object that contains query data.</returns>
		public static Query ParseQuery()
		{
			HttpRequest request = HttpContext.Current.Request;
			string queryString = !String.IsNullOrEmpty(request["d"])
			                     	? EncryptionHelper.DecryptString(request["d"])
			                     	: HttpUtility.UrlDecode(request.QueryString.ToString());
			return ParseQueryString(queryString, request["v"]);
		}

		/// <summary>
		/// Internal method that parses query string.
		/// </summary>
		/// <param name="queryString">Query string to parse.</param>
		/// <param name="version">Requested script version.</param>
		/// <returns>A <see cref="Query" /> object that contains query data.</returns>
		private static Query ParseQueryString(string queryString, string version)
		{
			Query query = new Query();
			string[] parts = queryString.Split('&');
			ICollection<AssetList> assetLists = AssetsHelper.LoadAssets();
			string urls = null;
			foreach (string part in parts)
			{
				string[] param = part.Split('=');
				switch (param[0])
				{
					case "set":
						query.AssetList = AssetsHelper.FindAssetList(assetLists, param[1]);
						if (param[1] == "fake") query.AssetList = new AssetList("fake", false, "js");
						break;
					case "urls":
						urls = param[1];
						break;
					case "m":
						query.IsDebug = String.Compare(param[1], "d", StringComparison.OrdinalIgnoreCase) == 0;
						break;
				}
			}
			if (!String.IsNullOrEmpty(urls))
				foreach (Asset asset in query.AssetList.FindAssets(urls.Split(',')))
					query.Assets.Add(asset);
			query.Version = version;
			return query;
		}
	}
}
