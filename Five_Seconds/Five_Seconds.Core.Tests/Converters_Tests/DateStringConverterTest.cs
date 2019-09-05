using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class RecordDateStringConverterTest
    {
        private RecordDateStringConverter recordDateStringConverter;

        [SetUp]
        public void Setup()
        {
            recordDateStringConverter = new RecordDateStringConverter();
        }
        
        [Test]
        public void Convert_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = new DateTime(1994, 2, 12, 13, 20, 49);
            // Act //
            var converted = recordDateStringConverter.Convert(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual("1994-02-12", converted);
        }

        [Test]
        public void ConvertBack_ConvertsADateToAString()
        {
            // Arrange //
            var dateTime = "1994/02/12  1:20 PM";
            // Act //
            var converted = recordDateStringConverter.ConvertBack(dateTime, null, null, null);
            // Assert //
            Assert.AreEqual(new DateTime(1994, 2, 12, 13, 20, 0), converted);
        }

        [Test]
        public void DateToString()
        {
            // Arrange
            object date = new DateTime(1994, 2, 12, 13, 20, 0);
            MethodInfo methodInfo = typeof(RecordDateStringConverter).GetMethod("DateToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { date };

            // Act //
            var converted = (string)methodInfo.Invoke(recordDateStringConverter, parameters);

            // Assert
            Assert.AreEqual("1994-02-12", converted);
        }
    }
}
