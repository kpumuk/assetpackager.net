using System;
using System.Web;

namespace AssetPackager
{
	/// <summary>
	/// Adds <see cref="ScriptDeferFilter" /> to all <c>.aspx</c> pages.
	/// </summary>
	public class ScriptDeferModule : IHttpModule
	{
		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">A <see cref="HttpApplication" /> that provides access to the 
		/// methods, properties, and events common to all application objects within an 
		/// ASP.NET application.</param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		/// <summary>
		/// Adds <see cref="ScriptDeferFilter" /> to all <c>.aspx</c> pages.
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="e">An <see cref="EventArgs" /> that contains no event data.</param>
		private void BeginRequest(object sender, EventArgs e)
		{
			if (Context.Request.AppRelativeCurrentExecutionFilePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) &&
				String.Compare(Context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) == 0)
			{
				Context.Response.Filter = new ScriptDeferFilter(Context.Response);
			}
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements 
		/// <see cref="IHttpModule" />.
		/// </summary>
		public void Dispose()
		{
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