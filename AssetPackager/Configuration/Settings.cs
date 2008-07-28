namespace AssetPackager.Configuration
{
	public static class Settings
	{
		public static bool Enabled { get; set; }

		public static string CacheKey { get; set; }
		
		public static string ScriptsCacheKeyFormat { get; set; }

		public static int CacheDuration { get; set; }

		public static string HiddenFieldName { get; set; }

		public static string AppVersion { get; set; }

		static Settings()
		{
			Enabled = true;
			CacheKey = "AssetPackager_Assets";
			ScriptsCacheKeyFormat = "AssetPackager_Scripts/{0}/{1}";
			HiddenFieldName = "__ASSETPACKAGER";
			AppVersion = "1.0.0";
			CacheDuration = 24 * 30;
		}
	}
}