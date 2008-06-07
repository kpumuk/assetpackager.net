using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Xml;

namespace AssetPackager
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
		public static string GetCombinedScriptsUrl(string setName, string setUrls)
		{
			string relativeUrl = String.Format("~/Scripts.axd?set={0}&urls={1}", setName, setUrls);
			return ResolveAbsoluteUrl(relativeUrl);
		}

		/// <summary>
		/// Gets relative URL starting with <c>~</c> from regular URL.
		/// </summary>
		/// <param name="url">URL to parse.</param>
		/// <returns>Relative URL.</returns>
		public static string MakeRelativeUrl(string url)
		{
			url = HttpUtility.HtmlDecode(url);
			if (url[0] != '~')
			{
				if (url.StartsWith("http://") || url.StartsWith("https://"))
					url = new Uri(url).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);

				if (url.StartsWith(Context.Request.ApplicationPath))
					url = url.Substring(Context.Request.ApplicationPath.Length);

				url = (url[0] == '/' ? "~" : "~/") + url;
			}
			return url;
		}

		public static string ResolveAbsoluteUrl(string relativeUrl)
		{
			if (relativeUrl[0] != '~') return relativeUrl;

			string appPath = Context.Request.ApplicationPath == "/" ? "/" : Context.Request.ApplicationPath + "/";
			return Context.Request.Url.GetLeftPart(UriPartial.Authority) + appPath + relativeUrl.Substring(2);
		}

		/// <summary>
		/// Gets absolute URL of the script.
		/// </summary>
		/// <param name="page">Page to get script for.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="assemblyName">Assemble name.</param>
		/// <returns>Absolute URL of the script.</returns>
		public static string ResolveScriptManagerUrl(Page page, string resourceName, string assemblyName)
		{
			ScriptReference sr = new ScriptReference(resourceName, assemblyName);
			return ResolveScriptManagerUrl(page, sr);
		}

		/// <summary>
		/// Gets absolute URL of the script.
		/// </summary>
		/// <param name="page">Page to get script for.</param>
		/// <param name="sr">A <see cref="ScriptReference"/> object.</param>
		/// <returns>Absolute URL of the script.</returns>
		public static string ResolveScriptManagerUrl(Page page, ScriptReference sr)
		{
			ScriptManager sm = ScriptManager.GetCurrent(page);
			return ResolveScriptManagerUrl(sm, sr);
		}

		/// <summary>
		/// Gets absolute URL of the script.
		/// </summary>
		/// <param name="sm">A <see cref="ScriptManager" /> object.</param>
		/// <param name="sr">A <see cref="ScriptReference"/> object.</param>
		/// <returns>Absolute URL of the script.</returns>
		public static string ResolveScriptManagerUrl(ScriptManager sm, ScriptReference sr)
		{
			MethodInfo mGetUrl = typeof(ScriptReference).GetMethod("GetUrl", BindingFlags.Instance | BindingFlags.NonPublic);
			return MakeRelativeUrl((string)mGetUrl.Invoke(sr, new object[] { sm, sm, true }));
		}

		/// <summary>
		/// Gets absolute URL of the script stored in resources (see <see cref="ClientScriptManager.GetWebResourceUrl" />).
		/// </summary>
		/// <param name="page">Page to get script for.</param>
		/// <param name="t">The type of the resource.</param>
		/// <param name="resourceName">The fully qualified name of the resource in the assembly.</param>
		/// <returns>Absolute URL of the script.</returns>
		public static string ResolveGetWebResourceUrl(Page page, Type t, string resourceName)
		{
			return MakeRelativeUrl(page.ClientScript.GetWebResourceUrl(t, resourceName));
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


		/// <summary>
		/// Loads URL sets from <c>~/App_Data/FileSets.xml</c> file.
		/// </summary>
		/// <returns>List of <see cref="UrlMapSet" /> objects.</returns>
		public static List<UrlMapSet> LoadSets()
		{
			List<UrlMapSet> sets = Context.Cache["UrlMapSet"] as List<UrlMapSet>;

			if (sets == null)
			{
				string configFile = Context.Server.MapPath("~/App_Data/AssetPackager.xml");
				sets = new List<UrlMapSet>();
				using (XmlReader reader = new XmlTextReader(new StreamReader(configFile)))
				{
					reader.MoveToContent();
					while (reader.Read())
					{
						if (String.CompareOrdinal(reader.Name, "set") != 0) continue;
						string setName = reader.GetAttribute("name");
						bool setForce = Boolean.Parse(reader.GetAttribute("force") ?? "false");
						UrlMapSet UrlMapSet = new UrlMapSet(setName, setForce);

						while (reader.Read())
						{
							if (String.CompareOrdinal(reader.Name, "set") == 0) break;
							if (String.CompareOrdinal(reader.Name, "url") != 0) continue;

							string urlName = reader.GetAttribute("name");
							bool force = Boolean.Parse(reader.GetAttribute("force") ?? "false");

							string assembly = reader.GetAttribute("assembly");
							string type = reader.GetAttribute("type");
							string resourceName = reader.GetAttribute("resourceName");
							string path = reader.GetAttribute("path");

							string scriptUrl = ResolveScriptUrl(resourceName, assembly, type, path);
							UrlMapSet.Urls.Add(new UrlMap(urlName, scriptUrl, force));
						}

						sets.Add(UrlMapSet);
					}
				}
				CacheDependency dependency = new CacheDependency(configFile);
				Context.Cache.Add("UrlMapSet", sets, dependency, DateTime.Now.AddDays(30), Cache.NoSlidingExpiration,
								  CacheItemPriority.NotRemovable, null);
			}
			return sets;
		}

		private static string ResolveScriptUrl(string resourceName, string assembly, string type, string path)
		{
			string url;
			if (type != null)
			{
				Page page = Context.CurrentHandler as Page;
				if (page == null) return null;

				Type t = Type.GetType(type);
				url = ResolveGetWebResourceUrl(page, t, resourceName);
			}
			else if (assembly != null)
			{
				Page page = Context.CurrentHandler as Page;
				if (page == null) return null;

				url = ResolveScriptManagerUrl(page, resourceName, assembly);
			}
			else
				url = MakeRelativeUrl(path);
			return url;
		}

		/// <summary>
		/// Gets the <see cref="HttpContext" /> object for the current HTTP request.
		/// </summary>
		private static HttpContext Context
		{
			get { return HttpContext.Current; }
		}
	}
}