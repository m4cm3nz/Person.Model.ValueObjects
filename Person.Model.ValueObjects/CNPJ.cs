using System;
using System.Linq;

namespace Person.Model.ValueObjects
{        
    /// <summary>
    /// Employer Number 
    /// </summary>
    [Serializable]
    public struct CNPJ
    {
        private static readonly int CheckNumberLength = 2;
        private static readonly int NumberLength = 12;
        private static readonly int CnpjLength = CheckNumberLength + NumberLength;

        private string Raw => Number + CheckNumber;
        public string Number { get; private set; }
        public string CheckNumber { get; private set; }

        public static implicit operator string(CNPJ number) => number.Raw;

        public static implicit operator CNPJ(string number)
        {
            number = number ??
                throw new InvalidOperationException(
                    $"Para valores nulos utilize Nullable<{typeof(CNPJ).Name}>");

            return new CNPJ(number);
        }

        public CNPJ(string number)
        {
            GuardArgument(number);

            if (!Internal.IsValid(number))
                throw new InvalidCastException(
                    "A cadeia de caracteres informada não corresponde a um CNPJ válido.");

            Number = Internal.GetNumberFrom(number);
            CheckNumber = Internal.GetCheckNumberFrom(number);
        }

        public override string ToString()
        {
            return Convert.ToUInt64(Raw).ToString(@"00\.000\.000\/0000\-00");
        }

        private static void GuardArgument(string cnpj)
        {
            if (cnpj == null)
                throw new ArgumentNullException(nameof(cnpj),
                    "Não é possível criar um CNPJ a partir de um valor nulo.");

            if (IsOutOfRange(cnpj))
                throw new ArgumentOutOfRangeException(
                    $"Era esperado uma string numérica de {CnpjLength} dígitos", nameof(cnpj));
        }

        public static bool IsOutOfRange(string number) =>
            !(IsFourteenLength(number) && IsNumeric(number));

        public static bool IsNumeric(string value) =>
            value.All(char.IsNumber);

        public static bool IsFourteenLength(string number) =>
            number.Length == CnpjLength;

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
            return IsOutOfRange(number) ?
                false :
                Internal.IsValid(number);
        }

        private class Internal
        {
            public static string GetCheckNumberFrom(string number)
            {
                return number?.Substring(NumberLength, CheckNumberLength);
            }

            public static string GetNumberFrom(string number)
            {
                return number?.Substring(0, NumberLength);
            }

            public static bool IsValid(string number)
            {
                var numberArray = GetNumberFrom(number)
                    .ToCharArray()
                    .Select(x => int.Parse(x.ToString()))
                    .ToArray();

                var firstDigit = CheckDigit(numberArray);

                numberArray = numberArray
                    .Append(firstDigit)
                    .ToArray();

                var secondDigit = CheckDigit(numberArray);

                return number == string.Join("", numberArray.Append(secondDigit));
            }

            private static int CheckDigit(int[] numberArray)
            {
                var summation = 0;
                int maxLoops = numberArray.Length;
                int weight = maxLoops - 7;
                int module = 11;

                for (var index = 0; index < maxLoops; index++)
                {
                    summation += (numberArray[index] * weight--);
                    if (weight == 1) weight = 9;
                }

                var remainder = (summation % module);

                return remainder < CheckNumberLength ?
                    default :
                    module - remainder;
            }
        }
    }
}
