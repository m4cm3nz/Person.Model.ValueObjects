using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class CnhTests
    {
        // ------------------------------------------------------------------ //
        // Construction                                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735264")]
        [TestCase("12345678900")]
        [TestCase("20000001107")] // flag case: first digit overflows (r1 >= 10)
        [TestCase("15705560294")]
        public void ValidCnhConstructionTest(string raw)
        {
            var cnh = new CNH(raw);

            Assert.That((string)cnh, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("  84718735264  ")]
        [TestCase(" 12345678900")]
        [TestCase("20000001107 ")]
        public void WhitespacePaddedInputIsAcceptedTest(string padded)
        {
            var cnh = new CNH(padded);

            Assert.That((string)cnh, Is.EqualTo(padded.Trim()));
        }

        // ------------------------------------------------------------------ //
        // Implicit conversion                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735264")]
        [TestCase("12345678900")]
        public void ImplicitConversionFromStringTest(string value)
        {
            CNH cnh = value;

            Assert.That((string)cnh, Is.EqualTo(value));
        }

        [Test]
        [TestCase("84718735264")]
        [TestCase("20000001107")]
        public void ImplicitConversionToStringTest(string value)
        {
            CNH cnh = new(value);
            string result = cnh;

            Assert.That(result, Is.EqualTo(value));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735264")]
        [TestCase("12345678900")]
        [TestCase("20000001107")]
        [TestCase("15705560294")]
        public void ToStringReturnsRawDigitsTest(string raw)
        {
            var cnh = new CNH(raw);

            // CNH has no standard display mask — ToString returns the 11-digit string.
            Assert.That(cnh.ToString(), Is.EqualTo(raw));
        }

        // ------------------------------------------------------------------ //
        // StripMask                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("  84718735264  ", "84718735264")]
        [TestCase(" 12345678900", "12345678900")]
        [TestCase("20000001107", "20000001107")]
        public void StripMaskRemovesWhitespaceTest(string input, string expected)
        {
            Assert.That(CNH.StripMask(input), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // IsValid                                                              //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735264")]
        [TestCase("12345678900")]
        [TestCase("20000001107")]
        [TestCase("15705560294")]
        public void IsValidReturnsTrueForValidInputTest(string value)
        {
            Assert.That(CNH.IsValid(value), Is.True);
        }

        [Test]
        [TestCase("84718735254")]  // wrong first check digit (should be 6)
        [TestCase("84718735261")]  // wrong second check digit (should be 4)
        [TestCase("12345678901")]  // wrong second check digit (should be 0)
        [TestCase("20000001100")]  // wrong second check digit (should be 7)
        [TestCase("00000000000")]  // null sentinel — passes mod-11 but is never a real CNH
        [TestCase("1234567890")]   // too short
        [TestCase("123456789000")] // too long
        [TestCase("1234567890A")]  // non-numeric
        [TestCase("847187352641234567891")] // exceeds MaxInputLength (21 chars)
        [TestCase("")]
        public void IsValidReturnsFalseForInvalidInputTest(string value)
        {
            Assert.That(CNH.IsValid(value), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(CNH.IsValid(null), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullableCnhBehaviorTest()
        {
            CNH? cnh = "84718735264";

            Assert.That(cnh.HasValue, Is.True);
            Assert.That((string)cnh.Value, Is.EqualTo("84718735264"));

            cnh = null;

            Assert.That(cnh.HasValue, Is.False);
        }

        [Test]
        public void NullableCnhValueThrowsWhenNullTest()
        {
            CNH? cnh = null;

            Assert.Throws<InvalidOperationException>(() => { var _ = cnh.Value; });
        }

        // ------------------------------------------------------------------ //
        // Equality                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735264")]
        [TestCase("12345678900")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            CNH a = new(value);
            CNH b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentCnhTest()
        {
            CNH a = new("84718735264");
            CNH b = new("12345678900");

            Assert.That(a != b, Is.True);
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                                //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CNH(null!));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { CNH cnh = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("")]
        [TestCase("1234567890")]                       // 10 digits
        [TestCase("123456789000")]                     // 12 digits
        [TestCase("1234567890A")]                      // non-numeric
        [TestCase("847187352641234567891")]             // 21 chars — exceeds MaxInputLength
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CNH(value));
        }

        // ------------------------------------------------------------------ //
        // InvalidCastException                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("84718735254")] // first check digit should be 6, got 5
        [TestCase("84718735261")] // second check digit should be 4
        [TestCase("12345678901")] // second check digit should be 0
        [TestCase("20000001100")] // second check digit should be 7 (flag case)
        public void InvalidCheckDigitShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CNH(value));
        }

        // ------------------------------------------------------------------ //
        // Homogeneous sequences                                                //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("00000000000")] // passes mod-11 but is a known null sentinel in DETRAN systems
        [TestCase("11111111111")]
        [TestCase("22222222222")]
        [TestCase("33333333333")]
        [TestCase("44444444444")]
        [TestCase("55555555555")]
        [TestCase("66666666666")]
        [TestCase("77777777777")]
        [TestCase("88888888888")]
        [TestCase("99999999999")]
        public void HomogeneousSequenceShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new CNH(value));
        }

        // ------------------------------------------------------------------ //
        // JSON                                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        public void CnhConverterSerializesAsPlainStringTest()
        {
            CNH cnh = new("84718735264");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            var json = JsonSerializer.Serialize(cnh, options);
            var doc  = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("84718735264"));
        }

        [Test]
        public void CnhConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            var result = JsonSerializer.Deserialize<CNH>("\"84718735264\"", options);

            Assert.That(result, Is.EqualTo(new CNH("84718735264")));
        }

        [Test]
        public void CnhConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CNH>("null", options));
        }

        [Test]
        public void CnhConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CNH>("84718735264", options));
        }

        [Test]
        public void NullableCnhRoundTripTest()
        {
            CNH? original = new CNH("84718735264");
            var options   = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            var json   = JsonSerializer.Serialize(original, options);
            var result = JsonSerializer.Deserialize<CNH?>(json, options);

            Assert.That(result, Is.EqualTo(original));
        }

        [Test]
        public void NullableCnhNullJsonProducesNullTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnhConverter());

            var result = JsonSerializer.Deserialize<CNH?>("null", options);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CnhAutoConverterRoundTripTest()
        {
            var original = new { Cnh = new CNH("84718735264") };

            var json   = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json);

            Assert.That(result["Cnh"].GetValue<string>(), Is.EqualTo("84718735264"));
        }
    }
}
