using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um CNPJ válido como value object imutável.
    /// Suporta o formato numérico legado (14 dígitos) e o novo formato
    /// alfanumérico vigente a partir de julho/2026 (IN RFB nº 2.229/2024).
    /// <para>
    /// Formato: <c>[A-Z0-9]{12}[0-9]{2}</c> — os dois últimos caracteres
    /// (dígitos verificadores) são sempre numéricos. Letras minúsculas são
    /// rejeitadas; o caller é responsável pelo casing antes de construir o valor.
    /// </para>
    /// <para>
    /// <b>Breaking changes v2:</b>
    /// <list type="bullet">
    ///   <item><description><c>ToString()</c> retorna máscara alfanumérica (<c>AB.123.456/0001-00</c>) para todos os formatos.</description></item>
    ///   <item><description>Letras minúsculas lançam <c>ArgumentOutOfRangeException</c> em vez de serem normalizadas silenciosamente.</description></item>
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

        /// <summary>Os 12 primeiros caracteres: raiz + ordem do estabelecimento.</summary>
        public string Number { get; }

        /// <summary>Os 2 dígitos verificadores.</summary>
        public string CheckNumber { get; }

        public static implicit operator string(CNPJ cnpj) => cnpj._raw;

        public static implicit operator CNPJ(string value) => value is null
            ? throw new InvalidOperationException("Para valores nulos utilize CNPJ?.")
            : new(value);

        public static implicit operator CNPJ?(string value)
        {
            if (value is null) return null;
            return new CNPJ(value);
        }

        /// <summary>
        /// Constrói um CNPJ a partir de uma string com ou sem máscara de formatação.
        /// Pontos, barra e hífen são removidos automaticamente; letras devem estar
        /// em maiúsculas — minúsculas resultam em <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="value"/> é nulo.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Quando o formato é inválido.</exception>
        /// <exception cref="InvalidCastException">Quando os dígitos verificadores não conferem.</exception>
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
        /// Retorna o CNPJ formatado com máscara: <c>XX.XXX.XXX/XXXX-XX</c>.
        /// Funciona para formatos numérico e alfanumérico.
        /// </summary>
        public override string ToString() =>
            $"{_raw[..2]}.{_raw[2..5]}.{_raw[5..8]}/{_raw[8..12]}-{_raw[12..]}";

        /// <summary>
        /// Valida uma string como CNPJ sem lançar exceção.
        /// Remove máscara automaticamente; rejeita letras minúsculas.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>
        /// Remove os caracteres de máscara (<c>.</c>, <c>/</c>, <c>-</c>) da string.
        /// Não altera casing — letras minúsculas permanecem e serão rejeitadas
        /// na validação de formato.
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
