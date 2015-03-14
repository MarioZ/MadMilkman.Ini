using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class IniValueManipulationTests
    {
        [Test]
        public void ParseValueTest()
        {
            var dateTime = new DateTime(9999, 12, 31, 23, 59, 59);
            var timeSpan = new TimeSpan(10, 23, 59, 59, 999);

            var file = new IniFile();
            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "bool", bool.TrueString),
                    new IniKey(file, "number", 10.ToString()),
                    new IniKey(file, "date", dateTime.ToString()),
                    new IniKey(file, "time", timeSpan.ToString()),
                    new IniKey(file, "day", DayOfWeek.Sunday.ToString())));

            bool resultingBoolean;
            Assert.IsTrue(file.Sections["Section"].Keys["bool"].TryParseValue(out resultingBoolean));
            Assert.AreEqual(true, resultingBoolean);

            var numberKey = file.Sections["Section"].Keys["number"];

            byte resultingByte;
            Assert.IsTrue(numberKey.TryParseValue(out resultingByte));
            Assert.AreEqual((byte)10, resultingByte);

            sbyte resultingSignedByte;
            Assert.IsTrue(numberKey.TryParseValue(out resultingSignedByte));
            Assert.AreEqual((sbyte)10, resultingSignedByte);

            short resultingShort;
            Assert.IsTrue(numberKey.TryParseValue(out resultingShort));
            Assert.AreEqual((short)10, resultingShort);

            ushort resultingUnsignedShort;
            Assert.IsTrue(numberKey.TryParseValue(out resultingUnsignedShort));
            Assert.AreEqual((ushort)10, resultingUnsignedShort);

            int resultingInteger;
            Assert.IsTrue(numberKey.TryParseValue(out resultingInteger));
            Assert.AreEqual(10, resultingInteger);

            uint resultingUnsignedInteger;
            Assert.IsTrue(numberKey.TryParseValue(out resultingUnsignedInteger));
            Assert.AreEqual(10U, resultingUnsignedInteger);

            long resultingLong;
            Assert.IsTrue(numberKey.TryParseValue(out resultingLong));
            Assert.AreEqual(10L, resultingLong);

            ulong resultingUnsignedLong;
            Assert.IsTrue(numberKey.TryParseValue(out resultingUnsignedLong));
            Assert.AreEqual(10UL, resultingUnsignedLong);

            float resultingSingle;
            Assert.IsTrue(numberKey.TryParseValue(out resultingSingle));
            Assert.AreEqual(10F, resultingSingle);

            double resultingDouble;
            Assert.IsTrue(numberKey.TryParseValue(out resultingDouble));
            Assert.AreEqual(10D, resultingDouble);

            decimal resultingDecimal;
            Assert.IsTrue(numberKey.TryParseValue(out resultingDecimal));
            Assert.AreEqual(10M, resultingDecimal);

            DateTime resultingDateTime;
            Assert.IsTrue(file.Sections["Section"].Keys["date"].TryParseValue(out resultingDateTime));
            Assert.AreEqual(dateTime, resultingDateTime);

            TimeSpan resultingTimeSpan;
            Assert.IsTrue(file.Sections["Section"].Keys["time"].TryParseValue(out resultingTimeSpan));
            Assert.AreEqual(timeSpan, resultingTimeSpan);

            DayOfWeek resultingEnum;
            Assert.IsTrue(file.Sections["Section"].Keys["day"].TryParseValue(out resultingEnum));
            Assert.AreEqual(DayOfWeek.Sunday, resultingEnum);
        }

        [Test]
        public void ParseValueArrayTest()
        {
            var file = new IniFile();
            file.Sections.Add(
                new IniSection(file, "Section",
                    new IniKey(file, "integers", "{ 1 , 2 , 3 , 4 , 5 , 6 }"),
                    new IniKey(file, "invalid integers", "{ 1 , 2 , 3 , A , B , C }"),
                    new IniKey(file, "dates", "{1/1/2010, 1/1/2011, 1/1/2012, 1/1/2013}")));

            var section = file.Sections[0];

            int[] numberArray;
            Assert.IsTrue(section.Keys[0].TryParseValue(out numberArray));
            Assert.AreEqual(6, numberArray.Length);

            List<int> numberList;
            Assert.IsTrue(section.Keys[0].TryParseValue(out numberList));
            Assert.AreEqual(6, numberList.Count);

            for (int i = 0; i < 6; i++)
                Assert.IsTrue(numberArray[i] == i + 1 && numberList[i] == i + 1);

            Assert.IsFalse(section.Keys[1].TryParseValue(out numberArray));
            Assert.IsNull(numberArray);

            Assert.IsFalse(section.Keys[1].TryParseValue(out numberList));
            Assert.IsNull(numberList);

            DateTime[] dateArray;
            Assert.IsTrue(section.Keys[2].TryParseValue(out dateArray));
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(1, dateArray[i].Day);
                Assert.AreEqual(1, dateArray[i].Month);
                Assert.AreEqual(2010 + i, dateArray[i].Year);
            }
        }

        [Test]
        public void ParseValueMappingsTest()
        {
            var file = new IniFile();

            file.Sections.Add(
                new IniSection(file, "TRUE BOOLEANS",
                    new IniKey(file, "bool value", "YES"),
                    new IniKey(file, "bool value", "ON"),
                    new IniKey(file, "bool value", "1"),
                    new IniKey(file, "bool value", "TRUE")));

            file.Sections.Add(
                new IniSection(file, "FALSE BOOLEANS",
                    new IniKey(file, "bool value", "NO"),
                    new IniKey(file, "bool value", "OFF"),
                    new IniKey(file, "bool value", "0"),
                    new IniKey(file, "bool value", "FALSE")));

            file.Sections.Add(
                new IniSection(file, "DOUBLES",
                    new IniKey(file, "double value", "+∞"),
                    new IniKey(file, "double value", "-∞"),
                    new IniKey(file, "double value", "NaN")));

            file.Sections.Add(
                new IniSection(file, "ARRAYS",
                    new IniKey(file, "bool values", "{YES, ON, 1, TRUE}"),
                    new IniKey(file, "bool values", "{NO, OFF, 0, FALSE}"),
                    new IniKey(file, "double values", "{1.1, 10.1, 100.1, 1000.1, +∞}")));

            file.ValueMappings.Add("yes", true);
            file.ValueMappings.Add("on", true);
            file.ValueMappings.Add("1", true);
            file.ValueMappings.Add("no", false);
            file.ValueMappings.Add("off", false);
            file.ValueMappings.Add("0", false);

            file.ValueMappings.Add("+∞", double.PositiveInfinity);
            file.ValueMappings.Add("-∞", double.NegativeInfinity);
            file.ValueMappings.Add("NaN", double.NaN);

            foreach (var key in file.Sections["TRUE BOOLEANS"].Keys)
            {
                bool booleanResult;
                Assert.IsTrue(key.TryParseValue(out booleanResult));
                Assert.IsTrue(booleanResult);
            }
            foreach (var key in file.Sections["FALSE BOOLEANS"].Keys)
            {
                bool booleanResult;
                Assert.IsTrue(key.TryParseValue(out booleanResult));
                Assert.IsFalse(booleanResult);
            }

            double doubleResult;
            Assert.IsTrue(file.Sections["DOUBLES"].Keys[0].TryParseValue(out doubleResult));
            Assert.AreEqual(double.PositiveInfinity, doubleResult);
            Assert.IsTrue(file.Sections["DOUBLES"].Keys[1].TryParseValue(out doubleResult));
            Assert.AreEqual(double.NegativeInfinity, doubleResult);
            Assert.IsTrue(file.Sections["DOUBLES"].Keys[2].TryParseValue(out doubleResult));
            Assert.IsNaN(doubleResult);

            bool[] booleansResult;
            Assert.IsTrue(file.Sections["ARRAYS"].Keys[0].TryParseValue(out booleansResult));
            foreach (var boolean in booleansResult)
                Assert.IsTrue(boolean);
            Assert.IsTrue(file.Sections["ARRAYS"].Keys[1].TryParseValue(out booleansResult));
            foreach (var boolean in booleansResult)
                Assert.IsFalse(boolean);
            List<double> doublesResult;
            Assert.IsTrue(file.Sections["ARRAYS"].Keys[2].TryParseValue(out doublesResult));
            Assert.AreEqual(1.1, doublesResult[0]);
            Assert.AreEqual(10.1, doublesResult[1]);
            Assert.AreEqual(100.1, doublesResult[2]);
            Assert.AreEqual(1000.1, doublesResult[3]);
            Assert.AreEqual(double.PositiveInfinity, doublesResult[4]);
        }

        [Test]
        public void BindValueWithInternalDataTest()
        {
            string iniFileContent = "[Source Section]\n" +
                                    "Source Key 1 = Source Value 1\n" +
                                    "Source Key 2 = Source Value 2\n" +
                                    "Source Key 3 = Source Value 3\n" +

                                    "[Binding Section]\n" +
                                    "Source Key 4 = Source Value 4\n" +
                                    "Test Key 1 = @{Source Section|Source Key 1}\n" +
                                    "Test Key 2-3 = @{Source Section|Source Key 2} and @{Source Section|Source Key 3}\n" +
                                    "Test Key 1-4 = @{Source Section|Source Key 1} and @{Source Key 4}\n" +
                                    "Test Key X = @{Unknown}";
            
            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());
            file.ValueBinding.Bind();

            var bindedSection = file.Sections["Binding Section"];
            Assert.AreEqual("Source Value 1", bindedSection.Keys["Test Key 1"].Value);
            Assert.AreEqual("Source Value 2 and Source Value 3", bindedSection.Keys["Test Key 2-3"].Value);
            Assert.AreEqual("Source Value 1 and Source Value 4", bindedSection.Keys["Test Key 1-4"].Value);
            Assert.AreEqual("@{Unknown}", bindedSection.Keys["Test Key X"].Value);
        }

        [Test]
        public void BindValueWithExternalDataTest()
        {
            string iniFileContent = "[Binding Section]\n" +
                                    "Test Key 1 = @{First Tester}\n" +
                                    "Test Key 2-3 = @{Second Tester} and @{Third Tester}\n" +
                                    "Test Key 3-3-4 = {@{Third Tester}, @{Third Tester}, @{Fourth Tester}}\n" +
                                    "Test Key X = @{Unknown}\n" +
                                    "Test Key Nested = @{Nested @{Test}}";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());
            file.ValueBinding.Bind(
                new Dictionary<string, string>()
                {
                    {"First Tester", "Source Value 1"},
                    {"Second Tester", "Source Value 2"},
                    {"Third Tester", "Source Value 3"},
                    {"Fourth Tester", "Source Value 4"},
                    {"Test", "Tester"}
                });

            var bindedSection = file.Sections["Binding Section"];
            Assert.AreEqual("Source Value 1", bindedSection.Keys["Test Key 1"].Value);
            Assert.AreEqual("Source Value 2 and Source Value 3", bindedSection.Keys["Test Key 2-3"].Value);
            Assert.AreEqual("{Source Value 3, Source Value 3, Source Value 4}", bindedSection.Keys["Test Key 3-3-4"].Value);
            Assert.AreEqual("@{Unknown}", bindedSection.Keys["Test Key X"].Value);
            Assert.AreEqual("@{Nested Tester}", bindedSection.Keys["Test Key Nested"].Value);

            file.ValueBinding.Bind(new KeyValuePair<string, string>("Nested Tester", "Nested Source Value"));
            Assert.AreEqual("Nested Source Value", bindedSection.Keys["Test Key Nested"].Value);
        }

        [Test]
        public void BindValueCustomization()
        {
            string iniFileContent = "[Binding Customization Section]\n" +
                                    "Test Key 1 = @{First Tester}\n" +
                                    "Test Key 2-3 = @{Second Tester} and @{Third Tester}\n" +
                                    "Test Key 2-4 = @{Second Tester} and @{Fourth Tester}\n" +
                                    "Test Key X = @{Unknown}";

            IniFile file = IniUtilities.LoadIniFileContent(iniFileContent, new IniOptions());
            file.ValueBinding.Binding += (sender, e) =>
            {
                if (!e.IsValueFound)
                    e.Value = "Missing";
                else
                    switch (e.PlaceholderName)
                    {
                        case "First Tester":
                            e.Value = "Custom " + e.Value;
                            break;
                        case "Third Tester":
                            e.Value = null;
                            break;
                        case "Fourth Tester":
                            e.Value = string.Empty;
                            break;
                    }
            };

            file.ValueBinding.Bind(
                new Dictionary<string, string>()
                {
                    {"First Tester", "Source Value 1"},
                    {"Second Tester", "Source Value 2"},
                    {"Third Tester", "Source Value 3"},
                    {"Fourth Tester", "Source Value 4"}
                });

            var bindedSection = file.Sections["Binding Customization Section"];
            Assert.AreEqual("Custom Source Value 1", bindedSection.Keys["Test Key 1"].Value);
            Assert.AreEqual("Source Value 2 and @{Third Tester}", bindedSection.Keys["Test Key 2-3"].Value);
            Assert.AreEqual("Source Value 2 and ", bindedSection.Keys["Test Key 2-4"].Value);
            Assert.AreEqual("Missing", bindedSection.Keys["Test Key X"].Value);
        }
    }
}
