using System;
using System.IO;
using System.Text;
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
            section.Keys.Add("Username", "John Doe");
            section.Keys.Add("Password", "M4dM1lkM4n.1n1");

            var encryptedFile = new IniFile(new IniOptions() { EncryptionPassword = @"P@55\/\/0|2D" });
            encryptedFile.Sections.Add(section.Copy(encryptedFile));

            var encryptedFileWriter = new StringWriter();
            encryptedFile.Save(encryptedFileWriter);
            var encryptedFileContent = encryptedFileWriter.ToString();

            Assert.IsFalse(encryptedFileContent.Contains("Encrypt Test"));
            Assert.IsFalse(encryptedFileContent.Contains("Username"));
            Assert.IsFalse(encryptedFileContent.Contains("John Doe"));
            Assert.IsFalse(encryptedFileContent.Contains("Password"));
            Assert.IsFalse(encryptedFileContent.Contains("M4dM1lkM4n.1n1"));

            encryptedFile.Sections.Clear();
            encryptedFile.Load(new StringReader(encryptedFileContent));

            Assert.AreEqual("Encrypt Test", encryptedFile.Sections[0].Name);
            Assert.AreEqual("Username", encryptedFile.Sections[0].Keys[0].Name);
            Assert.AreEqual("John Doe", encryptedFile.Sections[0].Keys[0].Value);
            Assert.AreEqual("Password", encryptedFile.Sections[0].Keys[1].Name);
            Assert.AreEqual("M4dM1lkM4n.1n1", encryptedFile.Sections[0].Keys[1].Value);
        }
    }
}
