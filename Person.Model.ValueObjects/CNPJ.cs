using System;
using System.Linq;
using System.Text.RegularExpressions;

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
    /// <b>v2 breaking changes:</b>
    /// <list type="bullet">
    ///   <item><description><c>ToString()</c> uses alphanumeric mask for all formats (<c>AB.123.456/0001-00</c>).</description></item>
    ///   <item><description>Lowercase letters throw <c>ArgumentOutOfRangeException</c> instead of being silently uppercased.</description></item>
    /// </list>
    /// </para>
    /// <see href="https://www.gov.br/receitafederal/pt-br/centrais-de-conteudo/publicacoes/perguntas-e-respostas/cnpj/cnpj-alfanumerico.pdf"/>
    /// </summary>
    public readonly struct CNPJ : IEquatable<CNPJ>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 12;
        private const int CnpjLength = CheckNumberLength + NumberLength;

        private static readonly Regex FormatMask =
            new(@"^[A-Z0-9]{12}\d{2}$", RegexOptions.Compiled);

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
        /// <exception cref="ArgumentOutOfRangeException">When the format is invalid.</exception>
        /// <exception cref="InvalidCastException">When the check digits do not match.</exception>
        public CNPJ(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value),
                    "Não é possível criar um CNPJ a partir de um valor nulo.");

            value = StripMask(value);

            if (!FormatMask.IsMatch(value))
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
        public override string ToString() =>
            $"{_raw[..2]}.{_raw[2..5]}.{_raw[5..8]}/{_raw[8..12]}-{_raw[12..]}";

        /// <summary>
        /// Validates a string as a CNPJ without throwing.
        /// Strips the mask automatically; rejects lowercase letters.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>
        /// Removes mask characters (<c>.</c>, <c>/</c>, <c>-</c>) from the string.
        /// Does not alter casing — lowercase letters remain and will be rejected
        /// during format validation.
        /// </summary>
        public static string StripMask(string value) =>
            value.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

        public bool Equals(CNPJ other) => _raw == other._raw;
        public override bool Equals(object obj) => obj is CNPJ other && Equals(other);
        public override int GetHashCode() => _raw?.GetHashCode() ?? 0;
        public static bool operator ==(CNPJ left, CNPJ right) => left.Equals(right);
        public static bool operator !=(CNPJ left, CNPJ right) => !left.Equals(right);

        private static class Internal
        {
            // ASCII - 48: '0'→0 .. '9'→9 | 'A'→17 .. 'Z'→42
            private static int CharValue(char c) => c - 48;

            public static bool IsValid(string cnpj)
            {
                if (cnpj.Distinct().Count() == 1)
                    return false;

                int[] root = new int[NumberLength];
                for (int i = 0; i < NumberLength; i++)
                    root[i] = CharValue(cnpj[i]);

                int digit1 = CheckDigit(root);
                int digit2 = CheckDigit([.. root, digit1]);

                return (cnpj[12] - '0') == digit1
                    && (cnpj[13] - '0') == digit2;
            }

            private static int CheckDigit(int[] values)
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
