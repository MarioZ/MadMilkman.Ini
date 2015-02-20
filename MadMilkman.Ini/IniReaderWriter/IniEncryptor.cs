using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MadMilkman.Ini
{
    internal static class IniEncryptor
    {
        /* REMARKS:  RijndaelManaged
         *           - KeySize               - BlockSize
         *              DEFAULT = 256           DEFAULT = 128
         *              MIN = 128               MIN = 128
         *              MAX = 256               MAX = 256
         * 
         *           - InitializationVector.Length
         *              MUST = RijndaelManaged.BlockSize / 8
         * 
         *           - key.Length
         *              MUST = RijndaelManaged.KeySize / 8 */

        private static readonly byte[] InitializationVector = { 77, 52, 225, 184, 143, 77, 49, 225, 184, 187, 107, 77, 52, 225, 185, 137 };
        private static readonly byte[] Salt = { 49, 110, 49, 102, 49, 108, 51, 39, 53, 95, 53, 52, 108, 55, 95, 118, 52, 108, 117, 51 };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "MemoryStream doesn't have unmanaged resources.")]
        public static string Encrypt(string text, string passwordPhrase, Encoding encoding)
        {
            byte[] content = encoding.GetBytes(text);
            byte[] key = new Rfc2898DeriveBytes(passwordPhrase, IniEncryptor.Salt, 1000).GetBytes(32);
            var stream = new MemoryStream();

            using (var rijndaelAlgorithm = new RijndaelManaged())
            using (var encryptor = rijndaelAlgorithm.CreateEncryptor(key, IniEncryptor.InitializationVector))
            using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                cryptoStream.Write(content, 0, content.Length);

            return Convert.ToBase64String(stream.ToArray());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "MemoryStream doesn't have unmanaged resources.")]
        public static string Decrypt(string text, string passwordPhrase, Encoding encoding)
        {
            byte[] encryptedContent = Convert.FromBase64String(text);
            byte[] key = new Rfc2898DeriveBytes(passwordPhrase, IniEncryptor.Salt, 1000).GetBytes(32);
            byte[] content = new byte[encryptedContent.Length];
            int contentCount;

            using (var rijndaelAlgorithm = new RijndaelManaged())
            using (var decryptor = rijndaelAlgorithm.CreateDecryptor(key, IniEncryptor.InitializationVector))
            using (var cryptoStream = new CryptoStream(new MemoryStream(encryptedContent), decryptor, CryptoStreamMode.Read))
                contentCount = cryptoStream.Read(content, 0, content.Length);

            return encoding.GetString(content, 0, contentCount);
        }
    }
}
