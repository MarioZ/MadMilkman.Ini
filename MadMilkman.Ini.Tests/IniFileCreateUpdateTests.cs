using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMilkman.Ini.Tests
{
    [TestClass]
    public class IniFileCreateUpdateTests
    {
        [TestMethod]
        public void CreateIniFileBasicTest()
        {
            var file = new IniFile(new IniOptions());

            var section1 = file.Sections.Add("Section 1.");
            var section2 = file.Sections.Add("Section 2.");
            var section3 = file.Sections.Add("Section 3.");

            var key1 = section1.Keys.Add("Key 1.", "Value 1.");
            var key2 = section2.Keys.Add("Key 2.", "Value 2.");
            var key3 = section3.Keys.Add("Key 3.", "Value 3.");

            Assert.AreEqual(3, file.Sections.Count);

            Assert.AreEqual("Section 1.", section1.Name);
            Assert.AreEqual("Section 2.", section2.Name);
            Assert.AreEqual("Section 3.", section3.Name);

            Assert.AreEqual(1, section1.Keys.Count);
            Assert.AreEqual(1, section2.Keys.Count);
            Assert.AreEqual(1, section3.Keys.Count);

            Assert.AreEqual("Key 1.", key1.Name);
            Assert.AreEqual("Value 1.", key1.Value);
            Assert.AreEqual("Key 2.", key2.Name);
            Assert.AreEqual("Value 2.", key2.Value);
            Assert.AreEqual("Key 3.", key3.Name);
            Assert.AreEqual("Value 3.", key3.Value);
        }

        [TestMethod]
        public void CreateIniFileAdvanceTest()
        {
            var file = new IniFile(new IniOptions());

            file.Sections.Add(
                new IniSection(file, "Section 1.",
                    new IniKey(file, "Key 1.1.") { Value = "Value 1.1." },
                    new IniKey(file, "Key 1.2.") { Value = "Value 1.2." }));
            file.Sections.Add(
                new IniSection(file, "Section 2.",
                    new IniKey(file, "Key 2.1.") { Value = "Value 2.1." },
                    new IniKey(file, "Key 2.2.") { Value = "Value 2.2." }));
            file.Sections.Add(
                new IniSection(file, "Section 3.",
                    new IniKey(file, "Key 3.1.") { Value = "Value 3.1." },
                    new IniKey(file, "Key 3.2.") { Value = "Value 3.2." }));

            Assert.AreEqual(3, file.Sections.Count);

            Assert.AreEqual("Section 1.", file.Sections[0].Name);
            Assert.AreEqual("Section 2.", file.Sections[1].Name);
            Assert.AreEqual("Section 3.", file.Sections[2].Name);

            Assert.AreEqual(2, file.Sections[0].Keys.Count);
            Assert.AreEqual(2, file.Sections[1].Keys.Count);
            Assert.AreEqual(2, file.Sections[2].Keys.Count);

            Assert.AreEqual("Key 1.1.", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("Value 1.1.", file.Sections[0].Keys[0].Value);
            Assert.AreEqual("Key 1.2.", file.Sections[0].Keys[1].Name);
            Assert.AreEqual("Value 1.2.", file.Sections[0].Keys[1].Value);
            Assert.AreEqual("Key 2.1.", file.Sections[1].Keys[0].Name);
            Assert.AreEqual("Value 2.1.", file.Sections[1].Keys[0].Value);
            Assert.AreEqual("Key 2.2.", file.Sections[1].Keys[1].Name);
            Assert.AreEqual("Value 2.2.", file.Sections[1].Keys[1].Value);
            Assert.AreEqual("Key 3.1.", file.Sections[2].Keys[0].Name);
            Assert.AreEqual("Value 3.1.", file.Sections[2].Keys[0].Value);
            Assert.AreEqual("Key 3.2.", file.Sections[2].Keys[1].Name);
            Assert.AreEqual("Value 3.2.", file.Sections[2].Keys[1].Value);
        }

        [TestMethod]
        public void CreateIniFileExpertTest()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "Key 2.1.", "Value 2.1." },
                { "Key 2.2.", "Value 2.2." },
                { "Key 2.3.", "Value 2.3." },
                { "Key 2.4.", "Value 2.4." }
            };

            var file = new IniFile(new IniOptions());
            file.Sections.Add("Section 1.").Keys.Add(new KeyValuePair<string, string>("Key 1.1.", "Value 1.1."));
            file.Sections.Add("Section 2.", dictionary);

            Assert.AreEqual(2, file.Sections.Count);

            Assert.AreEqual("Section 1.", file.Sections[0].Name);
            Assert.AreEqual("Section 2.", file.Sections[1].Name);

            Assert.AreEqual(1, file.Sections[0].Keys.Count);
            Assert.AreEqual(4, file.Sections[1].Keys.Count);

            Assert.AreEqual("Key 1.1.", file.Sections[0].Keys[0].Name);
            Assert.AreEqual("Value 1.1.", file.Sections[0].Keys[0].Value);
            Assert.AreEqual("Key 2.1.", file.Sections[1].Keys[0].Name);
            Assert.AreEqual("Value 2.1.", file.Sections[1].Keys[0].Value);
            Assert.AreEqual("Key 2.2.", file.Sections[1].Keys[1].Name);
            Assert.AreEqual("Value 2.2.", file.Sections[1].Keys[1].Value);
            Assert.AreEqual("Key 2.3.", file.Sections[1].Keys[2].Name);
            Assert.AreEqual("Value 2.3.", file.Sections[1].Keys[2].Value);
            Assert.AreEqual("Key 2.4.", file.Sections[1].Keys[3].Name);
            Assert.AreEqual("Value 2.4.", file.Sections[1].Keys[3].Value);
        }

        [TestMethod]
        public void UpdateIniFileLinqTest()
        {
            var file = new IniFile(new IniOptions());

            var section1 = file.Sections.Add("Section 1.");

            var key1 = section1.Keys.Add("Key 1.1.", "Value 1.1.");
            var key2 = section1.Keys.Add("Key 1.2.", "Value 1.2.");

            foreach (var key in section1.Keys.Where(k => k.Name.Contains("1")))
                key.Name = key.Name.Replace("Key 1.", "First Section Key ");

            Assert.AreEqual("First Section Key 1.", section1.Keys[0].Name);
            Assert.AreEqual("First Section Key 2.", section1.Keys[1].Name);

            var values = from key in section1.Keys select key.Value;

            Assert.AreEqual(2, values.Count());
            Assert.AreEqual("Value 1.1.", values.First());
            Assert.AreEqual("Value 1.2.", values.Last());
        }

        [TestMethod]
        public void UpdateIniFileCorrectTest()
        {
            var file = new IniFile(new IniOptions());
            var section = new IniSection(file, "Section",
                              new IniKey(file, "Key"));

            file.Sections.Add(section);
            Assert.AreEqual(1, file.Sections.Count);

            file.Sections.Clear();
            Assert.AreEqual(0, file.Sections.Count);

            file.Sections.Add(section);
            Assert.AreEqual(1, file.Sections.Count);

            var key = section.Keys[0];
            section.Keys.RemoveAt(0);
            Assert.AreEqual(0, section.Keys.Count);

            section.Keys.Add(key);
            Assert.AreEqual(1, section.Keys.Count);
        }

        [TestMethod]
        public void UpdateIniFileIncorrectTest()
        {
            var file = new IniFile(new IniOptions());

            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key")));
            var section = file.Sections[0];

            try
            {
                file.Sections.Add(section);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, file.Sections.Count);
            }

            var newFile = new IniFile(new IniOptions());

            try
            {
                newFile.Sections.Add(section);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(0, newFile.Sections.Count);
            }
        }

        [TestMethod]
        public void UpdateIniFileCopyTest()
        {
            var file = new IniFile(new IniOptions());
            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "Key", "Value")));
            var section = file.Sections[0];
            file.Sections.Add(section.Copy());

            Assert.AreEqual(2, file.Sections.Count);
            
            section.Name = "S";
            section.Keys[0].Name = "K";
            section.Keys[0].Value = "V";
            section = file.Sections[1];

            Assert.AreEqual("Section", section.Name);
            Assert.AreEqual("Key", section.Keys[0].Name);
            Assert.AreEqual("Value", section.Keys[0].Value);

            var newFile = new IniFile(new IniOptions());
            newFile.Sections.Add(section.Copy(newFile));

            Assert.AreEqual(1, newFile.Sections.Count);

            section.Name = "S";
            section.Keys[0].Name = "K";
            section.Keys[0].Value = "V";
            section = newFile.Sections[0];

            Assert.AreEqual("Section", section.Name);
            Assert.AreEqual("Key", section.Keys[0].Name);
            Assert.AreEqual("Value", section.Keys[0].Value);
            
            section.Keys.Add(section.Keys[0].Copy());
            Assert.AreEqual(2, section.Keys.Count);
        }

        [TestMethod]
        public void UpdateIgnoreDuplicatesTest()
        {
            var options = new IniOptions()
            {
                KeyDuplicate = IniDuplication.Ignored,
                KeyNameCaseSensitive = true,
                SectionDuplicate = IniDuplication.Ignored,
                SectionNameCaseSensitive = false
            };

            var file = new IniFile(options);
            file.Sections.Add(
                new IniSection(file, "SECTION",
                    new IniKey(file, "KEY"),
                    new IniKey(file, "key")));
            file.Sections.Add(
                new IniSection(file, "section"));

            Assert.AreEqual(1, file.Sections.Count);
            Assert.AreEqual(2, file.Sections[0].Keys.Count);
            Assert.IsTrue(file.Sections[0].Keys.Contains("KEY"));
            Assert.IsTrue(file.Sections[0].Keys.Contains("key"));

            Assert.AreEqual("key", file.Sections[0].Keys[1].Name);
            file.Sections[0].Keys[1].Name = "KEY";
            Assert.AreNotEqual("KEY", file.Sections[0].Keys[1].Name);

            file.Sections[0].Keys.Insert(0, "key");
            Assert.AreEqual(2, file.Sections[0].Keys.Count);
            Assert.AreNotEqual("key", file.Sections[0].Keys[0].Name);

            file.Sections[0].Keys.Remove("KEY");
            Assert.IsFalse(file.Sections.Contains("KEY"));

            Assert.AreEqual("key", file.Sections[0].Keys[0].Name);
            file.Sections[0].Keys[0].Name = "KEY";
            Assert.AreEqual("KEY", file.Sections[0].Keys[0].Name);
        }

        [TestMethod]
        public void UpdateDisallowDuplicatesTest()
        {
            var options = new IniOptions()
            {
                KeyDuplicate = IniDuplication.Disallowed,
                SectionDuplicate = IniDuplication.Disallowed,
            };

            var file = new IniFile(options);
            file.Sections.Add("SECTION1").Keys.Add("KEY1");
            var section = new IniSection(file, "SECTION1");

            try
            {
                file.Sections.Add(section);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, file.Sections.Count);
            }

            section.Name = "SECTION2";
            file.Sections.Add(section);
            Assert.AreEqual(2, file.Sections.Count);
            var key = new IniKey(file, "KEY1");

            try
            {
                file.Sections[0].Keys.Add(key);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, file.Sections[0].Keys.Count);
            }

            file.Sections[1].Keys.Add(key);
            Assert.AreEqual(1, file.Sections[1].Keys.Count);
        }
    }
}
