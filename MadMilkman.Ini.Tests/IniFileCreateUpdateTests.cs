using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class IniFileCreateUpdateTests
    {
        [Test]
        public void CreateIniFileBasicTest()
        {
            var file = new IniFile();

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

        [Test]
        public void CreateIniFileAdvanceTest()
        {
            var file = new IniFile();

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

        [Test]
        public void CreateIniFileExpertTest()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "Key 2.1.", "Value 2.1." },
                { "Key 2.2.", "Value 2.2." },
                { "Key 2.3.", "Value 2.3." },
                { "Key 2.4.", "Value 2.4." }
            };

            var file = new IniFile();
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

        [Test]
        public void UpdateIniFileLinqTest()
        {
            var file = new IniFile();

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

        [Test]
        public void UpdateIniFileCorrectTest()
        {
            var file = new IniFile();
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

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void UpdateIniFileIncorrectBasicTest()
        {
            var file = new IniFile();

            var section = file.Sections.Add("Section");

            file.Sections.Add(section);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void UpdateIniFileIncorrectAdvanceTest()
        {
            var file = new IniFile(new IniOptions());

            var section = file.Sections.Add("Section");

            var newFile = new IniFile(new IniOptions());
            newFile.Sections.Add(section);
        }

        [Test]
        public void UpdateIniFileCopyTest()
        {
            var file = new IniFile();
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

            var newFile = new IniFile();
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

        [Test]
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

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void UpdateDisallowDuplicatesSectionsTest()
        {
            var options = new IniOptions()
            {
                SectionDuplicate = IniDuplication.Disallowed
            };

            var file = new IniFile(options);
            file.Sections.Add("SECTION1").Keys.Add("KEY1");
            var section = new IniSection(file, "SECTION1");
            file.Sections.Add(section);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void UpdateDisallowDuplicatesKeysTest()
        {
            var options = new IniOptions()
            {
                KeyDuplicate = IniDuplication.Disallowed,
            };

            var file = new IniFile(options);
            file.Sections.Add("SECTION1").Keys.Add("KEY1");
            file.Sections.Add("SECTION1").Keys.Add("KEY1");

            Assert.AreEqual(2, file.Sections.Count);
            Assert.AreEqual(1, file.Sections[0].Keys.Count);
            Assert.AreEqual(1, file.Sections[1].Keys.Count);

            var key = new IniKey(file, "KEY1");

            file.Sections[0].Keys.Add(key);
        }

        public void UpdateArrayIndexerTest()
        {
            var file = new IniFile();
            file.Sections.Add(
                new IniSection(file, "SECTION 0",
                    new IniKey(file, "UNKNOWN"),
                    new IniKey(file, "KEY 1"),
                    new IniKey(file, "KEY 2"),
                    new IniKey(file, "UNKNOWN")));

            file.Sections.Add(new IniSection(file, "UNKNOWN"));
            file.Sections.Add(new IniSection(file, "SEC 2"));
            file.Sections.Add(new IniSection(file, "UNKNOWN"));

            bool isOneKeyNull = false;
            foreach (var key in file.Sections[0].Keys["UNKNOWN", "UNKNOWN", "UNKNOWN", "KEY 2", "KEY 1"])
            {
                if (key == null)
                {
                    if (isOneKeyNull) Assert.Fail();
                    isOneKeyNull = true;
                }
                else
                    key.Value = "VALUE " + key.ParentCollection.IndexOf(key);
            }

            if (!isOneKeyNull) Assert.Fail();
            for (int i = 0; i < file.Sections[0].Keys.Count; i++)
                Assert.AreEqual("VALUE " + i, file.Sections[0].Keys[i].Value);

            bool isOneSectionRenamed = false;
            foreach (var section in file.Sections["UNKNOWN", "UNKNOWN", "SEC 2", "SECTION 2"])
            {
                string newSectionName = "SECTION " + section.ParentCollection.IndexOf(section);

                if (section.Name == newSectionName)
                {
                    if (isOneSectionRenamed) Assert.Fail();
                    isOneSectionRenamed = true;
                }
                else
                    section.Name = "SECTION " + section.ParentCollection.IndexOf(section);
            }

            if (!isOneSectionRenamed) Assert.Fail();
            for (int i = 0; i < file.Sections.Count; i++)
                Assert.AreEqual("SECTION " + i, file.Sections[i].Name);

            var order = new IniSection(file, "ORDER",
                            new IniKey(file, "A"),
                            new IniKey(file, "B"),
                            new IniKey(file, "C"),
                            new IniKey(file, "A"),
                            new IniKey(file, "B"),
                            new IniKey(file, "C"));
            file.Sections.Add(order);

            int returnedIndex = 0;
            foreach (var key in order.Keys["A", "A", "B", "B", "C", "C"])
                key.Value = (returnedIndex++).ToString();

            Assert.AreEqual("0", order.Keys[0].Value);
            Assert.AreEqual("1", order.Keys[3].Value);
            Assert.AreEqual("2", order.Keys[1].Value);
            Assert.AreEqual("3", order.Keys[4].Value);
            Assert.AreEqual("4", order.Keys[2].Value);
            Assert.AreEqual("5", order.Keys[5].Value);
        }

        [Test]
        public void IniItemParentsTest()
        {
            var file = new IniFile();
            var section = new IniSection(file, "Section");
            var key = new IniKey(file, "Key");

            Assert.AreSame(file, section.ParentFile);
            Assert.AreSame(file, key.ParentFile);

            Assert.IsNull(section.ParentCollection);
            Assert.IsNull(key.ParentCollection);
            Assert.IsNull(key.ParentSection);

            section.Keys.Add(key);
            Assert.AreSame(section.Keys, key.ParentCollection);
            Assert.AreSame(section, key.ParentSection);

            file.Sections.Add(section);
            Assert.AreSame(file.Sections, section.ParentCollection);

            file.Sections.Remove(section);
            Assert.IsNull(section.ParentCollection);

            section.Keys.Remove(key);
            Assert.IsNull(key.ParentCollection);
            Assert.IsNull(key.ParentSection);
        }
    }
}
