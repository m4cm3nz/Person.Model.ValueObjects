using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Refere.Insurance.Person.Model.Tests
{
    [TestFixture]
    public class CPFTests
    {
        [Test]
        [TestCase("38124036098")]
        public void ValidCpfShouldSplitTest(string value)
        {
            string number = CPF.GetNumberFrom(value);
            string digit = CPF.GetCheckNumberFrom(value);
            var cpf = new CPF(number + digit);

            Assert.That(cpf == value);
            Assert.That(cpf.Number, Is.EqualTo(number));
            Assert.That(cpf.CheckNumber, Is.EqualTo(digit));
        }

        [Test]
        [TestCase("12738386024")]
        public void ImplicitCompareCpfToStringTest(string value)
        {
            CPF cpf = new CPF(value);
            Assert.That(cpf == value);
        }

        [Test]
        [TestCase("23412412412")]
        public void InvalidCpfTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CPF(value));
        }

        [Test]
        [TestCase("10411981080", "104.119.810-80")]
        public void CanBeFormattedTest(string value, string expected)
        {
            var cpf = new CPF(value);
            var actual = cpf.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("")]
        [TestCase("1234567890")]
        [TestCase("A1234567890")]
        [TestCase("1234567890123")]
        public void ShouldBeNumericOfElevenDigitsTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CPF(value));
        }

        [Test]
        [TestCase(null)]
        public void NewCpfArgumentNullTest(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new CPF(value));
        }

        [Test]
        [TestCase(null)]
        public void CantBeNullAssignedTest(string value)
        {
            Assert.Throws<InvalidOperationException>(() => { CPF cpf = value; });
        }

        [Test]
        [TestCase("10411981080")]
        public void CanBeNullableTest(string value)
        {
            CPF? cpf = value;

            Assert.That(cpf == value);
            Assert.That(cpf.HasValue, Is.True);

            cpf = null;

            Assert.That(cpf == null);
            Assert.That(cpf.HasValue, Is.False);
        }

        [Test]
        [TestCase("10411981080")]
        public void CanBeStringAssignableTest(string value)
        {
            CPF cpf = value;
            Assert.That(cpf == value);
        }
    }
}