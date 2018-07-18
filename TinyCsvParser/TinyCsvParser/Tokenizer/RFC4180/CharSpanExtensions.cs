using System;
using System.Buffers;
using IToken = System.Buffers.IMemoryOwner<char>;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public static class CharSpanExtensions
    {
        /// <summary>
        /// Reads to the indicated character but does not include it in the results.
        /// Advances the input span to the indicated character but not past it.
        /// Optionally trims the return value.
        /// </summary>
        public static IToken ReadTo(this ReadOnlySpan<char> chars, char readTo, out ReadOnlySpan<char> remaining, bool trim = false)
        {
            var pool = SizedMemoryPool<char>.Instance;
            IToken token;
            var idx = chars.IndexOf(readTo);

            if (idx < 0)
            {
                remaining = ReadOnlySpan<char>.Empty;
                token = pool.Rent(chars.Length);
                chars.CopyTo(token.Memory.Span);
                return token;
            }

            remaining = chars.Slice(idx);
            var output = chars.Slice(0, idx);
            if (trim)
                output = output.Trim();

            token = pool.Rent(output.Length);
            output.CopyTo(token.Memory.Span);
            return token;
        }
    }
}
