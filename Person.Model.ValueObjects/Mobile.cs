using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um número de celular brasileiro válido como value object imutável.
    /// Aceita DDI +55 (opcional), DDD de dois dígitos e número no padrão ANATEL para
    /// celulares (primeiro dígito obrigatoriamente 9, total de 9 dígitos).
    /// <para>
    /// <see cref="Raw"/> sempre retorna a forma canônica <c>CountryCode + AreaCode + Number</c>,
    /// independentemente do formato de entrada.
    /// </para>
    /// </summary>
    public readonly struct Mobile : IPhoneNumber, IEquatable<Mobile>
    {
        private const string DefaultCountryCode = "55";
        private const string Pattern =
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9\d{4}[- ]?\d{4})$";
        private const string InvalidMessage =
            "O celular informado é inválido ou está em um formato incorreto.";

        private static readonly Regex OnlyNumbers =
            new(@"[0-9]+", RegexOptions.Compiled);

        private static string ExtractDigits(string value) =>
            string.Join(null, OnlyNumbers.Matches(value));

        /// <summary>Forma canônica: <c>CountryCode + AreaCode + Number</c> (somente dígitos).</summary>
        public string Raw { get; }

        /// <summary>DDI. Padrão <c>"55"</c> quando não informado na entrada.</summary>
        public string CountryCode { get; }

        /// <summary>DDD com dois dígitos.</summary>
        public string AreaCode { get; }

        /// <summary>Número local com nove dígitos (início obrigatório com 9).</summary>
        public string Number { get; }

        /// <summary>
        /// Constrói um Mobile a partir de uma string em qualquer formato aceito.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="phoneNumber"/> é nulo ou vazio.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Quando o formato não corresponde ao padrão ANATEL.</exception>
        public Mobile(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            var match = Regex.Match(phoneNumber, Pattern);

            if (!match.Success)
                throw new ArgumentOutOfRangeException(nameof(phoneNumber), InvalidMessage);

            var country = ExtractDigits(match.Groups[1].Value);

            CountryCode = string.IsNullOrEmpty(country) ? DefaultCountryCode : country;
            AreaCode = ExtractDigits(match.Groups[2].Value);
            Number = ExtractDigits(match.Groups[3].Value);
            Raw = CountryCode + AreaCode + Number;
        }

        /// <summary>Retorna o celular formatado: <c>+55 (51) 93635-1064</c>.</summary>
        public override string ToString() =>
            $"+{CountryCode} ({AreaCode}) {Number[..5]}-{Number[5..]}";

        public static implicit operator string(Mobile phone) => phone.Raw;

        /// <exception cref="InvalidOperationException">
        /// Lançada quando <see langword="null"/> é atribuído via conversão implícita.
        /// Use <see cref="Nullable{Mobile}"/> para representar ausência de valor.
        /// </exception>
        public static implicit operator Mobile(string phone)
        {
            _ = phone ?? throw new InvalidOperationException(
                $"Para valores nulos utilize Nullable<{typeof(Mobile).Name}>.");

            return new Mobile(phone);
        }

        public bool Equals(Mobile other) => Raw == other.Raw;
        public override bool Equals(object obj) => obj is Mobile other && Equals(other);
        public override int GetHashCode() => Raw?.GetHashCode() ?? 0;
        public static bool operator ==(Mobile left, Mobile right) => left.Equals(right);
        public static bool operator !=(Mobile left, Mobile right) => !left.Equals(right);
    }
}
