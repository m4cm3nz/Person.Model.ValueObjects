using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjectsTests
{
    public class DummyCard
    {
        public CardNumber? CardNumber { get; set; }
    }

    public class DummyCardWithAttribute
    {
        [JsonConverter(typeof(CardNumberConverter))]
        public CardNumber? CardNumber { get; set; }
    }

    [TestFixture]
    public class CardNumberTests
    {
        [TestCase("visa", "4929622041254286")]
        [TestCase("master", "5211801418318353")]
        [TestCase("diners", "38538228319872")]
        public void CardNumberShouldAcceptValidNumbers(string _, string number)
        {
            CardNumber cardNumber = number;
            Assert.That(cardNumber == number);
        }

        [Test]
        public void CardNumberShouldBeMod10LuhnAlgorithimValid()
        {
            var invalidCardNumber = "49538528316877";
            CardNumber cardNumber;
            Assert.Throws<ArgumentException>(() => cardNumber = invalidCardNumber);
        }

        [Test]
        public void ShouldBeAbleToDeserializeValidCardNumberWithConverterOptions()
        {
            var dummy = new DummyCard
            {
                CardNumber = "4929622041254286"
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new CardNumberConverter());

            var json = JsonSerializer.Serialize(dummy, options);

            var newDummy = JsonSerializer.Deserialize<DummyCard>(json, options);

            Assert.That(newDummy.CardNumber, Is.EqualTo(dummy.CardNumber));
        }

        [Test]
        public void ShouldBeAbleToDeserializeValidMonthOfYearWithConverterAttribute()
        {
            var dummy = new DummyCardWithAttribute
            {
                CardNumber = "4929622041254286"
            };

            var json = JsonSerializer.Serialize(dummy);

            var newDummy = JsonSerializer.Deserialize<DummyCardWithAttribute>(json);

            Assert.That(newDummy.CardNumber, Is.EqualTo(dummy.CardNumber));
        }
    }
}
