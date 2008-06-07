using AssetPackager;

namespace AssetPackager
{
	/// <summary>
	/// Represents a particular URL in the <see cref="UrlMapSet" />.
	/// </summary>
	public class UrlMap
	{
		/// <summary>
		/// Gets or sets name.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		private string _name;

		/// <summary>
		/// Gets or sets URL.
		/// </summary>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}
		private string _url;

		/// <summary>
		/// Gets or sets value indicating whether URL should be combined
		/// even if page does not require it.
		/// </summary>
		/// <value>Default <c>false</c>.</value>
		public bool Force
		{
			get { return _force; }
			set { _force = value; }
		}
		private bool _force;

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlMap" /> class.
		/// </summary>
		/// <param name="name">URL name.</param>
		/// <param name="url">Relative URL to combine.</param>
		/// <param name="force">When <c>true</c>, URL will be forced to combine (see <see cref="Force"/>).</param>
		public UrlMap(string name, string url, bool force)
		{
			Name = name;
			Url = url;
			Force = force;
		}
	}
}