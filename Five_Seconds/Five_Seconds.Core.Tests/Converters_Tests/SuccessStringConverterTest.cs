using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class SuccessStringConverterTest
    {
        private SuccessStringConverter successStringConverter;

        [SetUp]
        public void Setup()
        {
            successStringConverter = new SuccessStringConverter();
        }

        [Test]
        public void Convert_ConvertsBooleanToString()
        {
            // Arrange //
            var boolSuccess = true;
            // Act //
            var converted = successStringConverter.Convert(boolSuccess, null, null, null);
            // Assert //
            Assert.AreEqual("성공", converted);
        }

        [Test]
        public void ConvertBack_ConvertsStringToBoolean()
        {
            // Arrange //
            var strSuccess = "실패";
            // Act //
            var converted = successStringConverter.ConvertBack(strSuccess, null, null, null);
            // Assert //
            Assert.AreEqual(false, converted);
        }

        [Test]
        public void IsSuccessToString()
        {
            // Arrange
            var isSuccess = (object)true;
            MethodInfo methodInfo = typeof(SuccessStringConverter).GetMethod("IsSuccessToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { isSuccess };

            // Act //
            var converted = (string)methodInfo.Invoke(successStringConverter, parameters);

            // Assert
            Assert.AreEqual("성공", converted);
        }
    }
}
