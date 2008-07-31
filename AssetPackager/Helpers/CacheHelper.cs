using System;
using System.Web;
using System.Web.Caching;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Represents the method that defines a logic to retrieve data needed to cache
	/// when cache missed.
	/// </summary>
	/// <returns>An object to cache.</returns>
	public delegate object CacheHelperAction();

	/// <summary>
	/// Contains helper methods for caching.
	/// </summary>
	public static class CacheHelper
	{
		/// <summary>
		/// Gets data from cache or (when cache is missing) calls <paramref name="action" />
		/// and stores a value in cache.
		/// </summary>
		/// <param name="cacheKey">The cache key used to reference the item.</param>
		/// <param name="dependencies">The file or cache key dependencies for the item. When any 
		/// dependency changes, the object becomes invalid and is removed from the cache. If there 
		/// are no dependencies, this parameter contains <c>null</c>.</param>
		/// <param name="absoluteExpiration">The time at which the added object expires and is 
		/// removed from the cache.</param>
		/// <param name="priority">The relative cost of the object, as expressed by the 
		/// <see cref="CacheItemPriority" /> enumeration. The cache uses this value when it evicts 
		/// objects; objects with a lower cost are removed from the cache before objects with a 
		/// higher cost.</param>
		/// <param name="action">A delegate called when no data found in cache.</param>
		/// <returns>An <see cref="Object" /> stored in the <see cref="Cache" />.</returns>
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
