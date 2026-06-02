using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    [Serializable]
    public struct LandLine : IPhoneNumber, IEquatable<LandLine>
    {
        private const string DefaultCountryCode = "55";
        private const string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$";
        private const string InvalidMessage =
            "O telefone informado é inválido ou está em um formato incorreto.";

        private static readonly Regex OnlyNumbers =
            new(@"[0-9]+", RegexOptions.Compiled);

        private static string ExtractDigits(string value) =>
            string.Join(null, OnlyNumbers.Matches(value));

        public string Raw { get; }
        public string CountryCode { get; }
        public string AreaCode { get; }
        public string Number { get; }

        public LandLine(string phoneNumber)
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
            Raw = ExtractDigits(phoneNumber);
        }

        public override readonly string ToString() =>
            $"+{CountryCode} ({AreaCode}) {Number[..4]}-{Number[4..]}";

        public static implicit operator string(LandLine phone) => phone.Raw;

        public static implicit operator LandLine(string phone)
        {
            _ = phone ?? throw new InvalidOperationException(
                $"Para valores nulos utilize Nullable<{typeof(LandLine).Name}>.");

            return new LandLine(phone);
        }

        public bool Equals(LandLine other) => Raw == other.Raw;
        public override bool Equals(object obj) => obj is LandLine other && Equals(other);
        public override readonly int GetHashCode() => Raw.GetHashCode();

        public static bool operator ==(LandLine left, LandLine right) => left.Equals(right);
        public static bool operator !=(LandLine left, LandLine right) => !left.Equals(right);
    }
}