using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Refere.Insurance.Person.Model.Tests
{
    [TestFixture]
    internal class CNPJTests
    {

        [Test]
        [TestCase("65647062000135")]
        public void ValidCNPJShouldSplitTest(string value)
        {
            string number = CNPJ.GetNumberFrom(value);
            string digit = CNPJ.GetCheckNumberFrom(value);
            var cnpj = new CNPJ(number + digit);

            Assert.That(cnpj == value);
            Assert.That(cnpj.Number, Is.EqualTo(number));
            Assert.That(cnpj.CheckNumber, Is.EqualTo(digit));
        }

        [Test]
        [TestCase("39612247000102")]
        public void ImplicitCompareCNPJToStringTest(string value)
        {
            CNPJ number = new(value);
            Assert.That(number == value);
        }

        [Test]
        [TestCase("39612237000102")]
        public void InvalidCNPJTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CNPJ(value));
        }

        [Test]
        [TestCase("07223860000133", "07.223.860/0001-33")]
        public void CanBeFormattedTest(string value, string expected)
        {
            var number = new CNPJ(value);
            var actual = number.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("")]
        [TestCase("0722386000013")]
        [TestCase("A7223860000133")]
        public void ShouldBeNumericOfFourteenDigitsTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CNPJ(value));
        }

        [Test]
        [TestCase(null)]
        public void NewCNPJArgumentNullTest(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new CNPJ(value));
        }

        [Test]
        [TestCase(null)]
        public void CantBeNullAssignedTest(string value)
        {
            Assert.Throws<InvalidOperationException>(() => { CNPJ number = value; });
        }

        [Test]
        [TestCase("21165340000142")]
        public void CanBeNullableTest(string value)
        {
            CNPJ? number = value;

            Assert.That(number == value);
            Assert.That(number.HasValue, Is.True);

            number = null;

            Assert.That(number == null);
            Assert.That(number.HasValue, Is.False);
        }

        [Test]
        [TestCase("21165340000142")]
        public void CanBeStringAssignableTest(string value)
        {
            CNPJ number = value;
            Assert.That(number == value);
        }
    }
}
