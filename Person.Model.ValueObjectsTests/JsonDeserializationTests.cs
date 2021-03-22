using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjectsTests
{
    public class DummyObject
    {
        public string Name { get; set; }
        [JsonConverter(typeof(MobileConverter))]
        public Mobile Mobile { get; set; }
        [JsonConverter(typeof(LandLineConverter))]
        public LandLine? LandLine { get; set; }
    }

    [TestFixture]
    public class JsonDeserializationTests
    {
        public string SerializeWith<T>(dynamic dummy)
            where T : JsonConverter, new()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new T());
            return JsonSerializer.Serialize(dummy, options);
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
    }    
}
