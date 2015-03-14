using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MadMilkman.Ini.Tests
{
    [TestFixture]
    public class IniSerializationDeserializationTests
    {
        [Test]
        public void SerializeDeserializeTest()
        {
            var file = new IniFile();
            var section = file.Sections.Add("Sample Class");

            var serializedClass = new SampleClass();
            serializedClass.Initialize();

            section.Serialize(serializedClass);
            var deserializedClass = section.Deserialize<SampleClass>();

            Assert.IsTrue(serializedClass.Equals(deserializedClass));
        }

        [Test]
        public void SerializeDeserializeAttributeTest()
        {
            var file = new IniFile();
            var section = file.Sections.Add("Sample Attributed Class");

            var serializedClass = new SampleAttributedClass();
            serializedClass.Initialize();

            section.Serialize(serializedClass);
            Assert.IsNull(section.Keys["FirstSampleProperty"]);
            Assert.IsNotNull(section.Keys["SecondSampleProperty"]);
            Assert.IsNotNull(section.Keys["3. Sample Property"]);
            Assert.IsNotNull(section.Keys["FourthSampleProperty"]);

            var deserializedClass = section.Deserialize<SampleAttributedClass>();
            Assert.AreNotEqual(serializedClass.FirstSampleProperty, deserializedClass.FirstSampleProperty);
            Assert.IsNull(deserializedClass.FirstSampleProperty);
            Assert.AreEqual(serializedClass.SecondSampleProperty, deserializedClass.SecondSampleProperty);
            Assert.AreEqual(serializedClass.ThirdSampleProperty, deserializedClass.ThirdSampleProperty);
            Assert.IsTrue(
                // null
                string.IsNullOrEmpty(serializedClass.FourthSampleProperty) &&
                // string.Empty
                string.IsNullOrEmpty(deserializedClass.FourthSampleProperty));
        }

        private class SampleClass
        {
            public char SampleChar { get; set; }
            public string SampleString { get; set; }
            public DateTime SampleDate { get; set; }
            public TimeSpan SampleTime { get; set; }
            public DayOfWeek SampleDay { get; set; }
            public byte SampleByte { get; set; }
            public double SampleDouble { get; set; }
            public int[] SampleValueArray { get; set; }
            public string[] SampleReferenceArray { get; set; }
            public List<int> SampleValueList { get; set; }
            public List<string> SampleReferenceList { get; set; }
            
            public void Initialize()
            {
                this.SampleChar = 'S';
                this.SampleString = "Sample";
                this.SampleDate = new DateTime(2000, 1, 10);
                this.SampleTime = new TimeSpan(20, 1, 10);
                this.SampleDay = DayOfWeek.Sunday;
                this.SampleByte = 200;
                this.SampleDouble = 2000.2;
                this.SampleValueArray = new int[] { 2, 20, 200 };
                this.SampleReferenceArray = new string[] { "Sample 1", "Sample 2", "Sample 3" };
                this.SampleValueList = new List<int>() { 2000, 20000, 200000 };
                this.SampleReferenceList = new List<string>() { "Sample 4", "Sample 5", "Sample 6" };
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                SampleClass sampleObj;

                if (obj == null || (sampleObj = obj as SampleClass) == null)
                    return false;

                return this.SampleChar == sampleObj.SampleChar &&
                       this.SampleString == sampleObj.SampleString &&
                       this.SampleDate == sampleObj.SampleDate &&
                       this.SampleTime == sampleObj.SampleTime &&
                       this.SampleDay == sampleObj.SampleDay &&
                       this.SampleByte == sampleObj.SampleByte &&
                       this.SampleDouble == sampleObj.SampleDouble &&
                       this.SampleValueArray.SequenceEqual(sampleObj.SampleValueArray) &&
                       this.SampleReferenceArray.SequenceEqual(sampleObj.SampleReferenceArray) &&
                       this.SampleValueList.SequenceEqual(sampleObj.SampleValueList) &&
                       this.SampleReferenceList.SequenceEqual(sampleObj.SampleReferenceList);
            }
        }

        private class SampleAttributedClass
        {
            [IniSerialization(true)]
            public string FirstSampleProperty { get; set; }
            [IniSerialization(false)]
            public string SecondSampleProperty { get; set; }
            [IniSerialization("3. Sample Property")]
            public string ThirdSampleProperty { get; set; }
            public string FourthSampleProperty { get; set; }

            public void Initialize()
            {
                this.FirstSampleProperty = "1. Sample Content";
                this.SecondSampleProperty = "2. Sample Content";
                this.ThirdSampleProperty = "3. Sample Content";
            }
        }
    }
}
