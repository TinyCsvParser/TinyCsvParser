// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using TinyCsvParser.Extensions;

namespace TinyCsvParser.Tokenizer
{
    public class StringSplitTokenizer : ITokenizer
    {
        public readonly char[] FieldsSeparator;
        public readonly bool TrimLine;

        public StringSplitTokenizer(char[] fieldsSeparator, bool trimLine)
        {
            FieldsSeparator = fieldsSeparator;
            TrimLine = trimLine;
        }

        public ReadOnlyMemory<char>[] Tokenize(ReadOnlySpan<char> input)
        {
            var output = new List<ReadOnlyMemory<char>>();
            var parts = TrimLine ? input.Trim().Split(FieldsSeparator) : input.Split(FieldsSeparator);

            foreach (var part in parts)
            {
                output.Add(part.ToArray().AsMemory());
            }
            return output.ToArray();
        }

        public override string ToString()
        {
            return string.Format("StringSplitTokenizer (FieldsSeparator = {0}, TrimLine = {1})", FieldsSeparator, TrimLine);
        }
    }
}