using System;

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

        internal static string ExtractDigits(string value)
        {
            if (value.Length == 0) return value;
            Span<char> buf = stackalloc char[value.Length];
            int n = 0;
            foreach (char c in value)
                if (c >= '0' && c <= '9') buf[n++] = c;
            if (n == value.Length) return value;
            return new string(buf[..n]);
        }
    }
}
