using System;
using System.Collections.Generic;

namespace TinyCsvParser.Extensions
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
