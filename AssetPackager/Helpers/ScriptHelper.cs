using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Configuration;
using AssetPackager.Configuration;
using AssetPackager.Helpers;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods for scripts processing.
	/// </summary>
	public static class ScriptHelper
	{
		/// <summary>
		/// Gets an URL of the script handler which combines scripts to a single one.
		/// </summary>
		/// <param name="setName">URLs set name.</param>
		/// <param name="setUrls">URL names to include to combined script.</param>
		/// <returns>Absolute URL of the script handler.</returns>
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
		public static string GetCombinedScriptsUrl(string setName, string[] setUrls)
		{
			return GetCombinedScriptsUrl(setName, String.Join(",", setUrls));
		}

		/// <summary>
		/// Gets an URL of the script handler which combines scripts to a single one.
		/// </summary>
		/// <param name="setName">URLs set name.</param>
		/// <param name="setUrls">URL names to include to combined script, divided with <c>,</c>.</param>
		/// <returns>Absolute URL of the script handler.</returns>
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
		public static string GetCombinedScriptsUrl(string setName, string setUrls)
		{
			string mode = IsDebuggingEnabled ? "d" : "r";
			string queryStringFormat = "set={0}&urls={1}&m={2}";
			string queryString = String.Format(CultureInfo.InvariantCulture, queryStringFormat, setName,
			                                   setUrls, mode);
			if (Settings.EncryptQueryString)
				queryString = "d=" + EncryptionHelper.EncryptString(queryString);

			string scriptPathFormat = "~/Scripts.axd?{0}&v={1}";
			string relativeUrl = String.Format(CultureInfo.InvariantCulture, scriptPathFormat,
											   queryString, Settings.AppVersion);
			return UrlHelper.ResolveAbsoluteUrl(relativeUrl);
		}

		/// <summary>
		/// Parses serialized list of loaded script.
		/// </summary>
		/// <param name="value">Serialized list of loaded scripts.</param>
		/// <returns>List of set names with URL names in them.</returns>
		public static Dictionary<string, string[]> ParseScriptsHiddenField(string value)
		{
			Dictionary<string, string[]>  loadedScripts = new Dictionary<string, string[]>();
			if (!String.IsNullOrEmpty(value))
			{
				string[] sets = value.Split(';');
				foreach (string set in sets)
				{
					string[] data = set.Split('=');
					loadedScripts[data[0]] = data[1].Split(',');
				}
			}
			return loadedScripts;
		}

		public static bool IsDebuggingEnabled
		{
			get
			{
				if (_isDebuggingEnabled == null)
				{
					DeploymentSection section = (DeploymentSection)WebConfigurationManager.GetSection("system.web/deployment");
					bool retail = section.Retail;
					if (retail)
						_isDebuggingEnabled = false;
					else
					{
						CompilationSection webApplicationSection =
							(CompilationSection)WebConfigurationManager.GetWebApplicationSection("system.web/compilation");
						bool debug = webApplicationSection.Debug;
						_isDebuggingEnabled = debug;
					}
				}
				return (bool)_isDebuggingEnabled;
			}
		}
		private static bool? _isDebuggingEnabled;
	}
}