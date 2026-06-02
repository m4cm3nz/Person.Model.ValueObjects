using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Tests
{
    internal class DummyCard
    {
        public CardNumber? CardNumber { get; set; }
    }

    internal class DummyCardWithAttribute
    {
        [JsonConverter(typeof(CardNumberConverter))]
        public CardNumber? CardNumber { get; set; }
    }

    [TestFixture]
    internal class CardNumberTests
    {
        // ------------------------------------------------------------------ //
        // Valid numbers                                                        //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("visa", "4929622041254286")]
        [TestCase("master", "5211801418318353")]
        [TestCase("diners", "38538228319872")]
        public void CardNumberShouldAcceptValidNumbers(string _, string number)
        {
            CardNumber cardNumber = number;

            Assert.That((string)cardNumber, Is.EqualTo(number));
        }

        // ------------------------------------------------------------------ //
        // Luhn validation                                                      //
        // ------------------------------------------------------------------ //

        [Test]
        public void CardNumberShouldBeMod10LuhnAlgorithmValidTest()
        {
            var invalidCardNumber = "49538528316877";
            CardNumber cardNumber;

            Assert.Throws<ArgumentException>(() => cardNumber = invalidCardNumber);
        }

        // ------------------------------------------------------------------ //
        // ToString                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("4929622041254286", "4929 6220 4125 4286")]
        public void ToStringFormatsCorrectlyTest(string number, string expected)
        {
            CardNumber cardNumber = number;

            Assert.That(cardNumber.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                               //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => { CardNumber _ = new CardNumber(null); });
        }

        // ------------------------------------------------------------------ //
        // JSON serialization (converter options)                              //
        // ------------------------------------------------------------------ //

        [Test]
        public void ShouldBeAbleToDeserializeValidCardNumberWithConverterOptionsTest()
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

        // ------------------------------------------------------------------ //
        // JSON serialization (converter attribute)                            //
        // ------------------------------------------------------------------ //

        [Test]
        public void ShouldBeAbleToDeserializeValidCardNumberWithConverterAttributeTest()
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
