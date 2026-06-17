using System;
using System.Text.Json.Serialization;
using Person.Model.ValueObjects.Json;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing an email address.
    /// Validated against a practical subset of RFC 5321/5322:
    /// local part up to 64 characters, total up to 254 characters, standard domain format.
    /// Normalized to lowercase on construction. Leading and trailing whitespace is stripped.
    /// </summary>
    [JsonConverter(typeof(EmailConverter))]
    public readonly struct Email : IEquatable<Email>
    {
        private const int MaxLength = 254;       // RFC 5321
        private const int MaxLocalLength = 64;   // RFC 5321
        private const int MaxInputLength = 320;  // pre-trim DoS guard

        private readonly string _raw;            // normalized: trimmed, lowercased

        /// <summary>
        /// Constructs an Email from a string.
        /// Leading/trailing whitespace is stripped and the address is normalized to lowercase.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the address exceeds <c>MaxInputLength</c> before trimming, when the normalized
        /// address exceeds 254 characters (RFC 5321), when the local part exceeds 64 characters,
        /// or when the format is invalid (structure, consecutive dots, leading/trailing dots).
        /// </exception>
        public Email(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um Email a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo de entrada é {MaxInputLength} caracteres.");

            value = value.Trim().ToLowerInvariant();

            if (value.Length == 0 || value.Length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"O endereço de e-mail deve ter entre 1 e {MaxLength} caracteres.");

            if (!Patterns.EmailFormat().IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Formato de e-mail inválido.");

            var atIdx = value.IndexOf('@');

            if (atIdx > MaxLocalLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"A parte local (antes do @) deve ter no máximo {MaxLocalLength} caracteres.");

            var local = value[..atIdx];

            if (local[0] == '.' || local[^1] == '.' || local.Contains(".."))
                throw new ArgumentOutOfRangeException(nameof(value),
                    "A parte local não pode começar ou terminar com ponto, nem conter pontos consecutivos.");

            _raw = value;
        }

        /// <summary>The part of the address before the <c>@</c> sign.</summary>
        public string Local => _raw is null ? string.Empty : _raw[.._raw.IndexOf('@')];

        /// <summary>The part of the address after the <c>@</c> sign.</summary>
        public string Domain => _raw is null ? string.Empty : _raw[(_raw.IndexOf('@') + 1)..];

        /// <summary>Returns the email address in normalized form (lowercase).</summary>
        public override string ToString() => _raw ?? string.Empty;

        public static implicit operator string(Email email) => email._raw ?? string.Empty;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{Email}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator Email(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize Email?.")
            : new(value);

        public static implicit operator Email?(string value)
        {
            if (value == null) return null;
            return new Email(value);
        }

        /// <summary>
        /// Validates a string as an email address without throwing.
        /// Applies the same normalization (trim + lowercase) before validating.
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> characters.
        /// </summary>
        public static bool IsValid(string? value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = value.Trim().ToLowerInvariant();
            if (value.Length == 0 || value.Length > MaxLength) return false;
            if (!Patterns.EmailFormat().IsMatch(value)) return false;
            var atIdx = value.IndexOf('@');
            if (atIdx > MaxLocalLength) return false;
            var local = value[..atIdx];
            return local[0] != '.' && local[^1] != '.' && !local.Contains("..");
        }

        public bool Equals(Email other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is Email other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(Email left, Email right) => left.Equals(right);
        public static bool operator !=(Email left, Email right) => !left.Equals(right);
    }
}
