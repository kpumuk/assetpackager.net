using System;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace AssetPackager.Helpers
{
	/// <summary>
	/// Contains helper methods for URL parameters encryption.
	/// </summary>
	public static class EncryptionHelper
	{
		/// <summary>
		/// Encrypts a string value.
		/// </summary>
		/// <param name="value">String to encrypt.</param>
		/// <returns>Encrypted string ready to use in query string.</returns>
		public static string EncryptString(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return HttpServerUtility.UrlTokenEncode(EncryptOrDecryptData(true, bytes, null, 0, bytes.Length));
		}

		/// <summary>
		/// Decrypts a string value.
		/// </summary>
		/// <param name="value">Strign to decrypt.</param>
		/// <returns>Decrypted string value.</returns>
		public static string DecryptString(string value)
		{
			if (value == null) return null;

			byte[] buf = HttpServerUtility.UrlTokenDecode(value);
			if (buf == null) throw new HttpException("Script URL is invalid");

			buf = EncryptOrDecryptData(false, buf, null, 0, buf.Length);
			return Encoding.UTF8.GetString(buf);
		}

		/// <summary>
		/// Calls internal .NET methods to encrypt or decrypt string buffer.
		/// </summary>
		/// <param name="fEncrypt">When <c>true</c>, string will be encrypted, when <c>false</c> - decrypted.</param>
		/// <param name="buf">A string buffer to process.</param>
		/// <param name="modifier">A salt to use in encryption/decryption.</param>
		/// <param name="start">Start offset in the buffer.</param>
		/// <param name="length">Identifies how many bytes to process.</param>
		/// <returns>Encrypted or decrypted string value, base on <paramref name="fEncrypt" /> value.</returns>
		private static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length)
		{
			Type[] parameters = new Type[] {typeof (bool), typeof (byte[]), typeof (byte[]), typeof (int), typeof (int)};
			MethodInfo m = typeof (MachineKeySection).GetMethod("EncryptOrDecryptData",
			                                                    BindingFlags.Static | BindingFlags.NonPublic,
			                                                    null, parameters, null);
			return (byte[]) m.Invoke(null, new object[] {fEncrypt, buf, modifier, start, length});
		}
	}
}
