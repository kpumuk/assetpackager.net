namespace AssetPackager.Assets
{
	/// <summary>
	/// Represents a particular asset in the <see cref="AssetList" />.
	/// </summary>
	public class Asset
	{
		/// <summary>
		/// Gets or sets asset name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets relative asset path.
		/// </summary>
		public string RelativePath { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether asset should be combined
		/// even if page does not require it.
		/// </summary>
		/// <value>Default <c>false</c>.</value>
		public bool Force { get; set; }

		/// <summary>
		/// Gets or sets group name.
		/// </summary>
		/// <remarks>
		/// When asset from specified group is requested, all other assets from this group
		/// will be returned too.
		/// </remarks>
		public string Group { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Asset" /> class.
		/// </summary>
		/// <param name="name">Asset name.</param>
		/// <param name="path">Relative asset path to combine.</param>
		/// <param name="group">Asset group.</param>
		/// <param name="force">When <c>true</c>, asset will be forced to combine (see <see cref="Force" />).</param>
		public Asset(string name, string path, string group, bool force)
		{
			Name = name;
			RelativePath = path;
			Force = force;
			Group = group;
		}
	}
}