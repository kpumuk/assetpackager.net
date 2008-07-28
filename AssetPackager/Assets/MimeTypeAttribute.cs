using System;

namespace AssetPackager.Assets
{
	/// <summary>
	/// Contains MIME type of the objects contained in the <see cref="AssetList" />.
	/// </summary>
	/// <remarks>Should be defined for items in <see cref="AssetListType" /> enum.</remarks>
	/// <seealso cref="AssetListType" />
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MimeTypeAttribute : Attribute
	{
		/// <summary>
		/// Gets the MIME type of assets in the <see cref="AssetList" />.
		/// </summary>
		public string MimeType { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MimeTypeAttribute" /> class.
		/// </summary>
		public MimeTypeAttribute()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="MimeTypeAttribute" /> class.
		/// </summary>
		/// <param name="mimeType">Mime type (for example, "<c>text/css</c>").</param>
		public MimeTypeAttribute(string mimeType)
		{
			MimeType = mimeType;
		}
	}
}
