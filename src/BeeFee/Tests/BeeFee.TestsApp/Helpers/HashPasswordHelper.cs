using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BeeFee.TestsApp.Helpers
{
	public static class HashPasswordHelper
	{
		/// <summary>
		/// Return base64 of hash password with a salt
		/// </summary>
		/// <param name="password">Password as a string</param>
		/// <param name="salt">Salt as a byte array</param>
		/// <returns></returns>
		public static string GetHash(string password, byte[] salt)
			=> Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password,
				salt,
				KeyDerivationPrf.HMACSHA512,
				10000,
				64));

		public static byte[] GenerateSalt()
		{
			var salt = new byte[128 / 8];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}
			return salt;
		}
	}
}