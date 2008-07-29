using System.Configuration;

namespace AssetPackager.Configuration
{
	/// <summary>
	/// Represents custom configuration section.
	/// </summary>
	internal class AssetPackagerSection : ConfigurationSection
	{
		/// <summary>
		/// Gets or sets a value indicating whether AssetPackager.Net is enabled.
		/// </summary>
		/// <seealso cref="Settings.Enabled" />
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool) base["enabled"]; }
			set { base["enabled"] = value; }
		}

		/// <summary>
		/// Gets or sets cache key for assets list caching.
		/// </summary>
		/// <see cref="Settings.CacheKey" />
		[ConfigurationProperty("cacheKey", DefaultValue = "AssetPackager_Assets")]
		public string CacheKey
		{
			get { return (string) base["cacheKey"]; }
			set { base["cacheKey"] = value; }
		}

		/// <summary>
		/// Gets or sets cache ket format for scripts caching.
		/// </summary>
		/// <seealso cref="Settings.ScriptsCacheKeyFormat" />
		[ConfigurationProperty("scriptsCacheKeyFormat", DefaultValue = "AssetPackager_Scripts/{0}/{1}")]
		public string ScriptsCacheKeyFormat
		{
			get { return (string) base["scriptsCacheKeyFormat"]; }
			set { base["scriptsCacheKeyFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating how long in hours scripts should be cached.
		/// </summary>
		/// <seealso cref="Settings.CacheDuration" />
		[ConfigurationProperty("cacheDuration", DefaultValue = 720)]
		public int CacheDuration
		{
			get { return (int) base["cacheDuration"]; }
			set { base["cacheDuration"] = value; }
		}

		/// <summary>
		/// Gets or sets a name of the hidden field to store currently loaded assets.
		/// </summary>
		/// <seealso cref="Settings.HiddenFieldName" />
		[ConfigurationProperty("hiddenFieldName", DefaultValue = "__ASSETPACKAGER")]
		public string HiddenFieldName
		{
			get { return (string) base["hiddenFieldName"]; }
			set { base["hiddenFieldName"] = value; }
		}

		/// <summary>
		/// Gets or sets an application version.
		/// </summary>
		/// <seealso cref="Settings.AppVersion" />
		[ConfigurationProperty("appVersion", DefaultValue = "1.0.0")]
		public string AppVersion
		{
			get { return (string) base["appVersion"]; }
			set { base["appVersion"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether a query string should be encrypted.
		/// </summary>
		/// <seealso cref="Settings.EncryptQueryString" />
		[ConfigurationProperty("encryptQueryString", DefaultValue = false)]
		public bool EncryptQueryString
		{
			get { return (bool) base["encryptQueryString"]; }
			set { base["encryptQueryString"] = value; }
		}

		/// <summary>
		/// Gets an instance of the <see cref="AssetPackagerSection" /> class.
		/// </summary>
		public static AssetPackagerSection Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lockObject)
					{
						if (_instance == null)
						{
							_instance = (AssetPackagerSection) ConfigurationManager.GetSection("assetPackagerConfig") ??
							            new AssetPackagerSection();
						}
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// Returns <c>false</c> to make configuration non-readonly.
		/// </summary>
		/// <returns>Always <c>false</c>.</returns>
		public override bool IsReadOnly()
		{
			return false;
		}

		private static AssetPackagerSection _instance;
		private static readonly object _lockObject = new object();
	}
}
