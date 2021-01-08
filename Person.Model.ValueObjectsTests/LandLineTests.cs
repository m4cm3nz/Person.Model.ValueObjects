using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Refere.Insurance.Person.Model.Tests
{
    [TestFixture]
    public class LandLineTests
    {
        [Test]
        [TestCase("5126352520")]
        [TestCase("5136352520")]
        [TestCase("5146352520")]
        [TestCase("5156352520")]
        [TestCase("+555136352520")]
        [TestCase("+55513635-2520")]
        [TestCase("+55513635 2520")]
        [TestCase("+55(51)36352520")]
        [TestCase("+55(51)3635 2520")]
        [TestCase("+55(51)3635-2520")]
        [TestCase("+55 5136352520")]
        [TestCase("+55 513635-2520")]
        [TestCase("+55 513635 2520")]
        [TestCase("+55 (51)36352520")]
        [TestCase("+55 (51)3635 2520")]
        [TestCase("+55 (51)3635-2520")]
        [TestCase("+5551 36352520")]
        [TestCase("+5551 3635-2520")]
        [TestCase("+5551 3635 2520")]
        [TestCase("+55(51) 36352520")]
        [TestCase("+55(51) 3635 2520")]
        [TestCase("+55(51) 3635-2520")]
        [TestCase("51 36352520")]
        [TestCase("51 3635-2520")]
        [TestCase("51 3635 2520")]
        [TestCase("+55 51 36352520")]
        [TestCase("+55 51 3635-2520")]
        [TestCase("+55 51 3635 2520")]
        [TestCase("+55 (51) 36352520")]
        [TestCase("+55 (51) 3635 2520")]
        [TestCase("+55 (51) 3635-2520")]
        public void CreateLandLineTest(string phone)
        {
            var phoneNumber = new LandLine(phone);
            Assert.That(phoneNumber == phone);
            Assert.That(phoneNumber.CountryCode, Is.EqualTo("55"));
            Assert.That(phoneNumber.AreaCode, Is.EqualTo("51"));
            Assert.That("2,3,4,5".Contains(phoneNumber.Number.Substring(0, 1)));
            Assert.That(phoneNumber.Number.Substring(1, 7), Is.EqualTo("6352520"));
        }
        
        [Test]        
        [TestCase("+55 (99) 0999-9999")]
        [TestCase("+55 (99) 1999-9999")]
        [TestCase("+55 (99) 6999-9999")]
        [TestCase("+55 (99) 7999-9999")]
        [TestCase("+55 (99) 8999-9999")]
        [TestCase("+55 (99) 9999-9999")]        
        [TestCase("+55 (00) 2059-9999")]        
        [TestCase("+55 (90) 2950-9999")]        
        [TestCase("+55 (09) 2950-9999")]        
        public void ShoudNotBeAbleToCreateAnatelOutOfRangeLandLineTest(string phone)
        {
            //Range validator @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$"
            Assert.Throws<ArgumentOutOfRangeException>(() => new LandLine(phone));
        }

        [Test]
        [TestCase("5136352520")]
        public void ShouldBeAbleToDefineNullableLandLineTest(string value)
        {
            LandLine? phone = value;
            Assert.That(phone == value);
            Assert.That(phone.HasValue);

            phone = null;
            Assert.That(phone == null);
            Assert.That(phone.HasValue, Is.False);
        }

    }
}
