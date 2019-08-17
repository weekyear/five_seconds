using System;
using System.Reflection;
using Five_Seconds.Converters;
using NUnit.Framework;

namespace Five_Seconds.Core.Tests.Converters_Tests
{
    [TestFixture]
    public class PercentStringConverterTest
    {
        private PercentStringConverter percentStringConverter;

        [SetUp]
        public void Setup()
        {
            percentStringConverter = new PercentStringConverter();
        }

        [Test]
        public void Convert_ConvertsDoubleToString()
        {
            // Arrange //
            var doublePercent = 0.728;
            // Act //
            var converted = percentStringConverter.Convert(doublePercent, null, null, null);
            // Assert //
            Assert.AreEqual("72.8%", converted);
        }

        [Test]
        public void ConvertBack_ConvertsStringToDouble()
        {
            // Arrange //
            var strPercent = "72.8%";
            // Act //
            var converted = percentStringConverter.ConvertBack(strPercent, null, null, null);
            // Assert //
            Assert.AreEqual(0.728, converted);
        }

        [Test]
        public void PercentToString()
        {
            // Arrange
            var doublePercent = 0.728 as object;
            MethodInfo methodInfo = typeof(PercentStringConverter).GetMethod("PercentToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { doublePercent };

            // Act //
            var converted = (string)methodInfo.Invoke(percentStringConverter, parameters);

            // Assert
            Assert.AreEqual("72.8%", converted);
        }
    }
}
