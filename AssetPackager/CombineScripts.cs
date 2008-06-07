using System;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AssetPackager;

namespace AssetPackager
{
	/// <summary>
	/// Looks for a script tags with <c>src</c> attribute and replaces them
	/// with a single one based on <c>~/App_Data/FileSets.xml</c> file.
	/// </summary>
	/// <seealso cref="ScriptsHandler" />
	public class CombineScripts
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
			List<UrlMapSet> sets = ScriptHelper.LoadSets();
			StringBuilder hiddenField = new StringBuilder();

			foreach (UrlMapSet UrlMapSet in sets)
			{
				int setStartPos = -1;
				List<string> names = new List<string>();

				UrlMapSet urlMapSet = UrlMapSet;
				scripts = _scriptTagsRegex.Replace(
					scripts, delegate(Match match)
					         	{
					         		string url = ScriptHelper.MakeRelativeUrl(match.Groups["url"].Value);

					         		UrlMap urlMatch = urlMapSet.FindUrl(url);
					         		if (null == urlMatch) return match.Value;

					         		// Rememer the first script tag that matched in this UrlMapSet because
					         		// this is where the combined script tag will be inserted
					         		if (setStartPos < 0) setStartPos = match.Index;

					         		names.Add(urlMatch.Name);
					         		return String.Empty;
					         	});

				// Look for forced URLs
				List<UrlMap> forcedUrls = urlMapSet.FindForcedUrlsNotInSet(names);
				names.AddRange(forcedUrls.ConvertAll<string>(delegate(UrlMap map) { return map.Name; }));
				// No scripts found
				if (names.Count == 0) continue;

				names.Sort();
				string scriptNames = String.Join(",", names.ToArray());
				string scriptsUrl = ScriptHelper.GetCombinedScriptsUrl(UrlMapSet.Name, scriptNames);
				string scriptsTag = String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", scriptsUrl);

				if (setStartPos < 0)
					scripts += scriptsTag;
				else
					scripts = scripts.Insert(setStartPos, scriptsTag);

				if (hiddenField.Length > 0) hiddenField.Append(';');
				hiddenField.AppendFormat("{0}={1}", UrlMapSet.Name, scriptNames);
			}

			hiddenField.Insert(0, "<input type='hidden' name='" + ScriptDeferFilter.SCRIPT_DEFER_HIDDEN_FIELD + "' value='");
			hiddenField.Append("' />");
			hiddenField.Insert(0, scripts);
			return hiddenField.ToString();
		}

		/// <summary>
		/// Gets the <see cref="HttpContext" /> object for the current HTTP request.
		/// </summary>
		private static HttpContext Context
		{
			get { return HttpContext.Current; }
		}

		/// <summary>
		/// Gets Pikaba application version (build number).
		/// </summary>
		public static string AppVersion
		{
			get { return (string) Context.Application["Version"] ?? "1.0.1"; }
		}
	}
}