using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AssetPackager;
using AssetPackager.Assets;
using AssetPackager.Configuration;
using AssetPackager.Helpers;

namespace AssetPackager
{
	/// <summary>
	/// Looks for script tags with <c>src</c> attribute and replaces them
	/// with a single one based on <c>~/App_Data/AssetPackager.xml</c> file.
	/// </summary>
	/// <seealso cref="ScriptsHandler" />
	public static class CombineScripts
	{
		private static readonly Regex _scriptTagsRegex =
			new Regex(@"<script\s*src\s*=\s*""(?<url>.[^""]+)"".[^>]*>\s*</script>",
			          RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

		/// <summary>
		/// Combine script references using file sets defined in a configuration file.
		/// It will replace multiple script references using one 
		/// </summary>
		public static string CombineScriptBlocks(string scripts)
		{
			ICollection<AssetList> sets = AssetsHelper.LoadAssets();
			StringBuilder hiddenField = new StringBuilder();

			foreach (AssetList assetList in sets)
			{
				int setStartPos = -1;
				List<Asset> assets = new List<Asset>();

				AssetList list = assetList;
				scripts = _scriptTagsRegex.Replace(
					scripts, delegate(Match match)
					         	{
					         		string url = UrlHelper.MakeRelativePath(match.Groups["url"].Value);

									Asset asset = list.FindAsset(url);
									if (null == asset) return match.Value;

					         		// Remember the first script tag that matched in this UrlMapSet because
					         		// this is where the combined script tag will be inserted.
					         		if (setStartPos < 0) setStartPos = match.Index;

									assets.Add(asset);
					         		return String.Empty;
					         	});

				// Look for forced URLs.
				assetList.AddForcedAssets(assets);
				// No scripts found.
				if (assets.Count == 0) continue;

				string scriptNames = AssetsHelper.SerializeAssets(assets);
				string scriptsUrl = ScriptHelper.GetCombinedScriptsUrl(assetList.Name, scriptNames);
				string scriptsTag = String.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\" src=\"{0}\"></script>", scriptsUrl);

				if (setStartPos < 0)
					scripts += scriptsTag;
				else
					scripts = scripts.Insert(setStartPos, scriptsTag);

				if (hiddenField.Length > 0) hiddenField.Append(';');
				hiddenField.AppendFormat("{0}={1}", assetList.Name, scriptNames);
			}

			hiddenField.Insert(0, "<input type='hidden' name='" + Settings.HiddenFieldName + "' value='");
			hiddenField.Append("' />");
			hiddenField.Insert(0, scripts);
			return hiddenField.ToString();
		}
	}
}