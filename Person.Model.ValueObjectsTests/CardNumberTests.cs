using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Person.Model.ValueObjectsTests
{

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
    }
}
