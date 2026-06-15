using System;
using System.Text.Json.Serialization;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a Brazilian driver's license number
    /// (CNH — Carteira Nacional de Habilitação).
    /// Format: 11 numeric digits with two weighted check digits per the SENATRAN algorithm
    /// (Resolução CONTRAN nº 541/2015). No standard display mask.
    /// </summary>
    [JsonConverter(typeof(CnhConverter))]
    public readonly struct CNH : IEquatable<CNH>
    {
        private const int CnhLength = 11;
        private const int MaxInputLength = 20;

        private readonly string _raw;

        /// <summary>
        /// Constructs a CNH from an 11-digit numeric string.
        /// Leading and trailing whitespace is stripped automatically.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the format is invalid (non-numeric or wrong length) or when the input
        /// exceeds <c>MaxInputLength</c> (20) characters.
        /// </exception>
        /// <exception cref="InvalidCastException">When the check digits do not match.</exception>
        public CNH(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar uma CNH a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo permitido é {MaxInputLength} caracteres.");

            value = StripMask(value);

            if (!Patterns.CnhFormat().IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Formato inválido. Esperado: string numérica de {CnhLength} dígitos.");

            if (!Internal.IsValid(value))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a uma CNH válida.");

            _raw = value;
        }

        /// <summary>Returns the CNH as a plain 11-digit string. CNH has no standard display mask.</summary>
        public override string ToString() => _raw ?? string.Empty;

        public static implicit operator string(CNH cnh) => cnh._raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{CNH}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator CNH(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CNH?.")
            : new(value);

        public static implicit operator CNH?(string value)
        {
            if (value == null) return null;
            return new CNH(value);
        }

        /// <summary>
        /// Validates a string as a CNH without throwing.
        /// Strips leading and trailing whitespace before validating.
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> characters.
        /// </summary>
        public static bool IsValid(string? value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = StripMask(value);
            return Patterns.CnhFormat().IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>
        /// Strips leading and trailing whitespace. CNH has no standard formatting mask.
        /// </summary>
        public static string StripMask(string value) => value.Trim();

        public bool Equals(CNH other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is CNH other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CNH left, CNH right) => left.Equals(right);
        public static bool operator !=(CNH left, CNH right) => !left.Equals(right);

        private static class Internal
        {
            // SENATRAN two-pass algorithm: both digit sets use the 9 base digits.
            private static ReadOnlySpan<byte> Weights1 => [9, 8, 7, 6, 5, 4, 3, 2, 1];
            private static ReadOnlySpan<byte> Weights2 => [1, 2, 3, 4, 5, 6, 7, 8, 9];

            private static bool AllSame(string s)
            {
                char first = s[0];
                for (int i = 1; i < s.Length; i++)
                    if (s[i] != first) return false;
                return true;
            }

            public static bool IsValid(string cnh)
            {
                if (AllSame(cnh)) return false;

                ReadOnlySpan<byte> w1 = Weights1;
                int sum1 = 0;
                for (int i = 0; i < w1.Length; i++)
                    sum1 += (cnh[i] - '0') * w1[i];

                int r1 = sum1 % 11;
                bool flag = r1 >= 10;
                int d10 = flag ? 0 : r1;

                if ((cnh[CnhLength - 2] - '0') != d10) return false;

                ReadOnlySpan<byte> w2 = Weights2;
                int sum2 = 0;
                for (int i = 0; i < w2.Length; i++)
                    sum2 += (cnh[i] - '0') * w2[i];

                int r2 = sum2 % 11;
                int d11Base = r2 >= 10 ? 0 : r2;
                // When the first digit overflowed (flag), subtract 1 from the second digit (wrapping 0 → 9).
                int d11 = flag ? (d11Base == 0 ? 9 : d11Base - 1) : d11Base;

                return (cnh[CnhLength - 1] - '0') == d11;
            }
        }
    }
}
