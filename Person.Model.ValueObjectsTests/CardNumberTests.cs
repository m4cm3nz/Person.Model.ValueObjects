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
        // Length boundaries                                                   //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("4532261615478")]        // 13 digits — Luhn valid
        [TestCase("4532261615478000000")] // 19 digits — Luhn valid
        public void CardNumberAtValidLengthBoundariesIsAcceptedTest(string number)
        {
            Assert.DoesNotThrow(() => { CardNumber _ = number; });
        }

        [Test]
        [TestCase("123456789012")]         // 12 digits — too short
        [TestCase("12345678901234567890")] // 20 digits — too long
        public void CardNumberOutsideLengthBoundariesIsRejectedTest(string number)
        {
            Assert.Throws<ArgumentException>(() => { CardNumber _ = number; });
        }

        // ------------------------------------------------------------------ //
        // Non-numeric characters                                              //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("4929-6220-4125-4286")]
        [TestCase("4929 6220 4125 4286")]
        public void CardNumberWithNonNumericCharactersIsRejectedTest(string number)
        {
            Assert.Throws<ArgumentException>(() => { CardNumber _ = number; });
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
        // IsValid                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(CardNumber.IsValid(null), Is.False);
        }

        [Test]
        [TestCase("4929622041254286")]
        public void IsValidReturnsTrueForValidNumberTest(string number)
        {
            Assert.That(CardNumber.IsValid(number), Is.True);
        }

        [Test]
        [TestCase("49538528316877")]       // Luhn fail
        [TestCase("123")]                  // too short
        [TestCase("4929-6220-4125-4286")] // non-numeric
        public void IsValidReturnsFalseForInvalidInputTest(string number)
        {
            Assert.That(CardNumber.IsValid(number), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Equality                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        public void EqualityBetweenTwoInstancesTest()
        {
            CardNumber a = new("4929622041254286");
            CardNumber b = new("4929622041254286");

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentCardNumbersTest()
        {
            CardNumber a = new("4929622041254286");
            CardNumber b = new("5211801418318353");

            Assert.That(a != b, Is.True);
        }

        // ------------------------------------------------------------------ //
        // ToString                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("4929622041254286", "**** **** **** 4286")]
        public void ToStringFormatsCorrectlyTest(string number, string expected)
        {
            CardNumber cardNumber = number;

            Assert.That(cardNumber.ToString(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("4929622041254286", "4929 6220 4125 4286")]
        public void ToFormattedReturnsFullNumberTest(string number, string expected)
        {
            CardNumber cardNumber = number;

            Assert.That(cardNumber.ToFormatted(), Is.EqualTo(expected));
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
