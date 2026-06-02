using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid Brazilian mobile phone number.
    /// Accepts DDI +55 (optional), a two-digit area code (DDD), and a local number
    /// following the ANATEL standard for mobile phones (first digit must be 9, 9 digits total).
    /// <para>
    /// <see cref="Raw"/> always returns the canonical form <c>CountryCode + AreaCode + Number</c>,
    /// regardless of the input format.
    /// </para>
    /// </summary>
    public readonly struct Mobile : IPhoneNumber, IEquatable<Mobile>
    {
        private const string DefaultCountryCode = "55";
        private const string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9\d{4}[- ]?\d{4})$";
        private const string InvalidMessage =
            "O celular informado é inválido ou está em um formato incorreto.";

        private static readonly Regex OnlyNumbers =
            new(@"[0-9]+", RegexOptions.Compiled);

        private static string ExtractDigits(string value) =>
            string.Join(null, OnlyNumbers.Matches(value));

        /// <summary>Canonical form: <c>CountryCode + AreaCode + Number</c> (digits only).</summary>
        public string Raw { get; }

        /// <summary>Country calling code (DDI). Defaults to <c>"55"</c> when not supplied in the input.</summary>
        public string CountryCode { get; }

        /// <summary>Two-digit area code (DDD).</summary>
        public string AreaCode { get; }

        /// <summary>Nine-digit local number (must start with 9).</summary>
        public string Number { get; }

        /// <summary>
        /// Constructs a Mobile from a string in any accepted format.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="phoneNumber"/> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When the format does not match the ANATEL pattern.</exception>
        public Mobile(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            var match = Regex.Match(phoneNumber, Pattern);

            if (!match.Success)
                throw new ArgumentOutOfRangeException(nameof(phoneNumber), InvalidMessage);

            var country = ExtractDigits(match.Groups[1].Value);

            CountryCode = string.IsNullOrEmpty(country) ? DefaultCountryCode : country;
            AreaCode = ExtractDigits(match.Groups[2].Value);
            Number = ExtractDigits(match.Groups[3].Value);
            Raw = CountryCode + AreaCode + Number;
        }

        /// <summary>Returns the phone number formatted as: <c>+55 (51) 93635-1064</c>.</summary>
        public override string ToString() =>
            $"+{CountryCode} ({AreaCode}) {Number[..5]}-{Number[5..]}";

        public static implicit operator string(Mobile phone) => phone.Raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{Mobile}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator Mobile(string phone)
        {
            _ = phone ?? throw new InvalidOperationException(
                $"Para valores nulos utilize Nullable<{typeof(Mobile).Name}>.");

            return new Mobile(phone);
        }

        public bool Equals(Mobile other) => Raw == other.Raw;
        public override bool Equals(object obj) => obj is Mobile other && Equals(other);
        public override int GetHashCode() => Raw?.GetHashCode() ?? 0;
        public static bool operator ==(Mobile left, Mobile right) => left.Equals(right);
        public static bool operator !=(Mobile left, Mobile right) => !left.Equals(right);
    }
}
