using System;

namespace TinyCsvParser.Extensions
{
    internal static class SpanExtensions
    {
        public static bool IsWhiteSpace(this ReadOnlySpan<char> span)
        {
            foreach (var c in span)
            {
                if (!char.IsWhiteSpace(c)) return false;
            }
            return true;
        }
    }
}
