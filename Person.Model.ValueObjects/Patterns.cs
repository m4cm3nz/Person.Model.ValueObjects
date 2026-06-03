using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    internal static partial class Patterns
    {
        [GeneratedRegex(@"^[A-Z0-9]{12}[0-9]{2}$")]
        internal static partial Regex CnpjFormat();

        [GeneratedRegex(@"^[0-9]{11}$")]
        internal static partial Regex CpfFormat();

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
