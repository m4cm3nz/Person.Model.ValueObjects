using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System;
using System.Text.Json;

namespace Person.Model.ValueObjects.Tests
{
    [TestFixture]
    internal class EmailTests
    {
        // ------------------------------------------------------------------ //
        // Construction                                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com")]
        [TestCase("user.name@example.com")]
        [TestCase("user+tag@gmail.com")]
        [TestCase("rafael@example.com.br")]
        [TestCase("u@example.co")]
        public void ValidEmailConstructionTest(string address)
        {
            var email = new Email(address);

            Assert.That((string)email, Is.EqualTo(address));
        }

        [Test]
        [TestCase("User@Example.COM",    "user@example.com")]
        [TestCase("RAFAEL@EXAMPLE.COM",  "rafael@example.com")]
        [TestCase("User.Name@Gmail.Com", "user.name@gmail.com")]
        public void ConstructionNormalizesToLowercaseTest(string input, string expected)
        {
            var email = new Email(input);

            Assert.That((string)email, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("  user@example.com  ", "user@example.com")]
        [TestCase(" rafael@example.com",  "rafael@example.com")]
        [TestCase("user@example.com ",    "user@example.com")]
        public void ConstructionStripsWhitespaceTest(string input, string expected)
        {
            var email = new Email(input);

            Assert.That((string)email, Is.EqualTo(expected));
        }

        // ------------------------------------------------------------------ //
        // Properties                                                           //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com",       "user",      "example.com")]
        [TestCase("user.name@example.com",  "user.name", "example.com")]
        [TestCase("user+tag@gmail.com",     "user+tag",  "gmail.com")]
        [TestCase("rafael@example.com.br",  "rafael",    "example.com.br")]
        public void LocalAndDomainPropertiesTest(string address, string expectedLocal, string expectedDomain)
        {
            var email = new Email(address);

            Assert.That(email.Local,  Is.EqualTo(expectedLocal));
            Assert.That(email.Domain, Is.EqualTo(expectedDomain));
        }

        [Test]
        public void DefaultEmailLocalAndDomainReturnEmptyTest()
        {
            Email email = default;

            Assert.That(email.Local,  Is.EqualTo(string.Empty));
            Assert.That(email.Domain, Is.EqualTo(string.Empty));
        }

        // ------------------------------------------------------------------ //
        // ToString                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com")]
        [TestCase("user.name@example.com")]
        public void ToStringReturnsNormalizedAddressTest(string address)
        {
            var email = new Email(address);

            Assert.That(email.ToString(), Is.EqualTo(address));
        }

        [Test]
        public void DefaultEmailToStringReturnsEmptyTest()
        {
            Email email = default;

            Assert.That(email.ToString(), Is.EqualTo(string.Empty));
        }

        // ------------------------------------------------------------------ //
        // Implicit conversion                                                  //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com")]
        [TestCase("user+tag@gmail.com")]
        public void ImplicitConversionFromStringTest(string value)
        {
            Email email = value;

            Assert.That((string)email, Is.EqualTo(value));
        }

        [Test]
        [TestCase("user@example.com")]
        [TestCase("rafael@example.com.br")]
        public void ImplicitConversionToStringTest(string value)
        {
            Email email = new(value);
            string result = email;

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void DefaultEmailImplicitConversionToStringReturnsEmptyTest()
        {
            Email email = default;
            string result = email;

            Assert.That(result, Is.EqualTo(string.Empty));
        }

        // ------------------------------------------------------------------ //
        // IsValid                                                              //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com")]
        [TestCase("user.name@example.com")]
        [TestCase("user+tag@gmail.com")]
        [TestCase("rafael@example.com.br")]
        [TestCase("User@Example.COM")]          // case-insensitive
        [TestCase("  user@example.com  ")]      // whitespace stripped
        [TestCase("u@example.co")]
        public void IsValidReturnsTrueForValidInputTest(string value)
        {
            Assert.That(Email.IsValid(value), Is.True);
        }

        [Test]
        [TestCase("")]                          // empty
        [TestCase("notanemail")]                // no @
        [TestCase("@example.com")]              // empty local
        [TestCase("user@")]                     // empty domain
        [TestCase("user@example")]              // no dot in domain
        [TestCase("user@@example.com")]         // multiple @
        [TestCase(".user@example.com")]         // local starts with dot
        [TestCase("user.@example.com")]         // local ends with dot
        [TestCase("user..name@example.com")]    // consecutive dots in local
        [TestCase("user@-example.com")]         // domain label starts with hyphen
        [TestCase("user@example-.com")]         // domain label ends with hyphen
        [TestCase("user@example..com")]         // consecutive dots in domain
        public void IsValidReturnsFalseForInvalidInputTest(string value)
        {
            Assert.That(Email.IsValid(value), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseForNullTest()
        {
            Assert.That(Email.IsValid(null), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseWhenExceedsMaxLengthTest()
        {
            // 260 chars — exceeds RFC 5321 limit of 254
            // 64 (local) + 1 (@) + 63.63.63.com (195) = 260
            var local  = new string('a', 64);
            var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 63) + ".com";
            var address = $"{local}@{domain}";
            Assert.That(address.Length, Is.GreaterThan(254));
            Assert.That(Email.IsValid(address), Is.False);
        }

        [Test]
        public void IsValidReturnsFalseWhenLocalExceedsMaxLengthTest()
        {
            // local part of 65 chars — exceeds RFC 5321 limit of 64
            var address = new string('a', 65) + "@example.com";
            Assert.That(Email.IsValid(address), Is.False);
        }

        // ------------------------------------------------------------------ //
        // Nullable                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullableEmailBehaviorTest()
        {
            Email? email = "user@example.com";

            Assert.That(email.HasValue, Is.True);
            Assert.That((string)email.Value, Is.EqualTo("user@example.com"));

            email = null;

            Assert.That(email.HasValue, Is.False);
        }

        [Test]
        public void NullableEmailValueThrowsWhenNullTest()
        {
            Email? email = null;

            Assert.Throws<InvalidOperationException>(() => { var _ = email.Value; });
        }

        // ------------------------------------------------------------------ //
        // Equality                                                             //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("user@example.com")]
        [TestCase("rafael@example.com.br")]
        public void EqualityBetweenTwoInstancesTest(string value)
        {
            Email a = new(value);
            Email b = new(value);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void InequalityBetweenDifferentEmailsTest()
        {
            Email a = new("user@example.com");
            Email b = new("other@example.com");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void CaseDifferenceProducesEqualInstancesTest()
        {
            Email fromLower = new("user@example.com");
            Email fromMixed = new("User@Example.COM");

            Assert.That(fromLower, Is.EqualTo(fromMixed));
            Assert.That(fromLower.GetHashCode(), Is.EqualTo(fromMixed.GetHashCode()));
        }

        // ------------------------------------------------------------------ //
        // ArgumentNullException                                                //
        // ------------------------------------------------------------------ //

        [Test]
        public void NullConstructorArgShouldThrowArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Email(null!));
        }

        [Test]
        public void NullImplicitAssignmentShouldThrowInvalidOperationTest()
        {
            Assert.Throws<InvalidOperationException>(() => { Email email = (string)null; });
        }

        // ------------------------------------------------------------------ //
        // ArgumentOutOfRangeException                                         //
        // ------------------------------------------------------------------ //

        [Test]
        [TestCase("")]
        [TestCase("notanemail")]
        [TestCase("@example.com")]
        [TestCase("user@")]
        [TestCase("user@example")]
        [TestCase("user@@example.com")]
        [TestCase(".user@example.com")]
        [TestCase("user.@example.com")]
        [TestCase("user..name@example.com")]
        public void InvalidFormatShouldThrowArgumentOutOfRangeTest(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Email(value));
        }

        [Test]
        public void LocalPartExceedingMaxLengthShouldThrowArgumentOutOfRangeTest()
        {
            var address = new string('a', 65) + "@example.com";
            Assert.Throws<ArgumentOutOfRangeException>(() => new Email(address));
        }

        [Test]
        public void TotalLengthExceedingMaxShouldThrowArgumentOutOfRangeTest()
        {
            var local  = new string('a', 64);
            var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 63) + ".com";
            var address = $"{local}@{domain}";
            Assert.That(address.Length, Is.GreaterThan(254));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Email(address));
        }

        // ------------------------------------------------------------------ //
        // JSON                                                                 //
        // ------------------------------------------------------------------ //

        [Test]
        public void EmailConverterSerializesAsPlainStringTest()
        {
            Email email  = new("user@example.com");
            var options  = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            var json = JsonSerializer.Serialize(email, options);
            var doc  = JsonDocument.Parse(json);

            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(doc.RootElement.GetString(), Is.EqualTo("user@example.com"));
        }

        [Test]
        public void EmailConverterDeserializesFromPlainStringTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            var result = JsonSerializer.Deserialize<Email>("\"user@example.com\"", options);

            Assert.That(result, Is.EqualTo(new Email("user@example.com")));
        }

        [Test]
        public void EmailConverterNormalizesOnDeserializeTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            var result = JsonSerializer.Deserialize<Email>("\"User@Example.COM\"", options);

            Assert.That(result, Is.EqualTo(new Email("user@example.com")));
        }

        [Test]
        public void EmailConverterThrowsOnNullTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Email>("null", options));
        }

        [Test]
        public void EmailConverterThrowsOnNonStringTokenTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Email>("42", options));
        }

        [Test]
        public void NullableEmailRoundTripTest()
        {
            Email? original = new Email("user@example.com");
            var options     = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            var json   = JsonSerializer.Serialize(original, options);
            var result = JsonSerializer.Deserialize<Email?>(json, options);

            Assert.That(result, Is.EqualTo(original));
        }

        [Test]
        public void NullableEmailNullJsonProducesNullTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EmailConverter());

            var result = JsonSerializer.Deserialize<Email?>("null", options);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void EmailAutoConverterRoundTripTest()
        {
            var original = new { Email = new Email("user@example.com") };

            var json   = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json);

            Assert.That(result["Email"].GetValue<string>(), Is.EqualTo("user@example.com"));
        }
    }
}
