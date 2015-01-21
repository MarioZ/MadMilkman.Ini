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

            int resultingInteger;
            file.Sections["Section"].Keys["number"].TryParseValue(out resultingInteger);
            Assert.AreEqual(10, resultingInteger);

            ulong resultingUnsignedLong;
            file.Sections["Section"].Keys["number"].TryParseValue(out resultingUnsignedLong);
            Assert.AreEqual(10ul, resultingUnsignedLong);

            double resultingDouble;
            file.Sections["Section"].Keys["number"].TryParseValue(out resultingDouble);
            Assert.AreEqual(10d, resultingDouble);

            DateTime resultingDateTime;
            file.Sections["Section"].Keys["date"].TryParseValue(out resultingDateTime);
            Assert.AreEqual(dateTime, resultingDateTime);

            TimeSpan resultingTimeSpan;
            file.Sections["Section"].Keys["time"].TryParseValue(out resultingTimeSpan);
            Assert.AreEqual(timeSpan, resultingTimeSpan);
        }
    }
}
