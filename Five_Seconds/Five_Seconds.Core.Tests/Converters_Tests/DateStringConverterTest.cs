using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class DateStringConverterTest
    {
        private DateStringConverter dateStringConverter;

        [SetUp]
        public void Setup()
        {
            dateStringConverter = new DateStringConverter();
        }
        
        [Test]
        public void Convert_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = new DateTime(1994, 2, 12, 13, 20, 49);
            // Act //
            var converted = dateStringConverter.Convert(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual("1994-02-12", converted);
        }

        [Test]
        public void ConvertBack_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = "1994/02/12  1:20 PM";
            // Act //
            var converted = dateStringConverter.ConvertBack(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual(new DateTime(1994, 2, 12, 13, 20, 0), converted);
        }

        [Test]
        public void DateToString()
        {
            // Arrange
            object date = new DateTime(1994, 2, 12, 13, 20, 0);
            MethodInfo methodInfo = typeof(DateStringConverter).GetMethod("DateToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { date };

            // Act //
            var converted = (string)methodInfo.Invoke(dateStringConverter, parameters);

            // Assert
            Assert.AreEqual("1994-02-12", converted);
        }
    }
}
