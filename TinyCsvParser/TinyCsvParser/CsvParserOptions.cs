// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser
{
    public class CsvParserOptions
    {
        public readonly ITokenizer Tokenizer;
        
        public readonly bool SkipHeader;

        public readonly string CommentCharacter;

        public readonly int DegreeOfParallelism;

        public readonly bool KeepOrder;

        public CsvParserOptions(bool skipHeader, char fieldsSeparator)
            : this(skipHeader, new RFC4180Tokenizer(new Options('"', '\\', fieldsSeparator, false)))
        {
        }

        public CsvParserOptions(bool skipHeader, char fieldsSeparator, int degreeOfParallelism, bool keepOrder)
            : this(skipHeader, string.Empty, new RFC4180Tokenizer(new Options('"', '\\', fieldsSeparator, false)), degreeOfParallelism, keepOrder)
        {
        }

        public CsvParserOptions(bool skipHeader, ITokenizer tokenizer)
            : this(skipHeader, string.Empty, tokenizer)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer)
            : this(skipHeader, commentCharacter, tokenizer, Environment.ProcessorCount, true)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer, int degreeOfParallelism, bool keepOrder)
        {
            SkipHeader = skipHeader;
            CommentCharacter = commentCharacter;
            Tokenizer = tokenizer;
            DegreeOfParallelism = degreeOfParallelism;
            KeepOrder = keepOrder;
        }

        public override string ToString()
        {
            return $"CsvParserOptions (Tokenizer = {Tokenizer}, SkipHeader = {SkipHeader}, DegreeOfParallelism = {DegreeOfParallelism}, KeepOrder = {KeepOrder}, CommentCharacter = {CommentCharacter})";
        }
    }
}
