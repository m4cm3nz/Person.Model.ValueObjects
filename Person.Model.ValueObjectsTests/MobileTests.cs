using NUnit.Framework;
using Person.Model.ValueObjects;

namespace Refere.Insurance.Person.Model.Tests
{
    [TestFixture]
    public class MobileTests
    {
        [Test]
        [TestCase("51936351064")]
        [TestCase("+5551936351064")]
        [TestCase("+555193635-1064")]
        [TestCase("+555193635 1064")]
        [TestCase("+55(51)936351064")]
        [TestCase("+55(51)93635 1064")]
        [TestCase("+55(51)93635-1064")]
        [TestCase("+55 51936351064")]
        [TestCase("+55 5193635-1064")]
        [TestCase("+55 5193635 1064")]
        [TestCase("+55 (51)936351064")]
        [TestCase("+55 (51)93635 1064")]
        [TestCase("+55 (51)93635-1064")]
        [TestCase("+5551 936351064")]
        [TestCase("+5551 93635-1064")]
        [TestCase("+5551 93635 1064")]
        [TestCase("+55(51) 936351064")]
        [TestCase("+55(51) 93635 1064")]
        [TestCase("+55(51) 93635-1064")]
        [TestCase("51 936351064")]
        [TestCase("51 93635-1064")]
        [TestCase("51 93635 1064")]
        [TestCase("+55 51 936351064")]
        [TestCase("+55 51 93635-1064")]
        [TestCase("+55 51 93635 1064")]
        [TestCase("(51) 936351064")]
        [TestCase("(51) 93635 1064")]
        [TestCase("(51) 93635-1064")]
        [TestCase("+55 (51) 936351064")]
        [TestCase("+55 (51) 93635 1064")]
        [TestCase("+55 (51) 93635-1064")]
        public void CreateMobileTest(string phone)
        {
            var phoneNumber = new Mobile(phone);
            Assert.That(phoneNumber == phone);
            Assert.That(phoneNumber.CountryCode, Is.EqualTo("55"));
            Assert.That(phoneNumber.AreaCode, Is.EqualTo("51"));
            Assert.That(phoneNumber.Number[..1], Is.EqualTo("9"));
            Assert.That(phoneNumber.Number.Substring(1, 8), Is.EqualTo("36351064"));
        }
    }
}
