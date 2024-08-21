using System.Text.RegularExpressions;

namespace Opera
{
    public sealed class OperaGxValidNamesChecker
    {
        // The regex matches the line (^...$) consisting of zero or more of the following characters:
        // any letter from any language (\p{L})
        // any number, decimal digit (\p{Nd})
        // any of the other symbols until the closed square bracket ("\-" is an escaped symbol)
        private string regexPattern = @"^[\p{L}\p{Nd}!? :.,()'&\-]*$";

        public bool IsValid(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName)) return false;

            return Regex.Match(gameName, regexPattern).Success;
        }
    }

}
