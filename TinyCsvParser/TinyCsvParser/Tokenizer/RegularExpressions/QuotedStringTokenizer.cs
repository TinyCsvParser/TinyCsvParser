// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public class QuotedStringTokenizer : RFC4180Tokenizer
    {
        public QuotedStringTokenizer(Options options)
            : base(options)
        {
        }

        public QuotedStringTokenizer(char columnDelimiter)
            : base(new Options('"', '\\', columnDelimiter))
        {
        }

        public override string ToString()
        {
            return string.Format("QuotedStringTokenizer({0})", base.ToString());
        }
    }
}