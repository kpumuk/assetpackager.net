﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
			string relativeUrl = String.Format(CultureInfo.InvariantCulture, "~/Scripts.axd?set={0}&urls={1}&v={2}", setName, setUrls, Settings.AppVersion);
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
	}
}