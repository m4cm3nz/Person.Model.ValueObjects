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

    [TestFixture]
    internal class JsonDeserializationTests
    {
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
        public void MobileConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MobileConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Mobile>("null", options));
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
    }
}
