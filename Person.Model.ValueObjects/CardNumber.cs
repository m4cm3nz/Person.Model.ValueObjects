using System;
using System.Text;

namespace Person.Model.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a valid payment card number.
    /// Validates the number via the Luhn algorithm (mod 10). Accepts cards with 13 to 19 digits.
    /// </summary>
    public readonly struct CardNumber : IEquatable<CardNumber>
    {
        private readonly string _number;

        /// <summary>
        /// Constructs a CardNumber from a digit string.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="number"/> is null.</exception>
        /// <exception cref="ArgumentException">When the number fails the Luhn check or has an invalid length.</exception>
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
        /// Returns the card number with all but the last 4 digits masked.
        /// Example: <c>**** **** **** 4286</c>.
        /// Use <see cref="ToFormatted"/> to obtain the full unmasked number.
        /// </summary>
        public override string ToString()
        {
            if (_number is null) return string.Empty;
            int maskUntil = _number.Length - 4;
            var sb = new StringBuilder(_number.Length + _number.Length / 4);
            for (int i = 0; i < _number.Length; i++)
            {
                if (i > 0 && i % 4 == 0) sb.Append(' ');
                sb.Append(i < maskUntil ? '*' : _number[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the full card number formatted in groups of 4 digits separated by spaces.
        /// Example: <c>4929 6220 4125 4286</c>.
        /// </summary>
        public string ToFormatted()
        {
            if (_number is null) return string.Empty;
            var sb = new StringBuilder(_number.Length + _number.Length / 4);
            for (int i = 0; i < _number.Length; i++)
            {
                if (i > 0 && i % 4 == 0) sb.Append(' ');
                sb.Append(_number[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Validates a string as a card number via the Luhn algorithm.
        /// Returns <see langword="false"/> for <see langword="null"/>, strings with length
        /// outside the 13–19 range, or strings containing non-numeric characters.
        /// </summary>
        public static bool IsValid(string cardNumber)
        {
            if (cardNumber is null) return false;
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
        public override bool Equals(object? obj) => obj is CardNumber other && Equals(other);
        public override int GetHashCode() => _number?.GetHashCode() ?? 0;
        public static bool operator ==(CardNumber left, CardNumber right) => left.Equals(right);
        public static bool operator !=(CardNumber left, CardNumber right) => !left.Equals(right);
    }
}
