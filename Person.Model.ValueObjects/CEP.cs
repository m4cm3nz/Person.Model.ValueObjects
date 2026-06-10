using System;
using System.Text.Json.Serialization;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a Brazilian postal code
    /// (CEP — Código de Endereçamento Postal).
    /// Format: 8 numeric digits, optionally formatted as <c>XXXXX-XXX</c>.
    /// </summary>
    [JsonConverter(typeof(CepConverter))]
    public readonly struct CEP : IEquatable<CEP>
    {
        private const int CepLength = 8;
        private const int MaxInputLength = 20;

        private readonly string _raw;

        /// <summary>
        /// Constructs a CEP from a string with or without the formatting mask.
        /// The hyphen is stripped automatically.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the format is invalid (non-numeric, wrong length) or when the input
        /// exceeds <c>MaxInputLength</c> (20) characters.
        /// </exception>
        public CEP(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um CEP a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo permitido é {MaxInputLength} caracteres.");

            value = StripMask(value);

            if (!Patterns.CepFormat().IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Formato inválido. Esperado: string numérica de {CepLength} dígitos.");

            _raw = value;
        }

        /// <summary>Returns the CEP formatted with mask: <c>XXXXX-XXX</c>.</summary>
        public override string ToString() => _raw is null
            ? string.Empty
            : $"{_raw[..5]}-{_raw[5..]}";

        public static implicit operator string(CEP cep) => cep._raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{CEP}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator CEP(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CEP?.")
            : new(value);

        public static implicit operator CEP?(string value)
        {
            if (value == null) return null;
            return new CEP(value);
        }

        /// <summary>
        /// Validates a string as a CEP without throwing.
        /// Strips the mask automatically before validating.
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> characters.
        /// </summary>
        public static bool IsValid(string? value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = StripMask(value);
            return Patterns.CepFormat().IsMatch(value);
        }

        /// <summary>
        /// Removes the mask character (<c>-</c>) from the string in a single pass
        /// using a <c>stackalloc</c> char filter.
        /// </summary>
        public static string StripMask(string value)
        {
            Span<char> buf = stackalloc char[value.Length];
            int n = 0;
            foreach (char c in value)
                if (c != '-')
                    buf[n++] = c;
            var trimmed = buf[..n].Trim();
            if (trimmed.Length == value.Length) return value;
            return new string(trimmed);
        }

        public bool Equals(CEP other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is CEP other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CEP left, CEP right) => left.Equals(right);
        public static bool operator !=(CEP left, CEP right) => !left.Equals(right);
    }
}
