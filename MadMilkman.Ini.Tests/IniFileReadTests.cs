using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class IniFileReadTests
    {
        [Test]
        public void ReadDefaultTest()
        {
            string iniFileContent = ";Section's trailing comment." + Environment.NewLine +
                                    "[Section's name];Section's leading comment." + Environment.NewLine +
                                    ";Key's trailing comment." + Environment.NewLine +
                                    "Key's name = Key's value;Key's leading comment.";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            Assert.AreEqual(1, file.Sections.Count);
            Assert.AreEqual("Section's trailing comment.", file.Sections[0].TrailingComment.Text);
            Assert.AreEqual("Section's name", file.Sections[0].Name);
            Assert.AreEqual("Section's leading comment.", file.Sections[0].LeadingComment.Text);

            Assert.AreEqual(1, file.Sections[0].Keys.Count);
            Assert.AreEqual("Key's trailing comment.", file.Sections[0].Keys[0].TrailingComment.Text);
            Assert.AreEqual("Key's name", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("Key's value", file.Sections[0].Keys[0].Value);
            Assert.AreEqual("Key's leading comment.", file.Sections[0].Keys[0].LeadingComment.Text);
        }

        [Test]
        public void ReadCustomTest()
        {
            string iniFileContent = "#Section's trailing comment." + Environment.NewLine +
                                    "{Section's name}#Section's leading comment." + Environment.NewLine +
                                    "#Key's trailing comment." + Environment.NewLine +
                                    "Key's name : Key's value#Key's leading comment.";

            IniOptions options = new IniOptions()
            {
                CommentStarter = IniCommentStarter.Hash,
                SectionWrapper = IniSectionWrapper.CurlyBrackets,
                KeyDelimiter = IniKeyDelimiter.Colon
            };
            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, options);

            Assert.AreEqual(1, file.Sections.Count);
            Assert.AreEqual("Section's trailing comment.", file.Sections[0].TrailingComment.Text);
            Assert.AreEqual("Section's name", file.Sections[0].Name);
            Assert.AreEqual("Section's leading comment.", file.Sections[0].LeadingComment.Text);

            Assert.AreEqual(1, file.Sections[0].Keys.Count);
            Assert.AreEqual("Key's trailing comment.", file.Sections[0].Keys[0].TrailingComment.Text);
            Assert.AreEqual("Key's name", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("Key's value", file.Sections[0].Keys[0].Value);
            Assert.AreEqual("Key's leading comment.", file.Sections[0].Keys[0].LeadingComment.Text);
        }

        [Test]
        public void ReadGlobalSectionTest()
        {
            string iniFileContent = ";Trailing comment1" + Environment.NewLine +
                                    "Key1 = Value1" + Environment.NewLine +
                                    ";Trailing comment2" + Environment.NewLine +
                                    "Key2 = Value2";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            Assert.AreEqual(1, file.Sections.Count);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[0].Name);

            Assert.AreEqual(2, file.Sections[0].Keys.Count);
            Assert.AreEqual("Trailing comment1", file.Sections[0].Keys[0].TrailingComment.Text);
            Assert.AreEqual("Key1", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("Value1", file.Sections[0].Keys[0].Value);
            Assert.AreEqual("Trailing comment2", file.Sections[0].Keys[1].TrailingComment.Text);
            Assert.AreEqual("Key2", file.Sections[0].Keys[1].Name);
            Assert.AreEqual("Value2", file.Sections[0].Keys[1].Value);
        }

        [Test]
        public void ReadMultipleGlobalSectionsTest()
        {
            string inputContent = "Key = Value" + Environment.NewLine +
                                  "[Section]" + Environment.NewLine +
                                  "Key = Value";
            IniFile file = new IniFile();

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(inputContent)))
            {
                file.Load(stream);
                file.Load(stream);
                file.Load(stream);
            }

            Assert.AreEqual(6, file.Sections.Count);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[0].Name);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[2].Name);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[4].Name);
            Assert.AreEqual("Section", file.Sections[1].Name);
            Assert.AreEqual("Section", file.Sections[3].Name);
            Assert.AreEqual("Section", file.Sections[5].Name);

            foreach (var section in file.Sections)
                Assert.AreEqual(1, section.Keys.Count);

            string outputContent;
            using (var stream = new MemoryStream())
            {
                file.Save(stream);
                outputContent = new StreamReader(stream, Encoding.ASCII).ReadToEnd();
            }

            file.Sections.Clear();
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(outputContent)))
                file.Load(stream);

            Assert.AreEqual(6, file.Sections.Count);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[0].Name);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[2].Name);
            Assert.AreEqual(IniSection.GlobalSectionName, file.Sections[4].Name);
            Assert.AreEqual("Section", file.Sections[1].Name);
            Assert.AreEqual("Section", file.Sections[3].Name);
            Assert.AreEqual("Section", file.Sections[5].Name);

            foreach (var section in file.Sections)
                Assert.AreEqual(1, section.Keys.Count);
        }

        [Test]
        public void ReadUTF8EncodingTest()
        {
            string iniFileContent = "[Καλημέρα κόσμε]" + Environment.NewLine +
                                    "こんにちは 世界 = ¥ £ € $ ¢ ₡ ₢ ₣ ₤ ₥ ₦ ₧ ₨ ₩ ₪ ₫ ₭ ₮ ₯ ₹";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions() { Encoding = Encoding.UTF8 });

            Assert.AreEqual("Καλημέρα κόσμε", file.Sections[0].Name);
            Assert.AreEqual("こんにちは 世界", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("¥ £ € $ ¢ ₡ ₢ ₣ ₤ ₥ ₦ ₧ ₨ ₩ ₪ ₫ ₭ ₮ ₯ ₹", file.Sections[0].Keys[0].Value);
        }

        [Test]
        public void ReadEmptyLinesTest()
        {
            string iniFileContent = Environment.NewLine +
                                    "  \t  " + Environment.NewLine +
                                    "[Section]" + Environment.NewLine +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    "  \t  " + Environment.NewLine +
                                    "Key = Value" + Environment.NewLine +
                                    Environment.NewLine +
                                    "  \t  " + Environment.NewLine +
                                    ";" + Environment.NewLine +
                                    "[Section]" + Environment.NewLine +
                                    Environment.NewLine +
                                    "  \t  " + Environment.NewLine +
                                    Environment.NewLine +
                                    ";" + Environment.NewLine +
                                    "Key = Value";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            Assert.AreEqual(2, file.Sections[0].LeadingComment.EmptyLinesBefore);
            Assert.AreEqual(3, file.Sections[0].Keys[0].LeadingComment.EmptyLinesBefore);
            Assert.AreEqual(2, file.Sections[1].TrailingComment.EmptyLinesBefore);
            Assert.AreEqual(3, file.Sections[1].Keys[0].TrailingComment.EmptyLinesBefore);
        }

        [Test]
        public void ReadCommentEdgeCasesTest()
        {
            string iniFileContent = ";" + Environment.NewLine +
                                    ";Section's trailing comment;" + Environment.NewLine +
                                    "[Section]" + Environment.NewLine +
                                    "[Section];" + Environment.NewLine +
                                    "[Section]  ;" + Environment.NewLine +
                                    ";" + Environment.NewLine +
                                    ";Key's trailing comment;" + Environment.NewLine +
                                    "Key = Value  " + Environment.NewLine +
                                    "Key = Value;" + Environment.NewLine +
                                    "Key = Value  ;";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            Assert.AreEqual(Environment.NewLine + "Section's trailing comment;", file.Sections[0].TrailingComment.Text);
            Assert.AreEqual("Section", file.Sections[0].Name);
            Assert.IsNull(file.Sections[0].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].LeadingComment.LeftIndentation);

            Assert.AreEqual("Section", file.Sections[1].Name);
            Assert.IsEmpty(file.Sections[1].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[1].LeadingComment.LeftIndentation);

            Assert.AreEqual("Section", file.Sections[2].Name);
            Assert.IsEmpty(file.Sections[2].LeadingComment.Text);
            Assert.AreEqual(2, file.Sections[2].LeadingComment.LeftIndentation);

            Assert.AreEqual(Environment.NewLine + "Key's trailing comment;", file.Sections[2].Keys[0].TrailingComment.Text);
            Assert.AreEqual("Key", file.Sections[2].Keys[0].Name);
            Assert.AreEqual("Value", file.Sections[2].Keys[0].Value);
            Assert.IsNull(file.Sections[2].Keys[0].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[2].Keys[0].LeadingComment.LeftIndentation);

            Assert.AreEqual("Key", file.Sections[2].Keys[1].Name);
            Assert.AreEqual("Value", file.Sections[2].Keys[1].Value);
            Assert.IsEmpty(file.Sections[2].Keys[1].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[2].Keys[1].LeadingComment.LeftIndentation);

            Assert.AreEqual("Key", file.Sections[2].Keys[2].Name);
            Assert.AreEqual("Value", file.Sections[2].Keys[2].Value);
            Assert.IsEmpty(file.Sections[2].Keys[2].LeadingComment.Text);
            Assert.AreEqual(2, file.Sections[2].Keys[2].LeadingComment.LeftIndentation);
        }

        [Test]
        public void ReadValueEdgeCasesTest()
        {
            string iniFileContent = "[Section]" + Environment.NewLine +
                                    "Key=" + Environment.NewLine +
                                    "Key=;" + Environment.NewLine +
                                    "Key= " + Environment.NewLine +
                                    "Key= ;" + Environment.NewLine +
                                    "Key =" + Environment.NewLine +
                                    "Key =;" + Environment.NewLine +
                                    "Key = " + Environment.NewLine +
                                    "Key = ;";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            Assert.IsEmpty(file.Sections[0].Keys[0].Value);
            Assert.IsNull(file.Sections[0].Keys[0].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[0].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[1].Value);
            Assert.IsEmpty(file.Sections[0].Keys[1].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[1].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[2].Value);
            Assert.IsNull(file.Sections[0].Keys[2].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[2].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[3].Value);
            Assert.IsEmpty(file.Sections[0].Keys[3].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[3].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[4].Value);
            Assert.IsNull(file.Sections[0].Keys[4].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[4].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[5].Value);
            Assert.IsEmpty(file.Sections[0].Keys[5].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[5].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[6].Value);
            Assert.IsNull(file.Sections[0].Keys[6].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[6].LeadingComment.LeftIndentation);
            Assert.IsEmpty(file.Sections[0].Keys[7].Value);
            Assert.IsEmpty(file.Sections[0].Keys[7].LeadingComment.Text);
            Assert.AreEqual(0, file.Sections[0].Keys[7].LeadingComment.LeftIndentation);
        }

        [Test]
        public void ReadSectionEdgeCasesTest()
        {
            string iniFileContent = "[" + Environment.NewLine +
                                    "]" + Environment.NewLine +
                                    "[]" + Environment.NewLine +
                                    "[;]" + Environment.NewLine +
                                    "[;;]" + Environment.NewLine +
                                    "[[]]" + Environment.NewLine +
                                    "[[]];" + Environment.NewLine +
                                    "[[;]]" + Environment.NewLine +
                                    "[[;]];";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());
            Assert.AreEqual(7, file.Sections.Count);
            Assert.AreEqual(string.Empty, file.Sections[0].Name);
            Assert.AreEqual(";", file.Sections[1].Name);
            Assert.AreEqual(";;", file.Sections[2].Name);
            Assert.AreEqual("[]", file.Sections[3].Name);
            Assert.AreEqual("[]", file.Sections[4].Name);
            Assert.AreEqual(string.Empty, file.Sections[4].LeadingComment.Text);
            Assert.AreEqual("[;]", file.Sections[5].Name);
            Assert.AreEqual("[;]", file.Sections[6].Name);
            Assert.AreEqual(string.Empty, file.Sections[6].LeadingComment.Text);
        }

        [Test]
        public void ReadQuotedValue()
        {
            string iniFileContent = "key1 = \"Test;Test\"" + Environment.NewLine +
                                    "key2 = \"Test;Test\";" + Environment.NewLine +
                                    "key3 = \"Test;Test" + Environment.NewLine +
                                    "key4 = \"Test;Test;Test\"Test;Test;Test";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());
            IniSection section = file.Sections[0];

            Assert.AreEqual("\"Test;Test\"", file.Sections[0].Keys[0].Value);
            Assert.IsNull(file.Sections[0].Keys[0].LeadingComment.Text);
            Assert.AreEqual("\"Test;Test\"", file.Sections[0].Keys[1].Value);
            Assert.AreEqual(string.Empty, file.Sections[0].Keys[1].LeadingComment.Text);
            Assert.AreEqual("\"Test", file.Sections[0].Keys[2].Value);
            Assert.AreEqual("Test", file.Sections[0].Keys[2].LeadingComment.Text);
            Assert.AreEqual("\"Test;Test;Test\"Test", file.Sections[0].Keys[3].Value);
            Assert.AreEqual("Test;Test", file.Sections[0].Keys[3].LeadingComment.Text);
        }
    }
}
