using System.Collections.Generic;
using System.Linq;

namespace Engine.Shared
{
    internal static class ExtensionMethods
    {
        internal static bool IsEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        internal static bool HasText(this string text)
        {
            return !text.IsEmpty();
        }

        internal static bool ContainsInvalidCharacter(this string text)
        {
            return !text.All(x => char.IsLetterOrDigit(x) || x.Equals('_') || x.Equals(' '));
        }

        internal static bool HasAnInternalSpace(this string text)
        {
            return text.HasText() && text.Trim().Contains(" ");
        }

        internal static string ToStringWithLineFeeds(this IEnumerable<string> lines)
        {
            return string.Join("\r\n", lines);
        }

        internal static string Repeated(this string text, int times)
        {
            return string.Concat(Enumerable.Repeat(text, times));
        }
    }
}