using System;
using System.Security.Cryptography;
using System.Text;

namespace EventManagement.Core.Utilities
{
    public class Hashing
    {
        public static string Create(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var md5 = MD5.Create();
            return ComputeMd5(md5, input);
        }

        public bool VerifyHash(string input, string hash)
        {
            // Hash the input.
            var hashOfInput = Create(input);

            // Create a StringComparer an compare the hashes.
            var comparer = StringComparer.OrdinalIgnoreCase;

            return 0 == comparer.Compare(hashOfInput, hash);
        }

        private static string ComputeMd5(HashAlgorithm md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}