using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace MyPass
{
	public class DecryptionError : Exception
	{
	}

	public class Encryption
	{
		private static byte[] InitVector =
		{
			0x55, 0x3b, 0x94, 0x9a, 0x8d, 0xe3, 0x1e, 0x84, 0xd8, 0xd5,
			0xb3, 0x05, 0x54, 0x81, 0x77, 0xad
		};
		private static int KeySize = 256;
		private static byte[] Salt = { 0x12, 49, 81, 102, 49 };

		public static byte[] Encrypt(string str, string pass)
		{
			byte[] strBytes = Encoding.UTF8.GetBytes(str);
			var passBytes = new PasswordDeriveBytes(pass, Salt);
			byte[] keyBytes = passBytes.GetBytes(KeySize / 8);
			var symKey = new RijndaelManaged();
			symKey.Mode = CipherMode.CBC;

			var encrypter = symKey.CreateEncryptor(keyBytes, InitVector);
			var memStream = new MemoryStream();
			var cryptoStream =
				new CryptoStream(memStream, encrypter, CryptoStreamMode.Write);
			cryptoStream.Write(strBytes, 0, strBytes.Length);
			cryptoStream.FlushFinalBlock();
			byte[] encryptedBytes = memStream.ToArray();
			memStream.Close();
			cryptoStream.Close();
			return encryptedBytes;
		}

		public static string Decrypt(byte[] encrypted, string pass)
		{
			var passBytes = new PasswordDeriveBytes(pass, Salt);
			byte[] keyBytes = passBytes.GetBytes(KeySize / 8);
			var symKey = new RijndaelManaged();
			symKey.Mode = CipherMode.CBC;
			try
			{
				var decrypter = symKey.CreateDecryptor(keyBytes, InitVector);
				var memStream = new MemoryStream(encrypted);
				var cryptoStream =
					new CryptoStream(memStream, decrypter, CryptoStreamMode.Read);
				byte[] decryptedBytes = new byte[encrypted.Length];
				int decryptedCount = cryptoStream.Read(decryptedBytes, 0,
					decryptedBytes.Length);
				memStream.Close();
				cryptoStream.Close();
				return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
			}
			catch (Exception)
			{
				throw new DecryptionError();
			}
		}
	}
}
