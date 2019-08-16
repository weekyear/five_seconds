using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class TimeStringConverterTest
    {
        private TimeStringConverter timeStringConverter;

        [SetUp]
        public void Setup()
        {
            timeStringConverter = new TimeStringConverter();
        }

        [Test]
        public void Convert_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = new TimeSpan(13, 20, 0);
            // Act //
            var converted = timeStringConverter.Convert(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual("오후 1:20", converted);
        }

        [Test]
        public void ConvertBack_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = "오후 1:20";
            // Act //
            var converted = timeStringConverter.ConvertBack(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual(new TimeSpan(13, 20, 0), converted);
        }

        [Test]
        public void TimeToString()
        {
            // Arrange
            object date = new TimeSpan(13, 20, 0);
            MethodInfo methodInfo = typeof(TimeStringConverter).GetMethod("TimeToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { date };

            // Act //
            var converted = (string)methodInfo.Invoke(timeStringConverter, parameters);

            // Assert
            Assert.AreEqual("오후 1:20", converted);
        }
    }
}
