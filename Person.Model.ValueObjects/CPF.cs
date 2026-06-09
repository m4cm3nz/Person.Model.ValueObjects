using System;
using System.Text.Json.Serialization;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid CPF
    /// (<i>Cadastro de Pessoa Física</i> — Brazilian Social Security Number).
    /// Format: 9-digit root + 2 check digits (11 numeric digits total).
    /// Accepts input with or without the formatting mask (<c>123.456.789-09</c>).
    /// </summary>
    [JsonConverter(typeof(CpfConverter))]
    public readonly struct CPF : IEquatable<CPF>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 9;
        private const int CpfLength = CheckNumberLength + NumberLength;
        private const int Modulus = 11;
        private const int MaxInputLength = 20;

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
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the format is invalid (non-numeric or wrong length) or when the input
        /// exceeds <c>MaxInputLength</c> (20) characters.
        /// </exception>
        /// <exception cref="InvalidCastException">When the check digits do not match.</exception>
        public CPF(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um CPF a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo permitido é {MaxInputLength} caracteres.");

            value = StripMask(value);

            if (!Patterns.CpfFormat().IsMatch(value))
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
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> (20) characters.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = StripMask(value);
            return Patterns.CpfFormat().IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>
        /// Removes mask characters (<c>.</c> and <c>-</c>) from the string in a single pass
        /// using a <c>stackalloc</c> char filter.
        /// </summary>
        public static string StripMask(string value)
        {
            Span<char> buf = stackalloc char[value.Length];
            int n = 0;
            foreach (char c in value)
                if (c != '.' && c != '-')
                    buf[n++] = c;
            var trimmed = buf[..n].Trim();
            if (trimmed.Length == value.Length) return value;
            return new string(trimmed);
        }

        public bool Equals(CPF other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is CPF other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CPF left, CPF right) => left.Equals(right);
        public static bool operator !=(CPF left, CPF right) => !left.Equals(right);

        private static class Internal
        {
            private static bool AllSame(string s)
            {
                char first = s[0];
                for (int i = 1; i < s.Length; i++)
                    if (s[i] != first) return false;
                return true;
            }

            public static bool IsValid(string cpf)
            {
                if (AllSame(cpf)) return false;

                Span<int> values = stackalloc int[NumberLength + 1];
                for (int i = 0; i < NumberLength; i++)
                    values[i] = cpf[i] - '0';

                int digit1 = CheckDigit(values[..NumberLength]);
                values[NumberLength] = digit1;
                int digit2 = CheckDigit(values);

                return (cpf[9] - '0') == digit1
                    && (cpf[10] - '0') == digit2;
            }

            private static int CheckDigit(ReadOnlySpan<int> values)
            {
                int weight = values.Length + 1;
                int sum = 0;
                for (int i = 0; i < values.Length; i++)
                    sum += values[i] * weight--;
                int remainder = sum % Modulus;
                return remainder < CheckNumberLength ? 0 : Modulus - remainder;
            }
        }
    }
}
