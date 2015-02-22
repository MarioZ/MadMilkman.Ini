using System;
using System.IO;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class IniFileEncryptionCompressionTests
    {
        [Test]
        public void CompressAndDecompressTest()
        {
            var file = new IniFile();
            var section = file.Sections.Add("Compression Test");
            for (int i = 0; i < 1000; i++)
                section.Keys.Add(
                    new IniKey(file, "Key " + i, Guid.NewGuid().ToString()));

            var compressedFile = new IniFile(new IniOptions() { Compression = true });
            compressedFile.Sections.Add(section.Copy(compressedFile));

            var fileStream = new MemoryStream();
            file.Save(fileStream);
            var compressedFileStream = new MemoryStream();
            compressedFile.Save(compressedFileStream);

            Assert.That(compressedFileStream.Length, Is.LessThan(fileStream.Length));

            compressedFile.Sections.Clear();
            compressedFile.Load(compressedFileStream);

            Assert.AreEqual(file.Sections[0].Name, compressedFile.Sections[0].Name);
            for (int i = 0; i < file.Sections[0].Keys.Count; i++)
            {
                Assert.AreEqual(file.Sections[0].Keys[i].Name, compressedFile.Sections[0].Keys[i].Name);
                Assert.AreEqual(file.Sections[0].Keys[i].Value, compressedFile.Sections[0].Keys[i].Value);
            }
        }

        [Test]
        public void EncryptAndDecryptTest()
        {
            var file = new IniFile();
            var section = file.Sections.Add("Encrypt Test");
            section.Keys.Add("Key 1", "Value 1");
            section.Keys.Add("Key 2", "Value 2");

            var encryptedFile = new IniFile(new IniOptions() { EncryptionPassword = @"abcdef" });
            encryptedFile.Sections.Add(section.Copy(encryptedFile));

            var encryptedFileWriter = new StringWriter();
            encryptedFile.Save(encryptedFileWriter);
            var encryptedFileContent = encryptedFileWriter.ToString();

            Assert.IsFalse(encryptedFileContent.Contains("Encrypt Test"));
            Assert.IsFalse(encryptedFileContent.Contains("Key 1"));
            Assert.IsFalse(encryptedFileContent.Contains("Value 1"));
            Assert.IsFalse(encryptedFileContent.Contains("Key 2"));
            Assert.IsFalse(encryptedFileContent.Contains("Value 2"));

            encryptedFile.Sections.Clear();
            encryptedFile.Load(new StringReader(encryptedFileContent));

            Assert.AreEqual("Encrypt Test", encryptedFile.Sections[0].Name);
            Assert.AreEqual("Key 1", encryptedFile.Sections[0].Keys[0].Name);
            Assert.AreEqual("Value 1", encryptedFile.Sections[0].Keys[0].Value);
            Assert.AreEqual("Key 2", encryptedFile.Sections[0].Keys[1].Name);
            Assert.AreEqual("Value 2", encryptedFile.Sections[0].Keys[1].Value);
        }
    }
}
