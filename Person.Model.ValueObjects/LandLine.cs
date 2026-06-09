using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid Brazilian landline phone number.
    /// Accepts DDI +55 (optional), a two-digit area code (DDD), and a local number
    /// following the ANATEL standard for landlines (first digit between 2 and 5).
    /// <para>
    /// <see cref="Raw"/> always returns the canonical form <c>CountryCode + AreaCode + Number</c>,
    /// regardless of the input format.
    /// </para>
    /// </summary>
    [JsonConverter(typeof(LandLineConverter))]
    public readonly struct LandLine : IPhoneNumber, IEquatable<LandLine>
    {
        private const int MaxInputLength = 30;
        private const string InvalidMessage =
            "O telefone informado é inválido ou está em um formato incorreto.";

        /// <summary>Canonical form: <c>CountryCode + AreaCode + Number</c> (digits only).</summary>
        public string Raw { get; }

        /// <summary>Country calling code (DDI). Defaults to <c>"55"</c> when not supplied in the input.</summary>
        public string CountryCode { get; }

        /// <summary>Two-digit area code (DDD).</summary>
        public string AreaCode { get; }

        /// <summary>Eight-digit local number.</summary>
        public string Number { get; }

        /// <summary>
        /// Constructs a LandLine from a string in any accepted format.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="phoneNumber"/> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When the format does not match the ANATEL pattern.</exception>
        public LandLine(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            if (phoneNumber.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(phoneNumber), InvalidMessage);

            var match = Patterns.LandLinePattern().Match(phoneNumber);

            if (!match.Success)
                throw new ArgumentOutOfRangeException(nameof(phoneNumber), InvalidMessage);

            var country = PhoneNumberHelper.ExtractDigits(match.Groups[1].Value);

            CountryCode = string.IsNullOrEmpty(country) ? PhoneNumberHelper.DefaultCountryCode : country;
            AreaCode = PhoneNumberHelper.ExtractDigits(match.Groups[2].Value);
            Number = PhoneNumberHelper.ExtractDigits(match.Groups[3].Value);
            Raw = CountryCode + AreaCode + Number;
        }

        /// <summary>Returns the phone number formatted as: <c>+55 (51) 3635-2520</c>.</summary>
        public override string ToString() => Raw is null
            ? string.Empty
            : $"+{CountryCode} ({AreaCode}) {Number[..4]}-{Number[4..]}";

        public static implicit operator string(LandLine phone) => phone.Raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{LandLine}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator LandLine(string phone)
        {
            _ = phone ?? throw new InvalidOperationException(
                $"Para valores nulos utilize Nullable<{typeof(LandLine).Name}>.");

            return new LandLine(phone);
        }

        public bool Equals(LandLine other) => Raw == other.Raw;
        public override bool Equals(object? obj) => obj is LandLine other && Equals(other);
        public override int GetHashCode() => Raw?.GetHashCode() ?? 0;
        public static bool operator ==(LandLine left, LandLine right) => left.Equals(right);
        public static bool operator !=(LandLine left, LandLine right) => !left.Equals(right);
    }
}
