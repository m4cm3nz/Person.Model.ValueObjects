using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    internal static partial class Patterns
    {
        [GeneratedRegex(@"^[A-Z0-9]{12}[0-9]{2}$")]
        internal static partial Regex CnpjFormat();

        [GeneratedRegex(@"^[0-9]{8}$")]
        internal static partial Regex CepFormat();

        [GeneratedRegex(@"^[0-9]{11}$")]
        internal static partial Regex CpfFormat();

        [GeneratedRegex(@"^[0-9]{11}$")]
        internal static partial Regex PisFormat();

        // Practical RFC 5321/5322 subset (after lowercasing): local@domain, domain requires dot.
        [GeneratedRegex(
            @"^[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?(\.[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)+$",
            RegexOptions.NonBacktracking)]
        internal static partial Regex EmailFormat();

        [GeneratedRegex(
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5][0-9]{3}[- ]?[0-9]{4})$",
            RegexOptions.NonBacktracking)]
        internal static partial Regex LandLinePattern();

        [GeneratedRegex(
            @"^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9[0-9]{4}[- ]?[0-9]{4})$",
            RegexOptions.NonBacktracking)]
        internal static partial Regex MobilePattern();
    }
}
