using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace AssetPackager
{
	/// <summary>
	/// Combines scripts into single one and writes it to response.
	/// </summary>
	public class ScriptsHandler : IHttpHandler
	{
		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements 
		/// the <see cref="IHttpHandler" /> interface.
		/// </summary>
		/// <param name="context">A <see cref="HttpContext" /> object that provides references to 
		/// the intrinsic server objects (for example, Request, Response, Session, and Server) 
		/// used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			string setName = context.Request.QueryString["set"];
			string urls = context.Request.QueryString["urls"];
			string[] urlMaps = urls.Split(',');

			string cacheKey = "Scripts/" + setName + "=" + urls;
			byte[] encodedBytes = context.Cache[cacheKey] as byte[];
			if (encodedBytes == null)
			{

				// Find the set
				UrlMapSet set = ScriptHelper.LoadSets().Find(delegate(UrlMapSet match) { return match.Name == setName; });

				// Find the URLs requested to be rendered
				List<UrlMap> maps = set.Urls.FindAll(delegate(UrlMap map) { return Array.BinarySearch(urlMaps, map.Name) >= 0; });
				StringBuilder buffer = new StringBuilder();
				foreach (UrlMap map in maps)
					FetchScript(map.Url, buffer);


				buffer.Replace("if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();", "");
				buffer.AppendLine();
				buffer.AppendLine("if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();");

				encodedBytes = context.Request.ContentEncoding.GetBytes(buffer.ToString());
				lock (_lockObject)
				{
					if (context.Cache[cacheKey] == null)
					{
						context.Cache.Add(cacheKey, encodedBytes, null, DateTime.Now.AddDays(1),
						                  Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
					}
				}
			}

			context.Response.ContentType = "text/javascript";
			context.Response.ContentEncoding = context.Request.ContentEncoding;
			context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
			context.Response.Cache.SetExpires(DateTime.Now.AddDays(30));
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.AppendHeader("Content-Length", encodedBytes.Length.ToString());

			context.Response.OutputStream.Write(encodedBytes, 0, encodedBytes.Length);
			context.Response.Flush();
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
			buffer.AppendLine("// " + relativeUrl);

			// ReSharper disable EmptyGeneralCatchClause
			try
			{
				string fileName = Context.Server.MapPath(relativeUrl);
				if (File.Exists(fileName))
				{
					buffer.AppendLine(File.ReadAllText(fileName));
					return;
				}
			}
			catch (Exception)
			{ }
			// ReSharper restore EmptyGeneralCatchClause

			string absoluteUrl = ScriptHelper.ResolveAbsoluteUrl(relativeUrl);
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

		/// <summary>
		/// Gets the <see cref="HttpContext" /> object for the current HTTP request.
		/// </summary>
		private static HttpContext Context
		{
			get { return HttpContext.Current; }
		}

		private static readonly object _lockObject = new object();
	}
}