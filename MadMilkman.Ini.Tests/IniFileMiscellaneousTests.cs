using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture, Category("Parsing IniKey values")]
    public class IniFileMiscellaneousTests
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
            Assert.AreEqual(double.NaN, doubleResult);
        }
    }
}
