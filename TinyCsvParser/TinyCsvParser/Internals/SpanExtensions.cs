// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser;

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