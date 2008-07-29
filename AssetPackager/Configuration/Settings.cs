namespace AssetPackager.Configuration
{
	/// <summary>
	/// Contains AssetPackager.Net settings.
	/// </summary>
	public static class Settings
	{
		/// <summary>
		/// Gets or sets a value indicating whether AssetPackager.Net is enabled.
		/// </summary>
		/// <value>Default is <c>true</c>.</value>
		public static bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets cache key for assets list caching.
		/// </summary>
		/// <remarks>When it's <c>null</c> a list will not be cached.</remarks>
		/// <value>Default is <c>AssetPackager_Assets</c>.</value>
		public static string CacheKey { get; set; }
		
		/// <summary>
		/// Gets or sets cache ket format for scripts caching.
		/// </summary>
		/// <remarks>
		/// <para>Should have 2 substitutions:</para>
		/// <list>
		/// <term>{0}</term><description>Application version (see <see cref="AppVersion" />)</description>
		/// <term>{1}</term><description>Asset list name along with assets in it (for example, <c>base=a,b,c,d</c>).</description>
		/// </list>
		/// <para>When it's <c>null</c> a list will not be cached.</para>
		/// </remarks>
		/// <value>Default is <c>AssetPackager_Scripts/{0}/{1}</c>.</value>
		public static string ScriptsCacheKeyFormat { get; set; }

		/// <summary>
		/// Gets or sets a value indicating how long in hours scripts should be cached.
		/// </summary>
		/// <remarks>There are several cache options will be affected: client cache and server cache.</remarks>
		/// <value>Default is <c>720</c> hours, which corresponds to <c>30</c> days.</value>
		public static int CacheDuration { get; set; }

		/// <summary>
		/// Gets or sets a name of the hidden field to store currently loaded assets.
		/// </summary>
		/// <value>Default is <c>__ASSETPACKAGER</c>.</value>
		public static string HiddenFieldName { get; set; }

		/// <summary>
		/// Gets or sets an application version.
		/// </summary>
		/// <value>Default is <c>1.0.0</c>.</value>
		public static string AppVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a query string should be encrypted.
		/// </summary>
		/// <value>Default is <c>false</c>.</value>
		public static bool EncryptQueryString { get; set; }

		/// <summary>
		/// Initializes settings with default values.
		/// </summary>
		static Settings()
		{
			Enabled = true;
			CacheKey = "AssetPackager_Assets";
			ScriptsCacheKeyFormat = "AssetPackager_Scripts/{0}/{1}";
			CacheDuration = 24 * 30;
			HiddenFieldName = "__ASSETPACKAGER";
			AppVersion = "1.0.0";
			EncryptQueryString = false;
		}
	}
}