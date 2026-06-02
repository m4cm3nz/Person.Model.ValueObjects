using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um CPF válido como value object imutável.
    /// Formato: 9 dígitos de raiz + 2 dígitos verificadores (total 11 dígitos numéricos).
    /// Aceita entrada com ou sem máscara (<c>123.456.789-09</c>).
    /// </summary>
    public readonly struct CPF : IEquatable<CPF>
    {
        private const int CheckNumberLength = 2;
        private const int NumberLength = 9;
        private const int CpfLength = CheckNumberLength + NumberLength;
        private const int Modulus = 11;

        private static readonly Regex FormatMask =
            new(@"^\d{11}$", RegexOptions.Compiled);

        private readonly string _raw;

        /// <summary>Os 9 primeiros dígitos: raiz do contribuinte.</summary>
        public string Number { get; }

        /// <summary>Os 2 dígitos verificadores.</summary>
        public string CheckNumber { get; }

        public static implicit operator string(CPF cpf) => cpf._raw;

        /// <exception cref="InvalidOperationException">
        /// Lançada quando <see langword="null"/> é atribuído via conversão implícita.
        /// Use <see cref="Nullable{CPF}"/> para representar ausência de valor.
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
        /// Constrói um CPF a partir de uma string com ou sem máscara de formatação.
        /// Pontos e hífen são removidos automaticamente.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="value"/> é nulo.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Quando o formato é inválido (não numérico ou comprimento incorreto).</exception>
        /// <exception cref="InvalidCastException">Quando os dígitos verificadores não conferem.</exception>
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

        /// <summary>Retorna o CPF formatado com máscara: <c>XXX.XXX.XXX-XX</c>.</summary>
        public override string ToString() =>
            $"{_raw[..3]}.{_raw[3..6]}.{_raw[6..9]}-{_raw[9..]}";

        /// <summary>
        /// Valida uma string como CPF sem lançar exceção.
        /// Remove máscara automaticamente antes de validar.
        /// </summary>
        public static bool IsValid(string value)
        {
            if (value is null) return false;
            value = StripMask(value);
            return FormatMask.IsMatch(value) && Internal.IsValid(value);
        }

        /// <summary>Remove os caracteres de máscara (<c>.</c> e <c>-</c>) da string.</summary>
        public static string StripMask(string value) =>
            value.Replace(".", "").Replace("-", "").Trim();

        public bool Equals(CPF other) => _raw == other._raw;
        public override bool Equals(object obj) => obj is CPF other && Equals(other);
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
