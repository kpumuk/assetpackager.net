using System;
using System.Collections.Generic;
using AssetPackager;

namespace AssetPackager
{
	/// <summary>
	/// Represents set of URLs for <see cref="CombineScripts" />.
	/// </summary>
	public class UrlMapSet
	{
		/// <summary>
		/// Gets or sets name of the URL set.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		private string _name;

		/// <summary>
		/// Gets or sets value indicating whether all URLs in set should be combined
		/// even if page does not require some ones.
		/// </summary>
		/// <value>Default <c>false</c>.</value>
		public bool Force
		{
			get { return _force; }
			set { _force = value; }
		}
		private bool _force;

		/// <summary>
		/// Gets a list of URLs in set.
		/// </summary>
		public List<UrlMap> Urls
		{
			get { return _urls; }
		}
		private readonly List<UrlMap> _urls = new List<UrlMap>();

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlMapSet" /> class.
		/// </summary>
		/// <param name="name">Set name.</param>
		/// <param name="force">When <c>true</c> all URLs will be forced to combine (see <see cref="Force"/>).</param>
		public UrlMapSet(string name, bool force)
		{
			Name = name;
			Force = force;
		}

		public List<UrlMap> FindForcedUrlsNotInSet(List<string> urlNames)
		{
			return Urls.FindAll(delegate(UrlMap url)
			                    	{
			                    		return (Force || url.Force) && !urlNames.Contains(url.Name);
			                    	});
		}

		public UrlMap FindUrl(string url)
		{
			return Urls.Find(delegate(UrlMap map) { return map.Url == url; });
		}

		public UrlMap FindUrl(string url, string[] urlNames)
		{
			return Urls.Find(delegate(UrlMap urlMap)
			                 	{
			                 		return url == urlMap.Url &&
			                 		       Array.IndexOf(urlNames, urlMap.Name) >= 0;
			                 	});
		}
	}
}