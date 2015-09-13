// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser
{
    public class CsvParserOptions
    {
        public readonly char[] FieldsSeparator;
        
        public readonly bool SkipHeader;

        public CsvParserOptions(bool skipHeader, char[] fieldsSeparator)
        {
            SkipHeader = skipHeader;
            FieldsSeparator = fieldsSeparator;
        }

        public override string ToString()
        {
            return string.Format("CsvParserOptions (FieldsSeparator = ({0}), SkipHeader = {1})",
                string.Join(", ", FieldsSeparator), SkipHeader);
        }
    }
}
