using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
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

        public string Raw { get; }
        public string CountryCode { get; }
        public string AreaCode { get; }
        public string Number { get; }

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

        public override string ToString() =>
            $"+{CountryCode} ({AreaCode}) {Number[..5]}-{Number[5..]}";

        public static implicit operator string(Mobile phone) => phone.Raw;

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
