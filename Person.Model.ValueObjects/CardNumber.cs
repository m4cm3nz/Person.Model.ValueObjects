using System;
using System.Text;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Representa um número de cartão de pagamento válido como value object imutável.
    /// Valida o número via algoritmo de Luhn (mod 10). Aceita cartões de 13 a 19 dígitos.
    /// </summary>
    public readonly struct CardNumber : IEquatable<CardNumber>
    {
        private readonly string _number;

        /// <summary>Número do cartão sem formatação.</summary>
        public string Number => _number;

        /// <summary>
        /// Constrói um CardNumber a partir de uma string de dígitos.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="number"/> é nulo.</exception>
        /// <exception cref="ArgumentException">Quando o número falha na validação de Luhn ou tem comprimento inválido.</exception>
        public CardNumber(string number)
        {
            if (number is null)
                throw new ArgumentNullException(nameof(number));

            if (!IsValid(number))
                throw new ArgumentException("Número do cartão inválido.", nameof(number));

            _number = number;
        }

        public static implicit operator string(CardNumber cardNumber) => cardNumber._number;

        public static implicit operator CardNumber(string cardNumber) =>
            new(cardNumber);

        /// <summary>
        /// Retorna o número formatado em grupos de 4 dígitos separados por espaço.
        /// Exemplo: <c>4929 6220 4125 4286</c>.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder(_number.Length + _number.Length / 4);
            for (int i = 0; i < _number.Length; i++)
            {
                if (i > 0 && i % 4 == 0) sb.Append(' ');
                sb.Append(_number[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Valida uma string como número de cartão via algoritmo de Luhn.
        /// Retorna <see langword="false"/> para strings com comprimento fora do intervalo 13–19
        /// ou com caracteres não numéricos.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="cardNumber"/> é nulo.</exception>
        public static bool IsValid(string cardNumber)
        {
            if (cardNumber is null) throw new ArgumentNullException(nameof(cardNumber));
            if (cardNumber.Length < 13 || cardNumber.Length > 19) return false;

            int sum = 0;
            bool shouldDouble = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                char c = cardNumber[i];
                if (c < '0' || c > '9') return false;
                int digit = c - '0';
                if (shouldDouble && (digit *= 2) > 9) digit -= 9;
                sum += digit;
                shouldDouble = !shouldDouble;
            }

            return sum % 10 == 0;
        }

        public bool Equals(CardNumber other) => _number == other._number;
        public override bool Equals(object obj) => obj is CardNumber other && Equals(other);
        public override int GetHashCode() => _number?.GetHashCode() ?? 0;
        public static bool operator ==(CardNumber left, CardNumber right) => left.Equals(right);
        public static bool operator !=(CardNumber left, CardNumber right) => !left.Equals(right);
    }
}
