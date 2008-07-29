using System;
using System.Web;

namespace DemoProject
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			AssetPackager.Configuration.Settings.HiddenFieldName = "__ASSETPACKAGERDEMO";
		}
	}
}