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
		public static string EncryptString(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return HttpServerUtility.UrlTokenEncode(EncryptOrDecryptData(true, bytes, null, 0, bytes.Length));
		}

		public static string DecryptString(string value)
		{
			if (value == null) return null;

			byte[] buf = HttpServerUtility.UrlTokenDecode(value);
			if (buf == null) throw new HttpException("Script URL is invalid");

			buf = EncryptOrDecryptData(false, buf, null, 0, buf.Length);
			return Encoding.UTF8.GetString(buf);
		}

		private static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length)
		{
			MethodInfo m = typeof(MachineKeySection).GetMethod("EncryptOrDecryptData",
																BindingFlags.Static | BindingFlags.NonPublic);
			return (byte[]) m.Invoke(null, new object[] {fEncrypt, buf, modifier, start, length});
		}
	}
}
