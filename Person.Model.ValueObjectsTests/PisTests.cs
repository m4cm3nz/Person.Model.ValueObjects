using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class PisTests
    {
        // ------------------------------------------------------------------ //
        // Construction                                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("12345678919")]
        [TestCase("10000000008")]
        [TestCase("11122233390")]
        public void ValidPisConstructionTest(string raw)
        {
            var pis = new PIS(raw);

            Assert.That((string)pis, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("123.45678.91-9", "12345678919")]
        [TestCase("100.00000.00-8", "10000000008")]
        [TestCase("111.22233.39-0", "11122233390")]
        public void MaskedInputIsAcceptedTest(string masked, string expectedRaw)
        {
            var pis = new PIS(masked);

            Assert.That((string)pis, Is.EqualTo(expectedRaw));
        }

        // ------------------------------------------------------------------ //
        // Implicit conversion                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("12345678919")]
        [TestCase("10000000008")]
        public void ImplicitConversionFromStringTest(string value)
        {
            PIS pis = value;

            Assert.That((string)pis, Is.EqualTo(value));
        }

        [Test]
        [TestCase("12345678919")]
        [TestCase("11122233390")]
        public void ImplicitConversionToStringTest(string value)
        {
            PIS pis = new(value);
            string result = pis;

            Assert.That(result, Is.EqualTo(value));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("12345678919", "123.45678.91-9")]
        [TestCase("10000000008", "100.00000.00-8")]
        [TestCase("11122233390", "111.22233.39-0")]
        public void ToStringFormatsCorrectlyTest(string raw, string expected)
        {
            var pis = new PIS(raw);

            Assert.That(pis.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // StripMask / IsValid                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("123.45678.91-9", "12345678919")]
        [TestCase("100.00000.00-8", "10000000008")]
        [TestCase("111.22233.39-0", "11122233390")]
        public void StripMaskRemovesFormattingCharactersTest(string masked, string expected)
        {
            Assert.That(PIS.StripMask(masked), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("12345678919")]
        [TestCase("123.45678.91-9")]
        [TestCase("10000000008")]
        [TestCase("11122233390")]
        public void IsValidReturnsTrueForValidInputTest(string value)
        {
            Assert.That(PIS.IsValid(value), Is.True);
        }

        [Test]
        [TestCase("12345678910")]  // wrong check digit
        [TestCase("10000000001")]  // wrong check digit
        [TestCase("00000000000")]  // null sentinel — passes mod-11 but is never a real PIS
        [TestCase("1234567891")]   // too short
        [TestCase("123456789100")] // too long
        [TestCase("1234567891A")]  // non-numeric
        [TestCase("")]
        public void IsValidReturnsFalseForInvalidInputTest(string value)
        {
            Assert.That(PIS.IsValid(value), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(PIS.IsValid(null), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullablePisBehaviorTest()
        {
            PIS? pis = "12345678919";

            Assert.That(pis.HasValue, Is.True);
            Assert.That((string)pis.Value, Is.EqualTo("12345678919"));

            pis = null;

            Assert.That(pis.HasValue, Is.False);
        }

        [Test]
        public void NullablePisValueThrowsWhenNullTest()
        {
            PIS? pis = null;

            Assert.Throws<InvalidOperationException>(() => { var _ = pis.Value; });
        }

        // ------------------------------------------------------------------ //
        // Equality                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("12345678919")]
        [TestCase("10000000008")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            PIS a = new(value);
            PIS b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentPisTest()
        {
            PIS a = new("12345678919");
            PIS b = new("10000000008");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void MaskedAndUnmaskedInputsProduceEqualInstancesTest()
        {
            PIS fromRaw    = new("12345678919");
            PIS fromMasked = new("123.45678.91-9");

            Assert.That(fromRaw, Is.EqualTo(fromMasked));
            Assert.That(fromRaw.GetHashCode(), Is.EqualTo(fromMasked.GetHashCode()));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                                //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new PIS(null!));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { PIS pis = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("")]
        [TestCase("1234567891")]    // 10 digits
        [TestCase("123456789100")]  // 12 digits
        [TestCase("1234567891A")]   // non-numeric
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PIS(value));
        }

        // ------------------------------------------------------------------ //
        // InvalidCastException                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("12345678910")] // check digit should be 9
        [TestCase("10000000001")] // check digit should be 8
        [TestCase("11122233391")] // check digit should be 0
        public void InvalidCheckDigitShouldThrowInvalidCastTest(string value)
        {
            Assert.Throws<InvalidCastException>(() => new PIS(value));
        }

        // ------------------------------------------------------------------ //
        // Homogeneous sequences                                                //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("00000000000")] // passes mod-11 but is a known null sentinel (eSocial, RAIS, CAGED)
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
            Assert.Throws<InvalidCastException>(() => new PIS(value));
        }

        // ------------------------------------------------------------------ //
        // JSON                                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        public void PisConverterSerializesAsPlainStringTest()
        {
            PIS pis = new("12345678919");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            var json = JsonSerializer.Serialize(pis, options);
            var doc  = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("12345678919"));
        }

        [Test]
        public void PisConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            var result = JsonSerializer.Deserialize<PIS>("\"12345678919\"", options);

            Assert.That(result, Is.EqualTo(new PIS("12345678919")));
        }

        [Test]
        public void PisConverterDeserializesFromMaskedStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            var result = JsonSerializer.Deserialize<PIS>("\"123.45678.91-9\"", options);

            Assert.That(result, Is.EqualTo(new PIS("12345678919")));
        }

        [Test]
        public void PisConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PIS>("null", options));
        }

        [Test]
        public void PisConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PIS>("12345678919", options));
        }

        [Test]
        public void NullablePisRoundTripTest()
        {
            PIS? original = new PIS("12345678919");
            var options   = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            var json   = JsonSerializer.Serialize(original, options);
            var result = JsonSerializer.Deserialize<PIS?>(json, options);

            Assert.That(result, Is.EqualTo(original));
        }

        [Test]
        public void NullablePisNullJsonProducesNullTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new PisConverter());

            var result = JsonSerializer.Deserialize<PIS?>("null", options);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void PisAutoConverterRoundTripTest()
        {
            var original = new { Pis = new PIS("12345678919") };

            var json   = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json);

            Assert.That(result["Pis"].GetValue<string>(), Is.EqualTo("12345678919"));
        }
    }
}
