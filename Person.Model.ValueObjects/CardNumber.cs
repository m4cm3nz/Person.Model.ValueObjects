using System;

namespace Person.Model.ValueObjects
{
    public struct CardNumber
    {
        readonly string number;

        private CardNumber(string number)
        {
            if (!IsValid(number))
                throw new ArgumentException("Número do cartão inválido.");

            this.number = number;
        }

        public static implicit operator string(CardNumber goodThruDate) =>
           goodThruDate.number;

        public static implicit operator CardNumber(string cardNumber)
        {
            return new CardNumber(cardNumber);
        }

        public override string ToString()
        {
            return Convert.ToUInt64(number).ToString(@"0000 0000 0000 0000");
        }

        public static bool IsValid(string cardNumber)
        {
            var sum = 0;
            var shouldDouble = false;

            var cardNumberArray = cardNumber.ToCharArray();

            for (var i = cardNumberArray.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(cardNumberArray[i].ToString());

                if (shouldDouble && (digit *= 2) > 9) digit -= 9;

                sum += digit;
                shouldDouble = !shouldDouble;
            }

            return (sum % 10) == 0;
        }
    }
}
