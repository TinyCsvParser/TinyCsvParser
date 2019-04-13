// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser
{
    public class CsvParserOptions
    {
        public readonly ITokenizer Tokenizer;
        
        public readonly bool SkipHeader;

        public readonly string CommentCharacter;

        public CsvParserOptions(bool skipHeader, char fieldsSeparator)
            : this(skipHeader, new QuotedStringTokenizer(fieldsSeparator))
        {
        }

        public CsvParserOptions(bool skipHeader, ITokenizer tokenizer)
            : this(skipHeader, string.Empty, tokenizer)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer)
        {
            SkipHeader = skipHeader;
            CommentCharacter = commentCharacter;
            Tokenizer = tokenizer;
        }


        public override string ToString()
        {
            return $"CsvParserOptions (Tokenizer = {Tokenizer}, SkipHeader = {SkipHeader}, CommentCharacter = {CommentCharacter})";
        }
    }
}
