using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    public interface IPhoneNumber
    {
        string Raw { get; }
        string CountryCode { get; }
        string AreaCode { get; }
        string Number { get; }
    }

    internal static class PhoneNumberHelper
    {
        internal const string DefaultCountryCode = "55";
        private static readonly Regex OnlyNumbers = new(@"[0-9]+", RegexOptions.Compiled);
        internal static string ExtractDigits(string value) =>
            string.Join(null, OnlyNumbers.Matches(value));
    }
}
