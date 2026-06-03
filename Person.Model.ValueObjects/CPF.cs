using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid CPF
    /// (<i>Cadastro de Pessoa Física</i> — Brazilian Social Security Number).
    /// Format: 9-digit root + 2 check digits (11 numeric digits total).
    /// Accepts input with or without the formatting mask (<c>123.456.789-09</c>).
    /// </summary>
    public readonly struct CPF : IEquatable<CPF>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 9;
        private const int CpfLength = CheckNumberLength + NumberLength;
        private const int Modulus = 11;

        private static readonly Regex FormatMask =
            new(@"^[0-9]{11}$", RegexOptions.Compiled);

        private readonly string _raw;

        /// <summary>The first 9 digits: taxpayer root.</summary>
        public string Number { get; }

        /// <summary>The 2 check digits.</summary>
        public string CheckNumber { get; }

        public static implicit operator string(CPF cpf) => cpf._raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{CPF}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator CPF(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CPF?.")
            : new(value);

        public static implicit operator CPF?(string value)
        {
            if (value == null) return null;
            return new CPF(value);
        }

        /// <summary>
        /// Constructs a CPF from a string with or without the formatting mask.
        /// Dots and hyphens are stripped automatically.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When the format is invalid (non-numeric or wrong length).</exception>
        /// <exception cref="InvalidCastException">When the check digits do not match.</exception>
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

        /// <summary>Returns the CPF formatted with mask: <c>XXX.XXX.XXX-XX</c>.</summary>
        public override string ToString() => _raw is null
            ? string.Empty
            : $"{_raw[..3]}.{_raw[3..6]}.{_raw[6..9]}-{_raw[9..]}";

        /// <summary>
        /// Validates a string as a CPF without throwing.
        /// Strips the mask automatically before validating.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>Removes mask characters (<c>.</c> and <c>-</c>) from the string.</summary>
        public static string StripMask(string value) =>
            value.Replace(".", "").Replace("-", "").Trim();

        public bool Equals(CPF other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is CPF other && Equals(other);
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
