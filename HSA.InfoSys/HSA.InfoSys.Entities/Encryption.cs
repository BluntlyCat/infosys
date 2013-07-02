// ------------------------------------------------------------------------
// <copyright file="Encryption.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// Encrypts and decrypts the given bytes.
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// The key.
        /// </summary>
        private const string Key = "tha=aciephuo`zeuzoh6mooj1dohthie";

        /// <summary>
        /// The logger for Encryption.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Encryption");

        /// <summary>
        /// The certificate key.
        /// </summary>
        private static readonly byte[] KeyBytes = Encoding.UTF8.GetBytes(Key);

        /// <summary>
        /// Encrypts the specified plain text bytes.
        /// </summary>
        /// <param name="plainTextBytes">The plain text bytes.</param>
        /// <returns>
        /// The encrypted bytes.
        /// </returns>
        public static byte[] Encrypt(byte[] plainTextBytes)
        {
            var iv = new byte[16];
            var myAes = Aes.Create();

            if (myAes != null)
            {
                var encryptor = myAes.CreateEncryptor(KeyBytes, iv);

                var memoryStream = new MemoryStream();

                var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();

                var cipherTextBytes = memoryStream.ToArray();

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                Log.Debug(Properties.Resources.ENCRYPT);

                return cipherTextBytes;
            }

            return new byte[0];
        }

        /// <summary>
        /// Decrypts the specified cipher text bytes.
        /// </summary>
        /// <param name="cipherTextBytes">The cipher text bytes.</param>
        /// <returns>
        /// The decrypted bytes.
        /// </returns>
        public static string Decrypt(byte[] cipherTextBytes)
        {
            var iv = new byte[16];

            var myAes = Aes.Create();

            if (myAes != null)
            {
                var decryptor = myAes.CreateDecryptor(KeyBytes, iv);

                var memoryStream = new MemoryStream(cipherTextBytes);

                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                var plainTextBytes = new byte[cipherTextBytes.Length];

                // Start decrypting.
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string.
                // Let us assume that the original plaintext string was UTF8-encoded.
                var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                Log.Debug(Properties.Resources.DECRYPT);

                // Return decrypted string.
                return plainText;
            }

            return string.Empty;
        }
    }
}
