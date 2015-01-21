using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMilkman.Ini.Tests
{
    [TestClass]
    public class IniFileMiscellaneousTests
    {
        [TestMethod]
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
                    new IniKey(file, "time", timeSpan.ToString())));

            bool resultingBoolean;
            file.Sections["Section"].Keys["bool"].TryParseValue(out resultingBoolean);
            Assert.AreEqual(true, resultingBoolean);

            var numberKey = file.Sections["Section"].Keys["number"];

            byte resultingByte;
            numberKey.TryParseValue(out resultingByte);
            Assert.AreEqual((byte)10, resultingByte);

            sbyte resultingSignedByte;
            numberKey.TryParseValue(out resultingSignedByte);
            Assert.AreEqual((sbyte)10, resultingSignedByte);

            short resultingShort;
            numberKey.TryParseValue(out resultingShort);
            Assert.AreEqual((short)10, resultingShort);

            ushort resultingUnsignedShort;
            numberKey.TryParseValue(out resultingUnsignedShort);
            Assert.AreEqual((ushort)10, resultingUnsignedShort);

            int resultingInteger;
            numberKey.TryParseValue(out resultingInteger);
            Assert.AreEqual(10, resultingInteger);

            uint resultingUnsignedInteger;
            numberKey.TryParseValue(out resultingUnsignedInteger);
            Assert.AreEqual(10U, resultingUnsignedInteger);

            long resultingLong;
            numberKey.TryParseValue(out resultingLong);
            Assert.AreEqual(10L, resultingLong);

            ulong resultingUnsignedLong;
            numberKey.TryParseValue(out resultingUnsignedLong);
            Assert.AreEqual(10UL, resultingUnsignedLong);

            float resultingSingle;
            numberKey.TryParseValue(out resultingSingle);
            Assert.AreEqual(10F, resultingSingle);

            double resultingDouble;
            numberKey.TryParseValue(out resultingDouble);
            Assert.AreEqual(10D, resultingDouble);

            DateTime resultingDateTime;
            file.Sections["Section"].Keys["date"].TryParseValue(out resultingDateTime);
            Assert.AreEqual(dateTime, resultingDateTime);

            TimeSpan resultingTimeSpan;
            file.Sections["Section"].Keys["time"].TryParseValue(out resultingTimeSpan);
            Assert.AreEqual(timeSpan, resultingTimeSpan);
        }
    }
}
