using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class RecordStringConverterTest
    {
        private RecordStringConverter recordStringConverter;

        [SetUp]
        public void Setup()
        {
            recordStringConverter = new RecordStringConverter();
        }

        [Test]
        public void Convert_ConvertsIntToString()
        {
            // Arrange //
            var doublePercent = 7;
            // Act //
            var converted = recordStringConverter.Convert(doublePercent, null, null, null);
            // Assert //
            Assert.AreEqual("7초", converted);
        }

        [Test]
        public void ConvertBack_ConvertsStringToInt()
        {
            // Arrange //
            var strPercent = "7초";
            // Act //
            var converted = recordStringConverter.ConvertBack(strPercent, null, null, null);
            // Assert //
            Assert.AreEqual(7, converted);
        }

        [Test]
        public void RecordToString()
        {
            // Arrange
            var record = 7 as object;
            MethodInfo methodInfo = typeof(RecordStringConverter).GetMethod("RecordToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { record };

            // Act //
            var converted = (string)methodInfo.Invoke(recordStringConverter, parameters);

            // Assert
            Assert.AreEqual("7초", converted);
        }
    }
}
