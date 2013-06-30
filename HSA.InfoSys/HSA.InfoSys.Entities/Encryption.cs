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
        /// The logger for Encryption.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Encryption");

        /// <summary>
        /// The key.
        /// </summary>
        private static string key = "tha=aciephuo`zeuzoh6mooj1dohthie";

        /// <summary>
        /// The certificate key.
        /// </summary>
        private static byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        /// <summary>
        /// Encrypts the specified plain text bytes.
        /// </summary>
        /// <param name="plainTextBytes">The plain text bytes.</param>
        /// <returns>
        /// The encrypted bytes.
        /// </returns>
        public static byte[] Encrypt(byte[] plainTextBytes)
        {
            byte[] iv = new byte[16];

            Aes myAes = Aes.Create();

            ICryptoTransform encryptor = myAes.CreateEncryptor(keyBytes, iv);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            Log.Debug(Properties.Resources.ENCRYPT);

            return cipherTextBytes;
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
            byte[] iv = new byte[16];

            Aes myAes = Aes.Create();

            ICryptoTransform decryptor = myAes.CreateDecryptor(keyBytes, iv);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string.
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            Log.Debug(Properties.Resources.DECRYPT);

            // Return decrypted string.
            return plainText;
        }
    }
}
