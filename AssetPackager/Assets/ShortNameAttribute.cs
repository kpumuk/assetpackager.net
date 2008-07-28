using System;

namespace AssetPackager.Assets
{
	/// <summary>
	/// Provides a short name for type of the <see cref="AssetList" /> object.
	/// </summary>
	/// <remarks>Should be defined for items in <see cref="AssetListType" /> enum.</remarks>
	/// <seealso cref="AssetListType" />
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class ShortNameAttribute : Attribute
	{
		/// <summary>
		/// Short name of the item in <see cref="AssetListType" /> enum.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortNameAttribute" /> class.
		/// </summary>
		public ShortNameAttribute()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortNameAttribute" /> class.
		/// </summary>
		/// <param name="name">Short name of the asset list type (for example, "<c>js</c>" 
		/// for <see cref="AssetListType.JavaScript"/>).</param>
		public ShortNameAttribute(string name)
		{
			Name = name;
		}
	}
}
