using System;
using System.Collections.Generic;
using System.Web.UI;
using AssetPackager.Assets;
using AssetPackager.Configuration;
using AssetPackager.Helpers;

namespace AssetPackager.WebControls
{
	/// <summary>
	/// Replaces already loaded scripts with empty script reference.
	/// </summary>
	public class AssetPackagerScriptManager : ScriptManager
	{
		/// <summary>
		/// Checks if script reference contains already loaded script link and replaces
		/// it with empty script.
		/// </summary>
		/// <param name="e">A <see cref="ScriptReferenceEventArgs" /> that contains event data.</param>
		protected override void OnResolveScriptReference(ScriptReferenceEventArgs e)
		{
			base.OnResolveScriptReference(e);

			if (IsInAsyncPostBack) ProcessLoadedScripts(e.Script);
		}

		/// <summary>
		/// Replaces already loaded scripts with fake script.
		/// </summary>
		/// <param name="script">A <see cref="ScriptReference" /> object.</param>
		private void ProcessLoadedScripts(ScriptReference script)
		{
			ICollection<AssetList> assetLists = AssetsHelper.LoadAssets();

			string scriptUrl = ResourceUrlHelper.ResolveScriptManagerUrl(this, script);

			foreach (KeyValuePair<string, string[]> loadedSet in LoadedScripts)
			{
				string setName = loadedSet.Key;
				string[] setUrls = loadedSet.Value;
				AssetList list = AssetsHelper.FindAssetList(assetLists, setName);
				if (list == null) continue;

				Asset asset = list.FindAsset(scriptUrl, setUrls);
				if (asset == null) continue;
				script.Name = "";
				script.Assembly = "";
				script.Path = ScriptHelper.GetCombinedScriptsUrl("fake", String.Empty);
				break;
			}
		}

		/// <summary>
		/// Gets a list of loaded scripts based on request parameter.
		/// </summary>
		private Dictionary<string, string[]> LoadedScripts
		{
			get
			{
				if (_loadedScripts == null)
				{
					string value = Page.Request.Params[Settings.HiddenFieldName];
					_loadedScripts = ScriptHelper.ParseScriptsHiddenField(value);
				}
				return _loadedScripts;
			}
		}
		private Dictionary<string, string[]> _loadedScripts;
	}
}