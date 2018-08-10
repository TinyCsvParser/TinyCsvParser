using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoreCsvParser.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, ReadOnlySpan<T> span)
        {
            for (int i = 0, len = span.Length; i < len; i++)
            {
                list.Add(span[i]);
            }
        }
    }
}
