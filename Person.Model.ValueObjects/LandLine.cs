using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um número de telefone fixo 
    /// respeitando a codificação brasileira definida pela ANATEL, 
    /// nos formatos atendidos pela expressão regular:
    /// ^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$
    /// </summary>
    [Serializable]
    public partial struct LandLine : IPhoneNumber
    {
        private static readonly string DefaultCountryCode = "55";

        private static readonly string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$";

        private static readonly string Message =
            "O telefone informado é inválido ou está em um formato incorreto.";

        [GeneratedRegex(@"[0-9]+")]
        private static partial Regex OnlyNumbersRegex();

        public static string GetOnlyNumbersFrom(string value) =>
           string.Join(null, OnlyNumbersRegex().Matches(value));

        public string Raw { get; private set; }
        public string CountryCode { get; private set; }
        public string AreaCode { get; private set; }
        public string Number { get; private set; }

        public override readonly string ToString()
        {
            return $"+{CountryCode} ({AreaCode} {Number[..4]} {Number.Substring(4, 4)})";
        }

        public LandLine(string phoneNumber)
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

        public static implicit operator string(LandLine phoneNumber) => phoneNumber.Raw;
        public static implicit operator LandLine(string phoneNumber)
        {
            phoneNumber = phoneNumber ??
                throw new InvalidOperationException(
                    $"Para valores nulos utilize Nullable<{typeof(LandLine).Name}>");

            return new LandLine(phoneNumber);
        }

    }
}
