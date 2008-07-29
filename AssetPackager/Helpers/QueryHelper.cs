using System;
using System.Collections.Generic;
using System.Web;
using AssetPackager.Assets;

namespace AssetPackager.Helpers
{
	public static class QueryHelper
	{
		public static Query ParseQuery()
		{
			HttpRequest request = HttpContext.Current.Request;
			string queryString = !String.IsNullOrEmpty(request["d"])
			                     	? EncryptionHelper.DecryptString(request["d"])
			                     	: HttpUtility.UrlDecode(request.QueryString.ToString());
			return ParseQueryString(queryString, request["v"]);
		}

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
