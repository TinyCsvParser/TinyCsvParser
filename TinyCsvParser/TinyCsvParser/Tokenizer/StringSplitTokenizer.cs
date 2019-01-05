// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.Tokenizer
{
    public class StringSplitTokenizer : ITokenizer
    {
        public readonly ReadOnlyMemory<char> FieldsSeparator;
        public readonly bool TrimLine;

        public StringSplitTokenizer(string fieldsSeparator, bool trimLine = false) : this(fieldsSeparator.AsMemory(), trimLine) { }

        public StringSplitTokenizer(char[] fieldsSeparator, bool trimLine = false) : this(fieldsSeparator.AsMemory(), trimLine) { }

        public StringSplitTokenizer(char fieldsSeparator, bool trimLine = false) : this(new[] { fieldsSeparator }.AsMemory(), trimLine) { }

        public StringSplitTokenizer(ReadOnlyMemory<char> fieldsSeparator, bool trimLine = false)
        {
            FieldsSeparator = fieldsSeparator;
            TrimLine = trimLine;
        }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            ReadOnlySpan<char> nextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining, out bool foundToken)
            {
                if (chars.IsEmpty)
                {
                    remaining = chars;
                    foundToken = false;
                    return chars;
                }

                int idx = chars.IndexOf(FieldsSeparator.Span, StringComparison.Ordinal);
                if (idx == -1)
                {
                    remaining = ReadOnlySpan<char>.Empty;
                    foundToken = !chars.IsEmpty;
                    return chars;
                }

                remaining = chars.Slice(idx + FieldsSeparator.Length);
                foundToken = true;
                return chars.Slice(0, idx);
            }

            return new TokenEnumerable(TrimLine ? input.Trim() : input, nextToken);
        }

        public override string ToString()
        {
            return string.Format("StringSplitTokenizer (FieldsSeparator = {0}, TrimLine = {1})", FieldsSeparator, TrimLine);
        }
    }
}