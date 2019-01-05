using System;

namespace CoreCsvParser.Tokenizer.RFC4180
{
    public static class CharSpanExtensions
    {
        /// <summary>
        /// Reads to the indicated character but does not include it in the results.
        /// Advances the input span to the indicated character but not past it.
        /// Optionally trims the return value.
        /// </summary>
        public static ReadOnlySpan<char> ReadTo(this ReadOnlySpan<char> chars, char readTo, out ReadOnlySpan<char> remaining, bool trim = false)
        {
            var idx = chars.IndexOf(readTo);

            if (idx < 0)
            {
                remaining = ReadOnlySpan<char>.Empty;
                return chars;
            }

            remaining = chars.Slice(idx);
            var output = chars.Slice(0, idx);
            if (trim)
                output = output.Trim();

            return output;
        }
    }
}
