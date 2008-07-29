using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using AssetPackager.Assets;
using AssetPackager.Configuration;
using AssetPackager.Helpers;

namespace AssetPackager
{
	/// <summary>
	/// Combines scripts into single one and writes it to response.
	/// </summary>
	public class ScriptsHandler : IHttpHandler
	{
		private static readonly Regex _notifyScriptRegex =
			new Regex(@"if\s*(\s*typeof\s*\(?Sys\)?\s*!==\s*['""]undefined['""])\s*Sys.Application.notifyScriptLoaded(\s*);",
					  RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements 
		/// the <see cref="IHttpHandler" /> interface.
		/// </summary>
		/// <param name="context">A <see cref="HttpContext" /> object that provides references to 
		/// the intrinsic server objects (for example, Request, Response, Session, and Server) 
		/// used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			Query query = QueryHelper.ParseQuery();

			string cacheKey = String.Format(CultureInfo.InvariantCulture,
			                                Settings.ScriptsCacheKeyFormat,
			                                Settings.AppVersion,
											query.AssetList + "=" + AssetsHelper.SerializeAssets(query.Assets));
			byte[] encodedBytes = (byte[]) CacheHelper.GetData(cacheKey, null,
			                                                   DateTime.Now.AddHours(Settings.CacheDuration),
			                                                   CacheItemPriority.NotRemovable,
			                                                   delegate { return CombineScripts(query.Assets); });

			context.Response.ContentType = AssetListTypeHelper.GetMimeType(query.AssetList.ListType);
			context.Response.ContentEncoding = context.Request.ContentEncoding;
			context.Response.Cache.SetMaxAge(TimeSpan.FromHours(Settings.CacheDuration));
			context.Response.Cache.SetExpires(DateTime.Now.AddHours(Settings.CacheDuration));
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.AppendHeader("Content-Length", encodedBytes.Length.ToString(CultureInfo.InvariantCulture));

			context.Response.OutputStream.Write(encodedBytes, 0, encodedBytes.Length);
			context.Response.Flush();
		}

		private static object CombineScripts(IEnumerable<Asset> assets)
		{
			StringBuilder buffer = new StringBuilder();
			foreach (Asset asset in assets)
				FetchScript(asset.RelativePath, buffer);

			string response = _notifyScriptRegex.Replace(buffer.ToString(), String.Empty);
			response += "\nif(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();";

			return HttpContext.Current.Request.ContentEncoding.GetBytes(response);
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <see cref="IHttpHandler" /> instance is reusable; otherwise, <c>false</c>.
		/// </returns>
		/// <value>Always <c>true</c>.</value>
		public bool IsReusable
		{
			get { return true; }
		}

		/// <summary>
		/// Retrieves script text by relative URL.
		/// </summary>
		/// <param name="relativeUrl">URL of the script.</param>
		/// <param name="buffer">A <see cref="StringBuilder" /> object to write script to.</param>
		private static void FetchScript(string relativeUrl, StringBuilder buffer)
		{
			buffer.AppendLine("/***** " + relativeUrl + " *****/");

			try
			{
				string fileName = HttpContext.Current.Server.MapPath(relativeUrl);
				fileName = FindFile(fileName);
				if (File.Exists(fileName))
				{
					buffer.AppendLine(File.ReadAllText(fileName));
					return;
				}
			}
			catch (HttpException)
			{ } // Invalid file path, so try to retrieve via HTTP

			string absoluteUrl = UrlHelper.ResolveAbsoluteUrl(relativeUrl);
			HttpWebRequest request = CreateHttpWebRequest(absoluteUrl);
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseContent = reader.ReadToEnd();
					buffer.AppendLine(responseContent);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="HttpWebRequest" />.
		/// </summary>
		/// <param name="url">Resource URL.</param>
		/// <returns>A <see cref="HttpWebRequest" /> object.</returns>
		private static HttpWebRequest CreateHttpWebRequest(string url)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			request.Headers.Add("Accept-Encoding", "gzip");
			request.AutomaticDecompression = DecompressionMethods.GZip;
			request.MaximumAutomaticRedirections = 2;
			request.MaximumResponseHeadersLength = 4 * 1024;
			request.ReadWriteTimeout = 1 * 1000;
			request.Timeout = 5 * 1000;

			return request;
		}

		private static string FindFile(string path)
		{
			if (ScriptHelper.IsDebuggingEnabled)
			{
				string debugFileName = GetDebugPath(path);
				return debugFileName;
			}
			return path;
		}

		private static string GetDebugPath(string releasePath)
		{
			string path, query;
			if (releasePath.IndexOf('?') >= 0)
			{
				int index = releasePath.IndexOf('?');
				path = releasePath.Substring(0, index);
				query = releasePath.Substring(index);
			}
			else
			{
				path = releasePath;
				query = String.Empty;
			}

			return ReplaceExtension(path) + query;
		}

		private static string ReplaceExtension(string path)
		{
			int extensionIndex = path.LastIndexOf('.');
			string extension = path.Substring(extensionIndex);
			return (path.Substring(0, extensionIndex) + ".debug" + extension);
		}
	}
}