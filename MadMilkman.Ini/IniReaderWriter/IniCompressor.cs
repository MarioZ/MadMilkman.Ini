using System;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace MadMilkman.Ini
{
    internal static class IniCompressor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "MemoryStream doesn't have unmanaged resources.")]
        public static string Compress(string text, Encoding encoding)
        {
            byte[] content = encoding.GetBytes(text);
            var stream = new MemoryStream();

            using (var compressor = new GZipStream(stream, CompressionMode.Compress, true))
                compressor.Write(content, 0, content.Length);

            byte[] compressedContent = new byte[stream.Length + 4];
            stream.Position = 0;
            stream.Read(compressedContent, 4, compressedContent.Length - 4);

            Buffer.BlockCopy(BitConverter.GetBytes(content.Length), 0, compressedContent, 0, 4);
            return Convert.ToBase64String(compressedContent);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "MemoryStream doesn't have unmanaged resources.")]
        public static string Decompress(string text, Encoding encoding)
        {
            byte[] compressedContent = Convert.FromBase64String(text);
            var stream = new MemoryStream(compressedContent, 4, compressedContent.Length - 4);
            byte[] content = new byte[BitConverter.ToInt32(compressedContent, 0)];

            stream.Position = 0;
            using (var decompressor = new GZipStream(stream, CompressionMode.Decompress, false))
                decompressor.Read(content, 0, content.Length);

            return encoding.GetString(content);
        }
    }
}
