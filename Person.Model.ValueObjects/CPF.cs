using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    public readonly struct CPF : IEquatable<CPF>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 9;
        private const int CpfLength = CheckNumberLength + NumberLength;
        private const int Modulus = 11;

        private static readonly Regex FormatMask =
            new(@"^\d{11}$", RegexOptions.Compiled);

        private readonly string _raw;

        public string Number { get; }
        public string CheckNumber { get; }

        public static implicit operator string(CPF cpf) => cpf._raw;

        public static implicit operator CPF(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CPF?.")
            : new(value);

        public static implicit operator CPF?(string value)
        {
            if (value == null) return null;
            return new CPF(value);
        }

        public CPF(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um CPF a partir de um valor nulo.");

            value = StripMask(value);

            if (!FormatMask.IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Formato inválido. Esperado: string numérica de {CpfLength} dígitos.");

            if (!Internal.IsValid(value))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a um CPF válido.");

            Number = value[..NumberLength];
            CheckNumber = value[NumberLength..];
            _raw = value;
        }

        public override string ToString() =>
            $"{_raw[..3]}.{_raw[3..6]}.{_raw[6..9]}-{_raw[9..]}";

        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        public static string StripMask(string value) =>
            value.Replace(".", "").Replace("-", "").Trim();

        public bool Equals(CPF other) => _raw == other._raw;
        public override bool Equals(object obj) => obj is CPF other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CPF left, CPF right) => left.Equals(right);
        public static bool operator !=(CPF left, CPF right) => !left.Equals(right);

        private static class Internal
        {
            public static bool IsValid(string cpf)
            {
                if (cpf.Distinct().Count() == 1)
                    return false;

                int[] root = cpf[..NumberLength].Select(c => c - '0').ToArray();

                int digit1 = CheckDigit(root);
                int digit2 = CheckDigit([.. root, digit1]);

                return (cpf[9] - '0') == digit1
                    && (cpf[10] - '0') == digit2;
            }

            private static int CheckDigit(int[] values)
            {
                int weight = values.Length + 1;
                int sum = values.Sum(v => v * weight--);
                int remainder = sum % Modulus;
                return remainder < CheckNumberLength ? 0 : Modulus - remainder;
            }
        }
    }
}
