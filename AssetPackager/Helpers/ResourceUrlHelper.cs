using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.UI;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods for resolving resource URLs.
	/// </summary>
	public static class ResourceUrlHelper
	{
		/// <summary>
		/// Gets absolute URL of the script.
		/// </summary>
		/// <param name="page">Page to get script for.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="assemblyName">Assemble name.</param>
		/// <returns>Absolute URL of the script.</returns>
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
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
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
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
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
		public static string ResolveScriptManagerUrl(ScriptManager sm, ScriptReference sr)
		{
			MethodInfo mGetUrl = typeof (ScriptReference).GetMethod("GetUrl", BindingFlags.Instance | BindingFlags.NonPublic);
			return UrlHelper.MakeRelativePath((string) mGetUrl.Invoke(sr, new object[] {sm, sm, true}));
		}

		/// <summary>
		/// Gets absolute URL of the script stored in resources (see <see cref="ClientScriptManager.GetWebResourceUrl" />).
		/// </summary>
		/// <param name="page">Page to get script for.</param>
		/// <param name="type">The type of the resource.</param>
		/// <param name="resourceName">The fully qualified name of the resource in the assembly.</param>
		/// <returns>Absolute URL of the script.</returns>
		[SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
		public static string ResolveWebResourceUrl(Page page, Type type, string resourceName)
		{
			return UrlHelper.MakeRelativePath(page.ClientScript.GetWebResourceUrl(type, resourceName));
		}
	}
}
