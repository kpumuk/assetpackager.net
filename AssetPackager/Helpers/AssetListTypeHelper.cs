using System;
using System.Reflection;
using AssetPackager.Assets;

namespace AssetPackager.Helpers
{
	public static class AssetListTypeHelper
	{
		/// <summary>
		/// Gets an asset list type from string.
		/// </summary>
		/// <param name="shortName">Asset type (full or short name).</param>
		/// <returns>Asset list type or <see cref="AssetListType.Undefined" /> when type is undefined.</returns>
		public static AssetListType GetTypeByName(string shortName)
		{
			if (String.IsNullOrEmpty(shortName)) return AssetListType.Undefined;

			shortName = shortName.ToUpperInvariant();
			foreach (FieldInfo field in typeof(AssetListType).GetFields())
			{
				object[] attributes = field.GetCustomAttributes(typeof(ShortNameAttribute), false);

				foreach (ShortNameAttribute shortNameAttribute in attributes)
				{
					if (String.CompareOrdinal(shortName, shortNameAttribute.Name.ToUpperInvariant()) == 0)
						return (AssetListType) field.GetValue(null);
				}
				if (String.CompareOrdinal(shortName, field.Name.ToUpperInvariant()) == 0)
					return (AssetListType)field.GetValue(null);
			}
			return AssetListType.Undefined;
		}

		public static string GetMimeType(AssetListType assetListType)
		{
			object[] attributes = assetListType.GetType().GetCustomAttributes(typeof (MimeTypeAttribute), false);
			return attributes.Length == 0 ? String.Empty : ((MimeTypeAttribute) attributes[0]).MimeType;
		}
	}
}
