using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture, Category("Writing INI files")]
    public class IniFileWriteTests
    {
        [Test]
        public void WriteDefaultTest()
        {
            IniOptions options = new IniOptions();
            IniFile file = new IniFile(options);

            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key", "Value")
                    {
                        TrailingComment = { Text = "Trailing comment" },
                        LeadingComment = { Text = "Leading comment" }
                    })
                {
                    TrailingComment = { Text = "Trailing comment" },
                    LeadingComment = { Text = "Leading comment" }
                });

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.AreEqual(";Trailing comment", lines[0]);
            Assert.AreEqual("[Section];Leading comment", lines[1]);
            Assert.AreEqual(";Trailing comment", lines[2]);
            Assert.AreEqual("Key=Value;Leading comment", lines[3]);
        }

        [Test]
        public void WriteCustomTest()
        {
            IniOptions options = new IniOptions()
            {
                KeyDelimiter = IniKeyDelimiter.Colon,
                KeySpaceAroundDelimiter = true,
                SectionWrapper = IniSectionWrapper.Parentheses
            };
            IniFile file = new IniFile(options);

            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key", "Value")));

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.AreEqual("(Section)", lines[0]);
            Assert.AreEqual("Key : Value", lines[1]);
        }

        [Test]
        public void WriteGlobalSectionTest()
        {
            IniOptions options = new IniOptions();
            IniFile file = new IniFile(options);

            file.Sections.Add(
                new IniSection(file, IniSection.GlobalSectionName,
                    new IniKey(file, "Key1", "Value1"),
                    new IniKey(file, "Key2", "Value2")));

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.AreEqual("Key1=Value1", lines[0]);
            Assert.AreEqual("Key2=Value2", lines[1]);
        }

        [Test]
        public void WriteUTF8EncodingTest()
        {
            IniOptions options = new IniOptions() { Encoding = Encoding.UTF8 };
            IniFile file = new IniFile(options);

            file.Sections.Add(new IniSection(file, "Καλημέρα κόσμε"));
            file.Sections.Add(new IniSection(file, "こんにちは 世界"));

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.AreEqual("[Καλημέρα κόσμε]", lines[0]);
            Assert.AreEqual("[こんにちは 世界]", lines[1]);
        }

        [Test]
        public void WriteEmptyLinesTest()
        {
            IniOptions options = new IniOptions() { Encoding = Encoding.UTF8 };
            IniFile file = new IniFile(options);

            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key")
                    {
                        TrailingComment = { Text = string.Empty, EmptyLinesBefore = 2 },
                        LeadingComment = { Text = string.Empty, EmptyLinesBefore = 1 }
                    })
                {
                    TrailingComment = { Text = string.Empty, EmptyLinesBefore = 2 },
                    LeadingComment = { Text = string.Empty, EmptyLinesBefore = 1 }
                });

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.IsEmpty(lines[0]);
            Assert.IsEmpty(lines[1]);
            Assert.AreEqual(";", lines[2]);
            Assert.IsEmpty(lines[3]);
            Assert.AreEqual("[Section];", lines[4]);
            Assert.IsEmpty(lines[5]);
            Assert.IsEmpty(lines[6]);
            Assert.AreEqual(";", lines[7]);
            Assert.IsEmpty(lines[8]);
            Assert.AreEqual("Key=;", lines[9]);
        }

        [Test]
        public void WriteLeftIndentionTest()
        {
            IniOptions options = new IniOptions() { Encoding = Encoding.UTF8 };
            IniFile file = new IniFile(options);

            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key")
                    {
                        LeftIndentation = 2,
                        TrailingComment = { Text = string.Empty, LeftIndentation = 4 },
                        LeadingComment = { Text = string.Empty, LeftIndentation = 2 }
                    })
                {
                    LeftIndentation = 2,
                    TrailingComment = { Text = string.Empty, LeftIndentation = 4 },
                    LeadingComment = { Text = string.Empty, LeftIndentation = 2 }
                });

            string[] lines = IniUtilities.SaveIniFileContent(file, options);
            Assert.AreEqual("    ;", lines[0]);
            Assert.AreEqual("  [Section]  ;", lines[1]);
            Assert.AreEqual("    ;", lines[2]);
            Assert.AreEqual("  Key=  ;", lines[3]);
        }
    }
}
