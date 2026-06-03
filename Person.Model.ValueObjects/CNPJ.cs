using System;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid CNPJ
    /// (<i>Cadastro Nacional de Pessoa Jurídica</i> — Brazilian Employer Identification Number).
    /// Supports both the legacy numeric format and the new alphanumeric format
    /// introduced by IN RFB nº 2.229/2024, effective July 2026.
    /// <para>
    /// Format: <c>[A-Z0-9]{12}[0-9]{2}</c> — the last two characters (check digits)
    /// are always numeric. Lowercase letters are rejected; the caller is responsible
    /// for casing before constructing the value.
    /// </para>
    /// <para>
    /// <b>v10 breaking changes:</b>
    /// <list type="bullet">
    ///   <item><description><c>ToString()</c> uses alphanumeric mask for all formats (<c>AB.123.456/0001-00</c>).</description></item>
    ///   <item><description>Lowercase letters throw <c>ArgumentOutOfRangeException</c> instead of being silently uppercased.</description></item>
    ///   <item><description>Internal helper methods (<c>IsNumeric</c>, <c>IsFourteenLength</c>, <c>IsOutOfRange</c>, <c>GetNumberFrom</c>, <c>GetCheckNumberFrom</c>) removed from the public API.</description></item>
    ///   <item><description><c>[Serializable]</c> attribute removed.</description></item>
    /// </list>
    /// </para>
    /// <see href="https://www.gov.br/receitafederal/pt-br/centrais-de-conteudo/publicacoes/perguntas-e-respostas/cnpj/cnpj-alfanumerico.pdf"/>
    /// </summary>
    public readonly struct CNPJ : IEquatable<CNPJ>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 12;
        private const int CnpjLength = CheckNumberLength + NumberLength;
        private const int MaxInputLength = 25;

        private readonly string _raw;

        /// <summary>The first 12 characters: company root and establishment order.</summary>
        public string Number { get; }

        /// <summary>The 2 check digits.</summary>
        public string CheckNumber { get; }

        public static implicit operator string(CNPJ cnpj) => cnpj._raw;

        /// <exception cref="InvalidOperationException">
        /// Thrown when <see langword="null"/> is assigned via implicit conversion.
        /// Use <see cref="Nullable{CNPJ}"/> to represent the absence of a value.
        /// </exception>
        public static implicit operator CNPJ(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CNPJ?.")
            : new(value);

        public static implicit operator CNPJ?(string value)
        {
            if (value is null) return null;
            return new CNPJ(value);
        }

        /// <summary>
        /// Constructs a CNPJ from a string with or without formatting mask.
        /// Dots, slash and hyphen are stripped automatically; letters must be
        /// uppercase — lowercase results in <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When the format is invalid or when the input exceeds <c>MaxInputLength</c> (25) characters.
        /// </exception>
        /// <exception cref="InvalidCastException">When the check digits do not match.</exception>
        public CNPJ(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um CNPJ a partir de um valor nulo.");

            if (value.Length > MaxInputLength)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Comprimento máximo permitido é {MaxInputLength} caracteres.");

            value = StripMask(value);

            if (!Patterns.CnpjFormat().IsMatch(value))
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Formato inválido. Esperado: [A-Z0-9]{{12}}[0-9]{{2}} ({CnpjLength} caracteres, maiúsculas).");

            if (!Internal.IsValid(value))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a um CNPJ válido.");

            Number = value[..NumberLength];
            CheckNumber = value[NumberLength..];
            _raw = value;
        }

        /// <summary>
        /// Returns the CNPJ formatted with mask: <c>XX.XXX.XXX/XXXX-XX</c>.
        /// Works for both numeric and alphanumeric formats.
        /// </summary>
        public override string ToString() => _raw is null
            ? string.Empty
            : $"{_raw[..2]}.{_raw[2..5]}.{_raw[5..8]}/{_raw[8..12]}-{_raw[12..]}";

        /// <summary>
        /// Validates a string as a CNPJ without throwing.
        /// Strips the mask automatically; rejects lowercase letters.
        /// Returns <see langword="false"/> for strings exceeding <c>MaxInputLength</c> (25) characters.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            if (value.Length > MaxInputLength) return false;
            value = StripMask(value);
            return Patterns.CnpjFormat().IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>
        /// Removes mask characters (<c>.</c>, <c>/</c>, <c>-</c>) from the string in a single pass
        /// using a <c>stackalloc</c> char filter (no heap allocation when no mask characters
        /// are present and the result already matches the input).
        /// </summary>
        public static string StripMask(string value)
        {
            Span<char> buf = stackalloc char[value.Length];
            int n = 0;
            foreach (char c in value)
                if (c != '.' && c != '/' && c != '-')
                    buf[n++] = c;
            var trimmed = buf[..n].Trim();
            if (trimmed.Length == value.Length) return value;
            return new string(trimmed);
        }

        public bool Equals(CNPJ other) => _raw == other._raw;
        public override bool Equals(object? obj) => obj is CNPJ other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CNPJ left, CNPJ right) => left.Equals(right);
        public static bool operator !=(CNPJ left, CNPJ right) => !left.Equals(right);

        private static class Internal
        {
            private static bool AllSame(string s)
            {
                char first = s[0];
                for (int i = 1; i < s.Length; i++)
                    if (s[i] != first) return false;
                return true;
            }

            // ASCII-48: '0'→0 .. '9'→9 | 'A'→17 .. 'Z'→42
            private static int CharValue(char c) => c - 48;

            public static bool IsValid(string cnpj)
            {
                if (AllSame(cnpj)) return false;

                Span<int> values = stackalloc int[NumberLength + 1];
                for (int i = 0; i < NumberLength; i++)
                    values[i] = CharValue(cnpj[i]);

                int digit1 = CheckDigit(values[..NumberLength]);
                values[NumberLength] = digit1;
                int digit2 = CheckDigit(values);

                return (cnpj[12] - '0') == digit1
                    && (cnpj[13] - '0') == digit2;
            }

            private static int CheckDigit(ReadOnlySpan<int> values)
            {
                int sum = 0;
                int weight = values.Length - 7;
                for (int i = 0; i < values.Length; i++)
                {
                    sum += values[i] * weight--;
                    if (weight == 1) weight = 9;
                }
                int remainder = sum % 11;
                return remainder < 2 ? 0 : 11 - remainder;
            }
        }
    }
}
