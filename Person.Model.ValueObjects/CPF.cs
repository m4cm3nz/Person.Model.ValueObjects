// CPF.cs
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    [Serializable]
    public readonly struct CPF : IEquatable<CPF>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 9;
        private const int CpfLength = CheckNumberLength + NumberLength;
        private const int Modulus = 11;

        private static readonly Regex FormatMask =
            new Regex(@"^\d{11}$", RegexOptions.Compiled);

        public string Number { get; }
        public string CheckNumber { get; }

        private string Raw => Number + CheckNumber;

        public static implicit operator string(CPF cpf) => cpf.Raw;

        public static implicit operator CPF(string value) => value is null ?
            throw new InvalidOperationException():
            new (value);

        public static implicit operator CPF?(string value)
        {
            if(value == null ) return null;
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
        }

        public override string ToString() =>
            $"{Raw[..3]}.{Raw[3..6]}.{Raw[6..9]}-{Raw[9..]}";

        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        public static string StripMask(string value) =>
            value.Replace(".", "").Replace("-", "").Trim();

        public static bool IsNumeric(string value) => value.All(char.IsNumber);
        public static bool IsElevenLength(string value) => value.Length == CpfLength;
        public static bool IsOutOfRange(string value) => !IsElevenLength(value) || !IsNumeric(value);

        public static string GetNumberFrom(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            value = StripMask(value);
            if (IsOutOfRange(value)) throw new ArgumentOutOfRangeException(nameof(value));
            return value[..NumberLength];
        }

        public static string GetCheckNumberFrom(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            value = StripMask(value);
            if (IsOutOfRange(value)) throw new ArgumentOutOfRangeException(nameof(value));
            return value[NumberLength..];
        }

        public bool Equals(CPF other) => Raw == other.Raw;
        public override bool Equals(object obj) => obj is CPF other && Equals(other);
        public override int GetHashCode() => Raw.GetHashCode();

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