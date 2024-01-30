using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um número de telefone móvel 
    /// respeitando a codificação brasileira definida pela ANATEL, 
    /// nos formatos atendidos pela expressão regular:
    /// ^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9\d{4}[-| ]?\d{4})$
    /// </summary>
    [Serializable]
    public partial struct Mobile : IPhoneNumber
    {
        private static readonly string DefaultCountryCode = "55";

        private static readonly string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9\d{4}[-| ]?\d{4})$";

        private static readonly string Message =
            "O celular informado é inválido ou está em um formato incorreto.";

        [GeneratedRegex(@"[0-9]+")]
        private static partial Regex OnlyNumbers();

        public static string GetOnlyNumbersFrom(string value) =>
            string.Join(null, OnlyNumbers().Matches(value));

        public override readonly string ToString()
        {
            return $"+{CountryCode} ({AreaCode} {Number[..5]} {Number.Substring(5, 4)})";
        }

        public string Raw { get; private set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; private set; }
        public string Number { get; private set; }

        public Mobile(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            var match = Regex.Match(phoneNumber, Pattern);

            if (!match.Success)
                throw new ArgumentOutOfRangeException(nameof(phoneNumber), Message);

            CountryCode = GetOnlyNumbersFrom(match.Groups[1].Value);
            CountryCode = string.IsNullOrEmpty(CountryCode) ? DefaultCountryCode : CountryCode;
            AreaCode = GetOnlyNumbersFrom(match.Groups[2].Value);
            Number = GetOnlyNumbersFrom(match.Groups[3].Value);
            Raw = phoneNumber;
        }

        public static implicit operator string(Mobile phoneNumber) => phoneNumber.Raw;
        public static implicit operator Mobile(string phoneNumber)
        {
            phoneNumber = phoneNumber ??
                throw new InvalidOperationException(
                    $"Para valores nulos utilize Nullable<{typeof(Mobile).Name}>");

            return new Mobile(phoneNumber);
        }
    }
}
