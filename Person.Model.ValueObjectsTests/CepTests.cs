using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class CepTests
    {
        // ------------------------------------------------------------------ //
        // Construction                                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("01310100")]
        [TestCase("20040020")]
        [TestCase("90010000")]
        public void ValidCepConstructionTest(string raw)
        {
            var cep = new CEP(raw);

            Assert.That((string)cep, Is.EqualTo(raw));
        }

        [Test]
        [TestCase("01310-100", "01310100")]
        [TestCase("20040-020", "20040020")]
        [TestCase("90010-000", "90010000")]
        public void MaskedInputIsAcceptedTest(string masked, string expectedRaw)
        {
            var cep = new CEP(masked);

            Assert.That((string)cep, Is.EqualTo(expectedRaw));
        }

        // ------------------------------------------------------------------ //
        // Implicit conversion                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("01310100")]
        [TestCase("90010000")]
        public void ImplicitConversionFromStringTest(string value)
        {
            CEP cep = value;

            Assert.That((string)cep, Is.EqualTo(value));
        }

        [Test]
        [TestCase("01310100")]
        [TestCase("90010000")]
        public void ImplicitConversionToStringTest(string value)
        {
            CEP cep = new(value);
            string result = cep;

            Assert.That(result, Is.EqualTo(value));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("01310100", "01310-100")]
        [TestCase("20040020", "20040-020")]
        [TestCase("90010000", "90010-000")]
        public void ToStringFormatsCorrectlyTest(string raw, string expected)
        {
            var cep = new CEP(raw);

            Assert.That(cep.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // StripMask / IsValid                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("01310-100", "01310100")]
        [TestCase("90010-000", "90010000")]
        public void StripMaskRemovesHyphenTest(string masked, string expected)
        {
            Assert.That(CEP.StripMask(masked), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("01310100")]
        [TestCase("01310-100")]
        [TestCase("90010000")]
        public void IsValidReturnsTrueForValidInputTest(string value)
        {
            Assert.That(CEP.IsValid(value), Is.True);
        }

        [Test]
        [TestCase("0131010")]
        [TestCase("013101000")]
        [TestCase("0131010A")]
        [TestCase("")]
        public void IsValidReturnsFalseForInvalidInputTest(string value)
        {
            Assert.That(CEP.IsValid(value), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(CEP.IsValid(null), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullableCepBehaviorTest()
        {
            CEP? cep = "01310100";

            Assert.That(cep.HasValue, Is.True);
            Assert.That((string)cep.Value, Is.EqualTo("01310100"));

            cep = null;

            Assert.That(cep.HasValue, Is.False);
        }

        [Test]
        public void NullableCepValueThrowsWhenNullTest()
        {
            CEP? cep = null;

            Assert.Throws<InvalidOperationException>(() => { var _ = cep.Value; });
        }

        // ------------------------------------------------------------------ //
        // Equality                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("01310100")]
        [TestCase("90010000")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            CEP a = new(value);
            CEP b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentCepsTest()
        {
            CEP a = new("01310100");
            CEP b = new("90010000");

            Assert.That(a != b, Is.True);
        }

        // ------------------------------------------------------------------ //
        // Exceptions                                                           //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new CEP(null!));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { CEP cep = (string)null; });
        }

        [Test]
        [TestCase("0131010")]
        [TestCase("013101000")]
        [TestCase("0131010A")]
        [TestCase("")]
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CEP(value));
        }

        // ------------------------------------------------------------------ //
        // JSON                                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        public void CepConverterSerializesAsPlainStringTest()
        {
            CEP cep = new("01310100");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            var json = JsonSerializer.Serialize(cep, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("01310100"));
        }

        [Test]
        public void CepConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            var result = JsonSerializer.Deserialize<CEP>("\"01310100\"", options);

            Assert.That(result, Is.EqualTo(new CEP("01310100")));
        }

        [Test]
        public void CepConverterDeserializesFromMaskedStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            var result = JsonSerializer.Deserialize<CEP>("\"01310-100\"", options);

            Assert.That(result, Is.EqualTo(new CEP("01310100")));
        }

        [Test]
        public void CepConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CEP>("null", options));
        }

        [Test]
        public void CepConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CEP>("1310100", options));
        }

        [Test]
        public void NullableCepRoundTripTest()
        {
            CEP? original = new CEP("01310100");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            var json = JsonSerializer.Serialize(original, options);
            var result = JsonSerializer.Deserialize<CEP?>(json, options);

            Assert.That(result, Is.EqualTo(original));
        }

        [Test]
        public void NullableCepNullJsonProducesNullTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CepConverter());

            var result = JsonSerializer.Deserialize<CEP?>("null", options);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CepAutoConverterRoundTripTest()
        {
            var original = new { Cep = new CEP("01310100") };

            var json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json);

            Assert.That(result["Cep"].GetValue<string>(), Is.EqualTo("01310100"));
        }
    }
}
