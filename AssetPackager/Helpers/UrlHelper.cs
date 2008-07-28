using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods for generic URLs processing.
	/// </summary>
	public static class UrlHelper
	{
		/// <summary>
		/// Gets relative path starting with <c>~</c> from regular URL.
		/// </summary>
		/// <param name="url">URL to parse.</param>
		/// <returns>Relative path.</returns>
		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings")]
		public static string MakeRelativePath(string url)
		{
			url = HttpUtility.HtmlDecode(url);
			if (url[0] != '~')
			{
				if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
					url = new Uri(url).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);

				if (url.StartsWith(HttpContext.Current.Request.ApplicationPath, StringComparison.OrdinalIgnoreCase))
					url = url.Substring(HttpContext.Current.Request.ApplicationPath.Length);

				url = (url[0] == '/' ? "~" : "~/") + url;
			}
			return url;
		}

		/// <summary>
		/// Converts relative path starting with <c>~</c> to absolute URL.
		/// </summary>
		/// <param name="relativePath">Relative path.</param>
		/// <returns>Absolute URL.</returns>
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
		public static string ResolveAbsoluteUrl(string relativePath)
		{
			if (relativePath[0] != '~') return relativePath;

			string appPath = HttpContext.Current.Request.ApplicationPath == "/"
			                 	? "/"
			                 	: HttpContext.Current.Request.ApplicationPath + "/";
			return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + appPath + relativePath.Substring(2);
		}
	}
}
