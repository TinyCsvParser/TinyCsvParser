using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TinyCsvParser.Extensions
{
    public static class ListExtensions
    {
        public static void CopyTo<T>(this IList<T> list, Span<T> span)
        {
            for (int i = 0, len = list.Count; i < len; i++)
            {
                span[i] = list[i];
            }
        }

        public static void AddRange<T>(this IList<T> list, ReadOnlySpan<T> span)
        {
            for (int i = 0, len = span.Length; i < len; i++)
            {
                list.Add(span[i]);
            }
        }
    }

    public static class MatchExtensions
    {
        public static void Deconstruct(this Match match, out int index, out int length)
        {
            index = match.Index;
            length = match.Length;
        }
    }
}
