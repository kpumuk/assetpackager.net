using System;
using System.Web;
using System.Web.Caching;

namespace AssetPackager.Helpers
{
	public delegate object CacheHelperAction();

	public static class CacheHelper
	{
		public static object GetData(string cacheKey, CacheDependency dependencies, 
			DateTime absoluteExpiration, CacheItemPriority priority, CacheHelperAction action)
		{
			if (String.IsNullOrEmpty(cacheKey)) return action();
			object data = HttpContext.Current.Cache[cacheKey];
			if (data == null)
			{
				data = action();
				lock (_lockObject)
				{
					if (HttpContext.Current.Cache[cacheKey] == null)
					{
						HttpContext.Current.Cache.Add(cacheKey, data, dependencies, absoluteExpiration,
						                              Cache.NoSlidingExpiration, priority, null);
					}
				}
			}
			return data;
		}

		private static readonly object _lockObject = new object();
	}
}
