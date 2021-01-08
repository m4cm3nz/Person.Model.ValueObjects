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
    public struct LandLine : PhoneNumber
    {
        private static readonly string DefaultCountryCode = "55";

        private static readonly string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$";

        private static readonly string Message =
            "O telefone informado é inválido ou está em um formato incorreto.";

        public static string GetOnlyNumbersFrom(string value) =>
           string.Join(null, Regex.Matches(value, @"[0-9]+"));

        public string Raw { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }

        public override string ToString()
        {
            return $"+{CountryCode} ({AreaCode} {Number.Substring(0, 4)} {Number.Substring(4, 4)})";
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
