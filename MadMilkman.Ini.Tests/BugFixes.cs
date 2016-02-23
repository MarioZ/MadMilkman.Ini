using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class BugFixes
    {
        [Test]
        public void Bug1()
        {
            string iniFileContent = "[Segment [A]]" + Environment.NewLine +
                                    "[Segment [A]];[Comment];" + Environment.NewLine +

                                    "[Segment [A][1]]" + Environment.NewLine +
                                    "[Segment [A][1]]  ;[Comment][1];" + Environment.NewLine +

                                    "[Segment;A]" + Environment.NewLine +
                                    "[Segment[A;B]]" + Environment.NewLine +
                                    "[Segment[A;B]];" + Environment.NewLine +
                                    "[Segment[A;B]]    ;[Comment];" + Environment.NewLine +

                                    "[Segment[A;B]]AB;Invalid comment" + Environment.NewLine +
                                    "[Segment[A;B]][[;Invalid comment" + Environment.NewLine +
                                    "[Segment[A;B;]";

            IniOptions options = new IniOptions();
            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, options);

            Assert.AreEqual("Segment [A]", file.Sections[0].Name);
            Assert.AreEqual("Segment [A]", file.Sections[1].Name);
            Assert.AreEqual("[Comment];", file.Sections[1].LeadingComment.Text);

            Assert.AreEqual("Segment [A][1]", file.Sections[2].Name);
            Assert.AreEqual("Segment [A][1]", file.Sections[3].Name);
            Assert.AreEqual("[Comment][1];", file.Sections[3].LeadingComment.Text);
            Assert.AreEqual(2, file.Sections[3].LeadingComment.LeftIndentation);

            Assert.AreEqual("Segment;A", file.Sections[4].Name);
            Assert.AreEqual(null, file.Sections[4].LeadingComment.Text);
            Assert.AreEqual("Segment[A;B]", file.Sections[5].Name);
            Assert.AreEqual(null, file.Sections[5].LeadingComment.Text);
            Assert.AreEqual("Segment[A;B]", file.Sections[6].Name);
            Assert.AreEqual(string.Empty, file.Sections[6].LeadingComment.Text);
            Assert.AreEqual("Segment[A;B]", file.Sections[7].Name);
            Assert.AreEqual("[Comment];", file.Sections[7].LeadingComment.Text);
            Assert.AreEqual(4, file.Sections[7].LeadingComment.LeftIndentation);

            Assert.AreEqual("Segment[A;B]", file.Sections[8].Name);
            Assert.AreEqual(null, file.Sections[8].LeadingComment.Text);
            Assert.AreEqual("Segment[A;B]", file.Sections[9].Name);
            Assert.AreEqual(null, file.Sections[9].LeadingComment.Text);
            Assert.AreEqual("Segment[A;B;", file.Sections[10].Name);

            string[] lines = IniUtilities.SaveIniFileContent(file, options);

            Assert.AreEqual("[Segment [A]]", lines[0]);
            Assert.AreEqual("[Segment [A]];[Comment];", lines[1]);
            Assert.AreEqual("[Segment [A][1]]", lines[2]);
            Assert.AreEqual("[Segment [A][1]]  ;[Comment][1];", lines[3]);
            Assert.AreEqual("[Segment;A]", lines[4]);
            Assert.AreEqual("[Segment[A;B]]", lines[5]);
            Assert.AreEqual("[Segment[A;B]];", lines[6]);
            Assert.AreEqual("[Segment[A;B]]    ;[Comment];", lines[7]);
            Assert.AreEqual("[Segment[A;B]]", lines[8]);
            Assert.AreEqual("[Segment[A;B]]", lines[9]);
            Assert.AreEqual("[Segment[A;B;]", lines[10]);
        }

        public class Bug2Class
        {
            public string[] NullArray { get; set; }
            public List<string> NullList { get; set; }

            public string[] EmptyArray { get; set; }
            public List<string> EmptyList { get; set; }
        }

        [Test]
        public void Bug2()
        {
            var ini = new IniFile();
            var sec = ini.Sections.Add("Sample");
            
            sec.Serialize(
                new Bug2Class()
                {
                    EmptyArray = new string[3],
                    EmptyList = new List<string>()
                });

            var deserializeObj = sec.Deserialize<Bug2Class>();

            Assert.IsNull(sec.Keys["NullArray"].Value);
            Assert.IsNull(sec.Keys["NullList"].Value);
            Assert.AreEqual("{,,}", sec.Keys["EmptyArray"].Value);
            Assert.AreEqual("{}", sec.Keys["EmptyList"].Value);

            Assert.IsNull(deserializeObj.NullArray);
            Assert.IsNull(deserializeObj.NullList);
            CollectionAssert.AreEqual(new string[] { "", "", "" }, deserializeObj.EmptyArray);
            CollectionAssert.IsEmpty(deserializeObj.EmptyList);
        }

        [Test]
        public void Bug3()
        {
            string iniFileContent = "[sektion]" + Environment.NewLine +
                                    "key=\"Data Source=server;Initial Catalog=catalog;Integrated Security=SSPI\"";
            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());

            IniKey key = file.Sections["sektion"].Keys["key"];
            Assert.AreEqual("\"Data Source=server;Initial Catalog=catalog;Integrated Security=SSPI\"", key.Value);
            Assert.IsNull(key.LeadingComment.Text);
        }
    }
}
