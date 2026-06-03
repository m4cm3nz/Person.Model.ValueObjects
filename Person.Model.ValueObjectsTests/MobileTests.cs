using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class MobileTests
    {
        // ------------------------------------------------------------------ //
        // Valid formats                                                        //
        // ------------------------------------------------------------------ //

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
        public void ValidFormatsAreAcceptedTest(string phone)
        {
            var mobile = new Mobile(phone);

            Assert.That(mobile.CountryCode, Is.EqualTo("55"));
            Assert.That(mobile.AreaCode, Is.EqualTo("51"));
            Assert.That(mobile.Number[0], Is.EqualTo('9'));
            Assert.That(mobile.Number[1..], Is.EqualTo("36351064"));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("51936351064", "+55 (51) 93635-1064")]
        public void ToStringFormatsCorrectlyTest(string raw, string expected)
        {
            var mobile = new Mobile(raw);

            Assert.That(mobile.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                               //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void NullOrEmptyShouldThrowArgumentNullTest(string phone)
        {
            Assert.Throws<ArgumentNullException>(() => new Mobile(phone));
        }

        // ------------------------------------------------------------------ //
        // InvalidOperationException (implicit null)                          //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { Mobile _ = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("51936351064")]
        public void NullableMobileBehaviorTest(string value)
        {
            Mobile? mobile = value;

            Assert.That(mobile.HasValue, Is.True);
            Assert.That(mobile.Value.CountryCode, Is.EqualTo("55"));
            Assert.That(mobile.Value.AreaCode, Is.EqualTo("51"));

            mobile = null;

            Assert.That(mobile.HasValue, Is.False);
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException — number without 9 prefix              //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("51836351064")]
        public void NumberWithoutNinePrefixShouldThrowArgumentOutOfRangeTest(string phone)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Mobile(phone));
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException — invalid area code                    //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("00936351064")]
        public void InvalidAreaCodeShouldThrowArgumentOutOfRangeTest(string phone)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Mobile(phone));
        }

        // ------------------------------------------------------------------ //
        // Equality                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        public void EqualityBetweenEquivalentFormatsTest()
        {
            Mobile a = new("51936351064");
            Mobile b = new("+55 (51) 93635-1064");

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentMobilesTest()
        {
            Mobile a = new("51936351064");
            Mobile b = new("51987654321");

            Assert.That(a != b, Is.True);
        }

        // ------------------------------------------------------------------ //
        // Default struct                                                      //
        // ------------------------------------------------------------------ //

        [Test]
        public void DefaultToStringReturnsEmptyTest()
        {
            Assert.That(default(Mobile).ToString(), Is.EqualTo(string.Empty));
        }

        // ------------------------------------------------------------------ //
        // DoS — max input length                                              //
        // ------------------------------------------------------------------ //

        [Test]
        public void InputExceedingMaxLengthIsRejectedTest()
        {
            var oversized = new string('5', 31);
            Assert.Throws<ArgumentOutOfRangeException>(() => new Mobile(oversized));
        }
    }
}
