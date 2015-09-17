// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser
{
    public class CsvParserOptions
    {
        public readonly char[] FieldsSeparator;
        
        public readonly bool SkipHeader;

        public readonly int DegreeOfParallelism;
        
        public CsvParserOptions(bool skipHeader, char[] fieldsSeparator)
            : this(skipHeader, fieldsSeparator, Environment.ProcessorCount)
        {
        }

        public CsvParserOptions(bool skipHeader, char[] fieldsSeparator, int degreeOfParallelism)
        {
            SkipHeader = skipHeader;
            FieldsSeparator = fieldsSeparator;
            DegreeOfParallelism = degreeOfParallelism;
        }

        public override string ToString()
        {
            return string.Format("CsvParserOptions (FieldsSeparator = ({0}), SkipHeader = {1})",
                string.Join(", ", FieldsSeparator), SkipHeader);
        }
    }
}
