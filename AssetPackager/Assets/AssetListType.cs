namespace AssetPackager.Assets
{
	/// <summary>
	/// Represents <see cref="AssetList" /> type.
	/// </summary>
	public enum AssetListType
	{
		/// <summary>
		/// Asset list type is undefined.
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Asset list contains JavaScript files.
		/// </summary>
		[ShortName("js"), MimeType("text/javascript")]
		JavaScript = 1,
		/// <summary>
		/// Asset list contains CSS files.
		/// </summary>
		[ShortName("css"), MimeType("text/css")]
		Css = 2
	}
}