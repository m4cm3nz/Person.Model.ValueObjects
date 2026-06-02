using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Refere.Insurance.Person.Model.Tests
{
    [TestFixture]
    internal class CNPJTests
    {
        // ------------------------------------------------------------------ //
        // Construção e split                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("65647062000135", "656470620001", "35")]
        [TestCase("07223860000133", "072238600001", "33")]
        [TestCase("39612247000102", "396122470001", "02")]
        [TestCase("21165340000142", "211653400001", "42")]
        [TestCase("11223344550097", "112233445500", "97")]
        [TestCase("98765432100060", "987654321000", "60")]
        public void ValidCNPJShouldSplitCorrectlyTest(string raw, string expectedNumber, string expectedCheck)
        {
            var cnpj = new CNPJ(raw);

            Assert.That(cnpj.Number, Is.EqualTo(expectedNumber));
            Assert.That(cnpj.CheckNumber, Is.EqualTo(expectedCheck));
            Assert.That((string)cnpj, Is.EqualTo(raw));
        }

        // ------------------------------------------------------------------ //
        // Conversão implícita                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("39612247000102")]
        [TestCase("65647062000135")]
        public void ImplicitConversionFromStringTest(string value)
        {
            CNPJ cnpj = value;
            Assert.That(cnpj == value);
        }

        [Test]
        [TestCase("21165340000142")]
        [TestCase("98765432100060")]
        public void ImplicitConversionToStringTest(string value)
        {
            CNPJ cnpj = new(value);
            string result = cnpj;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { CNPJ _ = null!; });
        }

        // ------------------------------------------------------------------ //
        // Formatação (ToString)                                               //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("07223860000133", "07.223.860/0001-33")]
        [TestCase("65647062000135", "65.647.062/0001-35")]
        [TestCase("11223344550097", "11.223.344/5500-97")]
        [TestCase("98765432100060", "98.765.432/1000-60")]
        public void NumericCNPJFormatsCorrectlyTest(string raw, string expected)
        {
            var cnpj = new CNPJ(raw);
            Assert.That(cnpj.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // Sanitize / StripMask                                                //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("07.223.860/0001-33", "07223860000133")]
        [TestCase("65.647.062/0001-35", "65647062000135")]
        [TestCase("11.223.344/5500-97", "11223344550097")]
        public void MaskedInputIsAcceptedTest(string masked, string raw)
        {
            var cnpj = new CNPJ(masked);
            Assert.That((string)cnpj, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("07.223.860/0001-33", "07223860000133")]
        [TestCase("65.647.062/0001-35", "65647062000135")]
        public void StripMaskRemovesFormattingCharactersTest(string masked, string expected)
        {
            Assert.That(CNPJ.StripMask(masked), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("07.223.860/0001-33")]
        [TestCase("07223860000133")]
        [TestCase("98765432100060")]
        public void IsValidAcceptsMaskedAndRawTest(string value)
        {
            Assert.That(CNPJ.IsValid(value), Is.True);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(CNPJ.IsValid(null!), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("21165340000142")]
        [TestCase("39612247000102")]
        public void CNPJCanBeSetToNullableBehaviorTest(string value)
        {
            CNPJ? cnpj = value;
            Assert.That(cnpj.Value == value);
            Assert.That(cnpj.HasValue, Is.True);

            cnpj = null;
            Assert.That(cnpj == null);
            Assert.That(cnpj.HasValue, Is.False);
        }

        [Test]
        public void NullCNPJShouldNotHaveValueTest()
        {
            CNPJ? cnpj = null;
            Assert.That(cnpj.HasValue, Is.False);
        }
        
        [Test]
        public void NullCNPJShouldThrowWhenAccessingValueTest()
        {
            CNPJ? cnpj = null;
            Assert.Throws<InvalidOperationException>(() => { var _ = cnpj.Value; });
        }

        // ------------------------------------------------------------------ //
        // Igualdade                                                           //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("07223860000133")]
        [TestCase("98765432100060")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            CNPJ a = new(value);
            CNPJ b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        [TestCase("07223860000133", "65647062000135")]
        [TestCase("39612247000102", "11223344550097")]
        public void InequalityBetweenDifferentCNPJsTest(string a, string b)
        {
            CNPJ cnpjA = new(a);
            CNPJ cnpjB = new(b);

            Assert.That(cnpjA != cnpjB, Is.True);
        }

        // ------------------------------------------------------------------ //
        // Sequência homogênea                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("00000000000000")]
        [TestCase("11111111111111")]
        [TestCase("99999999999999")]
        public void HomogeneousSequenceShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CNPJ(value));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                               //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CNPJ(null!));
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException — formato inválido                     //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("")]                  // vazio
        [TestCase("0722386000013")]     // 13 chars
        [TestCase("072238600001333")]   // 15 chars
        [TestCase("a7223860000133")]    // minúscula — não normalizado na v2
        [TestCase("07223860000!33")]    // caractere especial
        [TestCase("0722386000013A")]    // letra no dígito verificador
        [TestCase("1a2b3c4d5e6f78")]   // alfanumérico com minúsculas
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CNPJ(value));
        }

        // ------------------------------------------------------------------ //
        // InvalidCastException — formato ok, DV incorreto                    //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("39612237000102")]    // numérico — DV incorreto
        [TestCase("07223860000134")]    // numérico — último dígito errado
        [TestCase("00000000000100")]    // numérico — DV incorreto
        [TestCase("11223344550001")]    // numérico — DV incorreto
        [TestCase("A0000000000199")]    // alfanumérico — DV incorreto
        [TestCase("AB123456000199")]    // alfanumérico — DV incorreto
        public void InvalidCheckDigitShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CNPJ(value));
        }

        // ------------------------------------------------------------------ //
        // Alfanumérico válido — novo formato (julho/2026)                    //
        // CNPJs gerados via algoritmo oficial (ASCII-48, módulo 11)          //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("A0000000000113", "A00000000001", "13")]
        [TestCase("AB123456000110", "AB1234560001", "10")]
        [TestCase("AABBCC11220088", "AABBCC112200", "88")]
        [TestCase("1A2B3C4D5E6F34", "1A2B3C4D5E6F", "34")]
        [TestCase("ZZ999999000191", "ZZ9999990001", "91")]
        [TestCase("AA000000000108", "AA0000000001", "08")]
        [TestCase("ABCDEF12340026", "ABCDEF123400", "26")]
        [TestCase("A1B2C3D4E5F668", "A1B2C3D4E5F6", "68")]
        [TestCase("ZZZZZZ00000151", "ZZZZZZ000001", "51")]
        [TestCase("ABCDEFABCDEF56", "ABCDEFABCDEF", "56")]
        public void ValidAlphanumericCNPJShouldSplitCorrectlyTest(string raw, string expectedNumber, string expectedCheck)
        {
            var cnpj = new CNPJ(raw);

            Assert.That(cnpj.Number, Is.EqualTo(expectedNumber));
            Assert.That(cnpj.CheckNumber, Is.EqualTo(expectedCheck));
            Assert.That((string)cnpj, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("A0000000000113", "A0.000.000/0001-13")]
        [TestCase("AB123456000110", "AB.123.456/0001-10")]
        [TestCase("1A2B3C4D5E6F34", "1A.2B3.C4D/5E6F-34")]
        [TestCase("ABCDEFABCDEF56", "AB.CDE.FAB/CDEF-56")]
        public void AlphanumericCNPJFormatsCorrectlyTest(string raw, string expected)
        {
            var cnpj = new CNPJ(raw);
            Assert.That(cnpj.ToString(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("A0000000000113")]
        [TestCase("AB123456000110")]
        [TestCase("1A2B3C4D5E6F34")]
        [TestCase("ABCDEFABCDEF56")]
        public void IsValidAcceptsAlphanumericTest(string value)
        {
            Assert.That(CNPJ.IsValid(value), Is.True);
        }
    }
}