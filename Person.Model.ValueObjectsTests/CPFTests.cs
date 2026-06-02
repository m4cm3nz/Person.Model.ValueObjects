using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class CPFTests
    {
        // ------------------------------------------------------------------ //
        // Construction and split                                              //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("38124036098", "381240360", "98")]
        [TestCase("10411981080", "104119810", "80")]
        [TestCase("12738386024", "127383860", "24")]
        public void ValidCpfShouldSplitCorrectlyTest(string raw, string expectedNumber, string expectedCheck)
        {
            var cpf = new CPF(raw);

            Assert.That(cpf.Number, Is.EqualTo(expectedNumber));
            Assert.That(cpf.CheckNumber, Is.EqualTo(expectedCheck));
            Assert.That((string)cpf, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("104.119.810-80", "10411981080")]
        [TestCase("381.240.360-98", "38124036098")]
        public void MaskedInputIsAcceptedTest(string masked, string expectedRaw)
        {
            var cpf = new CPF(masked);

            Assert.That((string)cpf, Is.EqualTo(expectedRaw));
        }

        // ------------------------------------------------------------------ //
        // Implicit conversion                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("38124036098")]
        [TestCase("12738386024")]
        public void ImplicitConversionFromStringTest(string value)
        {
            CPF cpf = value;

            Assert.That((string)cpf, Is.EqualTo(value));
        }

        [Test]
        [TestCase("38124036098")]
        [TestCase("10411981080")]
        public void ImplicitConversionToStringTest(string value)
        {
            CPF cpf = new(value);
            string result = cpf;

            Assert.That(result, Is.EqualTo(value));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("10411981080", "104.119.810-80")]
        [TestCase("38124036098", "381.240.360-98")]
        [TestCase("12738386024", "127.383.860-24")]
        public void ToStringFormatsCorrectlyTest(string raw, string expected)
        {
            var cpf = new CPF(raw);

            Assert.That(cpf.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // StripMask / IsValid                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("104.119.810-80", "10411981080")]
        [TestCase("381.240.360-98", "38124036098")]
        public void StripMaskRemovesFormattingCharactersTest(string masked, string expected)
        {
            Assert.That(CPF.StripMask(masked), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("38124036098")]
        [TestCase("104.119.810-80")]
        public void IsValidReturnsTrueForValidInputTest(string value)
        {
            Assert.That(CPF.IsValid(value), Is.True);
        }

        [Test]
        [TestCase("23412412412")]
        [TestCase("00000000000")]
        [TestCase("")]
        public void IsValidReturnsFalseForInvalidInputTest(string value)
        {
            Assert.That(CPF.IsValid(value), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(CPF.IsValid(null), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("10411981080")]
        public void NullableCpfBehaviorTest(string value)
        {
            CPF? cpf = value;

            Assert.That(cpf.HasValue, Is.True);
            Assert.That((string)cpf.Value, Is.EqualTo(value));

            cpf = null;

            Assert.That(cpf.HasValue, Is.False);
        }

        [Test]
        public void NullableCpfValueThrowsWhenNullTest()
        {
            CPF? cpf = null;

            Assert.Throws<InvalidOperationException>(() => { var _ = cpf.Value; });
        }

        // ------------------------------------------------------------------ //
        // Equality                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("38124036098")]
        [TestCase("10411981080")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            CPF a = new(value);
            CPF b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        [TestCase("38124036098", "10411981080")]
        [TestCase("10411981080", "12738386024")]
        public void InequalityBetweenDifferentCpfsTest(string a, string b)
        {
            CPF cpfA = new(a);
            CPF cpfB = new(b);

            Assert.That(cpfA != cpfB, Is.True);
        }

        // ------------------------------------------------------------------ //
        // Homogeneous sequences                                               //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("00000000000")]
        [TestCase("11111111111")]
        [TestCase("99999999999")]
        public void HomogeneousSequenceShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CPF(value));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                               //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CPF(null));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { CPF cpf = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("")]
        [TestCase("1234567890")]
        [TestCase("123456789012")]
        [TestCase("A1234567890")]
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CPF(value));
        }

        // ------------------------------------------------------------------ //
        // InvalidCastException                                                //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("23412412412")]
        [TestCase("10411981081")]
        public void InvalidCheckDigitShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CPF(value));
        }
    }
}
