using System;
using System.Linq;

namespace Person.Model.ValueObjects
{
    [Serializable]
    public struct CPF
    {
        private static readonly int CheckNumberLength = 2;
        private static readonly int NumberLength = 9;
        private static readonly int CPFLength = CheckNumberLength + NumberLength;

        private readonly string Raw => Number + CheckNumber;
        public string Number { get; set; }
        public string CheckNumber { get; set; }

        public static implicit operator string(CPF number) => number.Raw;

        public static implicit operator CPF(string number)
        {
            number = number ??
                throw new InvalidOperationException(
                    $"Para valores nulos utilize Nullable<{typeof(CPF).Name}>");

            return new CPF(number);
        }

        public CPF(string number)
        {
            GuardArgument(number);

            if (!Internal.IsValid(number))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a um CPF válido.");

            Number = Internal.GetNumberFrom(number);
            CheckNumber = Internal.GetCheckNumberFrom(number);
        }

        public override readonly string ToString()
        {
            return Convert.ToUInt64(Raw).ToString(@"000\.000\.000\-00");
        }

        private static void GuardArgument(string number)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number),
                    "Não é possível criar um CPF a partir de um valor nulo.");

            if (IsOutOfRange(number))
                throw new ArgumentOutOfRangeException(
                    nameof(number), $"Era esperado uma string numérica de {CPFLength} dígitos");
        }

        public static bool IsOutOfRange(string number) =>
            !(IsElevenLength(number) && IsNumeric(number));

        public static bool IsNumeric(string value) =>
            value.All(char.IsNumber);

        public static bool IsElevenLength(string value) =>
            value.Length == CPFLength;

        public static string GetCheckNumberFrom(string number)
        {
            GuardArgument(number);
            return Internal.GetCheckNumberFrom(number);
        }

        public static string GetNumberFrom(string number)
        {
            GuardArgument(number);
            return Internal.GetNumberFrom(number);
        }
        public static bool IsValid(string number)
        {
            return !IsOutOfRange(number) && Internal.IsValid(number);
        }

        private class Internal
        {
            public static string GetCheckNumberFrom(string number)
            {
                return number?.Substring(NumberLength, CheckNumberLength);
            }

            public static string GetNumberFrom(string number)
            {
                return number?[..NumberLength];
            }

            public static bool IsValid(string number)
            {
                var numberArray = GetNumberFrom(number)
                    .ToCharArray()
                    .Select(x => int.Parse(x.ToString()))
                    .ToArray();

                var firstDigit = GetCheckDigitFrom(numberArray);

                numberArray = [.. numberArray, firstDigit];

                var secondDigit = GetCheckDigitFrom(numberArray);

                return number == string.Join("", numberArray.Append(secondDigit));
            }

            private static int GetCheckDigitFrom(int[] numberArray)
            {
                var summation = 0;
                int maxLoops = numberArray.Length;
                int weight = maxLoops + 1;

                for (var index = 0; index < maxLoops; index++)
                    summation += (numberArray[index] * weight--);

                var remainder = (summation % CPFLength);

                return remainder < CheckNumberLength ?
                    default :
                    CPFLength - remainder;
            }
        }
    }
}
