using System;
using System.Text.Json.Serialization;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a Brazilian worker identification number
    /// (PIS — Programa de Integração Social; also known as NIS or PASEP).
    /// Format: 11 numeric digits with a weighted check digit (mod 11, weights 3,2,9,8,7,6,5,4,3,2).
    /// Accepts input with or without the formatting mask (<c>XXX.XXXXX.XX-X</c>).
    /// </summary>
    [JsonConverter(typeof(PisConverter))]
    public readonly struct PIS : IEquatable<PIS>
    {
        private const int PisLength = 11;
        private const int MaxInputLength = 20;

        private readonly string _raw;

        /// <summary>
        /// Constructs a PIS from a string with or without the formatting mask.
        /// Dots and hyphens are stripped automatically.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the format is invalid (non-numeric or wrong length) or when the input
        /// exceeds <c>MaxInputLength</c> (20) characters.
        /// </exception>
        /// <exception cref="InvalidCastException">When the check digit does not match.</exception>
        public PIS(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um PIS a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo permitido é {MaxInputLength} caracteres.");

            value = StripMask(value);

            if (!Patterns.PisFormat().IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Formato inválido. Esperado: string numérica de {PisLength} dígitos.");

            if (!Internal.IsValid(value))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a um PIS válido.");

            _raw = value;
        }

        /// <summary>Returns the PIS formatted with mask: <c>XXX.XXXXX.XX-X</c>.</summary>
        public override string ToString() => _raw is null
            ? string.Empty
            : $"{_raw[..3]}.{_raw[3..8]}.{_raw[8..10]}-{_raw[10..]}";

        public static implicit operator string(PIS pis) => pis._raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{PIS}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator PIS(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize PIS?.")
            : new(value);

        public static implicit operator PIS?(string value)
        {
            if (value == null) return null;
            return new PIS(value);
        }

        /// <summary>
        /// Validates a string as a PIS without throwing.
        /// Strips the mask automatically before validating.
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> characters.
        /// </summary>
        public static bool IsValid(string? value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = StripMask(value);
            return Patterns.PisFormat().IsMatch(value) && Internal.IsValid(value);
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

        public bool Equals(PIS other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is PIS other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(PIS left, PIS right) => left.Equals(right);
        public static bool operator !=(PIS left, PIS right) => !left.Equals(right);

        private static class Internal
        {
            // Portaria MTE / DATAPREV: weights applied to the first 10 digits.
            private static ReadOnlySpan<byte> Weights => [3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            private static bool AllSame(string s)
            {
                char first = s[0];
                for (int i = 1; i < s.Length; i++)
                    if (s[i] != first) return false;
                return true;
            }

            public static bool IsValid(string pis)
            {
                if (AllSame(pis)) return false;

                ReadOnlySpan<byte> weights = Weights;
                int sum = 0;
                for (int i = 0; i < weights.Length; i++)
                    sum += (pis[i] - '0') * weights[i];

                int remainder = sum % 11;
                int expected = remainder < 2 ? 0 : 11 - remainder;

                return (pis[PisLength - 1] - '0') == expected;
            }
        }
    }
}
