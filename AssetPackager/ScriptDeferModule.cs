using System;
using System.Web;
using System.Web.UI;
using AssetPackager.Configuration;

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
			context.PostRequestHandlerExecute += PostRequestHandlerExecute;
		}

		/// <summary>
		/// Adds <see cref="ScriptDeferFilter" /> to all <c>.aspx</c> pages.
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="e">An <see cref="EventArgs" /> that contains no event data.</param>
		private void PostRequestHandlerExecute(object sender, EventArgs e)
		{
			Page page = Context.CurrentHandler as Page;
			if (page == null) return;

			ScriptManager sm = ScriptManager.GetCurrent(page);

			if (Settings.Enabled && (sm == null || !sm.IsInAsyncPostBack))
				Context.Response.Filter = new ScriptDeferFilter(Context.Response);
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