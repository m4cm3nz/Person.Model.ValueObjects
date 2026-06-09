using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Tests
{
    internal class DummyObject
    {
        public string Name { get; set; }
        [JsonConverter(typeof(MobileConverter))]
        public Mobile Mobile { get; set; }
        [JsonConverter(typeof(LandLineConverter))]
        public LandLine? LandLine { get; set; }
    }

    internal class DummySubscriber
    {
        public string Name { get; set; }
        [JsonConverter(typeof(CpfConverter))]
        public CPF Cpf { get; set; }
        [JsonConverter(typeof(CnpjConverter))]
        public CNPJ Cnpj { get; set; }
    }

    // No [JsonConverter] attributes — relies entirely on the struct-level [JsonConverter] attribute.
    internal class DummyWithAutoConverters
    {
        public string Name { get; set; }
        public CPF Cpf { get; set; }
        public CNPJ Cnpj { get; set; }
        public Mobile Mobile { get; set; }
        public LandLine? LandLine { get; set; }
        public CardNumber CardNumber { get; set; }
    }

    [TestFixture]
    internal class JsonDeserializationTests
    {
        // ------------------------------------------------------------------ //
        // Struct-level [JsonConverter] — no property annotation needed       //
        // ------------------------------------------------------------------ //

        [Test]
        public void AllValueObjectsRoundTripWithoutPropertyAnnotationsTest()
        {
            var original = new DummyWithAutoConverters
            {
                Name = "Rafael",
                Cpf = "52998224725",
                Cnpj = "11222333000181",
                Mobile = "51985680052",
                LandLine = "5136350102",
                CardNumber = new CardNumber("4111111111111111"),
            };

            var json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<DummyWithAutoConverters>(json);

            Assert.That(result.Cpf, Is.EqualTo(original.Cpf));
            Assert.That(result.Cnpj, Is.EqualTo(original.Cnpj));
            Assert.That(result.Mobile, Is.EqualTo(original.Mobile));
            Assert.That(result.LandLine, Is.EqualTo(original.LandLine));
            Assert.That(result.CardNumber, Is.EqualTo(original.CardNumber));
        }

        [Test]
        public void ShouldBeAbleToDeserializeMobileAndLandlineUsingTextJson()
        {
            var dummyObject = new DummyObject
            {
                Name = "Rafael",
                Mobile = "51985680052",
                LandLine = "5136350102",
            };

            var stream = JsonSerializer.Serialize(dummyObject);

            var newDummy = JsonSerializer.Deserialize<DummyObject>(stream);

            Assert.That(newDummy.Mobile, Is.EqualTo(dummyObject.Mobile));
            Assert.That(newDummy.LandLine, Is.EqualTo(dummyObject.LandLine));
        }

        // ------------------------------------------------------------------ //
        // Payload format — converters must serialise as plain JSON string     //
        // ------------------------------------------------------------------ //

        [Test]
        public void MobileConverterSerializesAsPlainStringTest()
        {
            Mobile mobile = "51985680052";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MobileConverter());

            var json = JsonSerializer.Serialize(mobile, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo(mobile.Raw));
        }

        [Test]
        public void LandLineConverterSerializesAsPlainStringTest()
        {
            LandLine? landLine = "5136350102";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new LandLineConverter());

            var json = JsonSerializer.Serialize(landLine, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo(landLine.Value.Raw));
        }

        // ------------------------------------------------------------------ //
        // Null token handling                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        public void MobileConverterDeserializesFromPlainStringTest()
        {
            Mobile expected = "51985680052";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MobileConverter());

            var result = JsonSerializer.Deserialize<Mobile>($"\"{expected.Raw}\"", options);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void MobileConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MobileConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Mobile>("null", options));
        }

        [Test]
        public void CardNumberConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CardNumber>("4111111111111111", options));
        }

        [Test]
        public void LandLineConverterDeserializesFromPlainStringTest()
        {
            LandLine expected = "5136350102";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new LandLineConverter());

            var result = JsonSerializer.Deserialize<LandLine?>($"\"{expected.Raw}\"", options);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LandLineConverterSerializesNullAsJsonNullTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new LandLineConverter());

            var json = JsonSerializer.Serialize<LandLine?>(null, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Null));
        }

        [Test]
        public void CardNumberConverterSerializesAsPlainStringTest()
        {
            CardNumber cardNumber = new("4111111111111111");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            var json = JsonSerializer.Serialize(cardNumber, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("4111111111111111"));
        }

        [Test]
        public void CardNumberConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            var result = JsonSerializer.Deserialize<CardNumber>("\"4111111111111111\"", options);

            Assert.That(result, Is.EqualTo(new CardNumber("4111111111111111")));
        }

        [Test]
        public void CardNumberConverterRoundTripTest()
        {
            CardNumber cardNumber = new("4111111111111111");
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            var json = JsonSerializer.Serialize(cardNumber, options);
            var result = JsonSerializer.Deserialize<CardNumber>(json, options);

            Assert.That(result, Is.EqualTo(cardNumber));
        }

        [Test]
        public void CardNumberConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CardNumber>("null", options));
        }

        [Test]
        public void LandLineConverterReturnsNullOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new LandLineConverter());

            var result = JsonSerializer.Deserialize<LandLine?>("null", options);

            Assert.That(result, Is.Null);
        }

        // ------------------------------------------------------------------ //
        // CPF converter                                                       //
        // ------------------------------------------------------------------ //

        [Test]
        public void CpfConverterSerializesAsPlainStringTest()
        {
            CPF cpf = "52998224725";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CpfConverter());

            var json = JsonSerializer.Serialize(cpf, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("52998224725"));
        }

        [Test]
        public void CpfConverterRoundTripWithCaseInsensitiveOptionsTest()
        {
            var subscriber = new DummySubscriber
            {
                Name = "Rafael",
                Cpf = "52998224725",
                Cnpj = "11222333000181",
            };

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new CpfConverter());
            options.Converters.Add(new CnpjConverter());

            var json = JsonSerializer.Serialize(subscriber, options);
            var result = JsonSerializer.Deserialize<DummySubscriber>(json, options);

            Assert.That(result.Cpf, Is.EqualTo(subscriber.Cpf));
            Assert.That(result.Cnpj, Is.EqualTo(subscriber.Cnpj));
        }

        [Test]
        public void CpfConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CpfConverter());

            var result = JsonSerializer.Deserialize<CPF>("\"52998224725\"", options);

            Assert.That(result, Is.EqualTo(new CPF("52998224725")));
        }

        [Test]
        public void CpfConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CpfConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CPF>("null", options));
        }

        [Test]
        public void CpfConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CpfConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CPF>("52998224725", options));
        }

        [Test]
        public void CpfConverterThrowsOnPropertyLevelNullTest()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new CpfConverter());
            options.Converters.Add(new CnpjConverter());

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<DummySubscriber>("{\"Name\":\"x\",\"Cpf\":null,\"Cnpj\":\"11222333000181\"}", options));
        }

        // ------------------------------------------------------------------ //
        // CNPJ converter                                                      //
        // ------------------------------------------------------------------ //

        [Test]
        public void CnpjConverterSerializesAsPlainStringTest()
        {
            CNPJ cnpj = "11222333000181";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnpjConverter());

            var json = JsonSerializer.Serialize(cnpj, options);
            var doc = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("11222333000181"));
        }

        [Test]
        public void CnpjConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnpjConverter());

            var result = JsonSerializer.Deserialize<CNPJ>("\"11222333000181\"", options);

            Assert.That(result, Is.EqualTo(new CNPJ("11222333000181")));
        }

        [Test]
        public void CnpjConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnpjConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CNPJ>("null", options));
        }

        [Test]
        public void CnpjConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CnpjConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CNPJ>("11222333000181", options));
        }

        // ------------------------------------------------------------------ //
        // Nullable variants — T? must round-trip and accept null             //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullableCpfDeserializesNullTokenAsNullTest()
        {
            var result = JsonSerializer.Deserialize<CPF?>("null");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NullableCpfRoundTripTest()
        {
            CPF? value = new CPF("52998224725");
            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<CPF?>(json);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void NullableCnpjDeserializesNullTokenAsNullTest()
        {
            var result = JsonSerializer.Deserialize<CNPJ?>("null");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NullableCnpjRoundTripTest()
        {
            CNPJ? value = new CNPJ("11222333000181");
            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<CNPJ?>(json);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void NullableMobileDeserializesNullTokenAsNullTest()
        {
            var result = JsonSerializer.Deserialize<Mobile?>("null");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NullableMobileRoundTripTest()
        {
            Mobile? value = new Mobile("51985680052");
            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<Mobile?>(json);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void NullableCardNumberDeserializesNullTokenAsNullTest()
        {
            var result = JsonSerializer.Deserialize<CardNumber?>("null");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NullableCardNumberRoundTripTest()
        {
            CardNumber? value = new CardNumber("4111111111111111");
            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<CardNumber?>(json);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void NullableLandLineDeserializesNullTokenAsNullTest()
        {
            var result = JsonSerializer.Deserialize<LandLine?>("null");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void NullableLandLineRoundTripTest()
        {
            LandLine? value = new LandLine("5136350102");
            var json = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<LandLine?>(json);
            Assert.That(result, Is.EqualTo(value));
        }
    }
}
