using NUnit.Framework;
using Person.Model.ValueObjects;
using System;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class LandLineTests
    {
        // ------------------------------------------------------------------ //
        // Valid formats                                                        //
        // ------------------------------------------------------------------ //

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
        [TestCase("(51) 36352520")]
        [TestCase("(51) 3635 2520")]
        [TestCase("(51) 3635-2520")]
        [TestCase("+55 51 36352520")]
        [TestCase("+55 51 3635-2520")]
        [TestCase("+55 51 3635 2520")]
        [TestCase("+55 (51) 36352520")]
        [TestCase("+55 (51) 3635 2520")]
        [TestCase("+55 (51) 3635-2520")]
        public void ValidFormatsAreAcceptedTest(string phone)
        {
            var landLine = new LandLine(phone);

            Assert.That(landLine.CountryCode, Is.EqualTo("55"));
            Assert.That(landLine.AreaCode, Is.EqualTo("51"));
            Assert.That(landLine.Number[0], Is.InRange('2', '5'));
            Assert.That(landLine.Number[1..], Is.EqualTo("6352520"));
        }

        // ------------------------------------------------------------------ //
        // Anatel out-of-range                                                 //
        // ------------------------------------------------------------------ //

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
        public void AnatelOutOfRangeNumberShouldThrowArgumentOutOfRangeTest(string phone)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LandLine(phone));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("5136352520", "+55 (51) 3635-2520")]
        public void ToStringFormatsCorrectlyTest(string raw, string expected)
        {
            var landLine = new LandLine(raw);

            Assert.That(landLine.ToString(), Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                               //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void NullOrEmptyShouldThrowArgumentNullTest(string phone)
        {
            Assert.Throws<ArgumentNullException>(() => new LandLine(phone));
        }

        // ------------------------------------------------------------------ //
        // InvalidOperationException (implicit null)                          //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { LandLine _ = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // Raw                                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("5136352520", "555136352520")]
        [TestCase("+55 (51) 3635-2520", "555136352520")]
        public void RawIsCanonicalFormTest(string input, string expectedRaw)
        {
            var landLine = new LandLine(input);

            Assert.That(landLine.Raw, Is.EqualTo(expectedRaw));
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                            //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("5136352520")]
        public void NullableLandLineBehaviorTest(string value)
        {
            LandLine? phone = value;

            Assert.That(phone.HasValue, Is.True);
            Assert.That(phone.Value.CountryCode, Is.EqualTo("55"));
            Assert.That(phone.Value.AreaCode, Is.EqualTo("51"));

            phone = null;

            Assert.That(phone.HasValue, Is.False);
        }
    }
}
