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

        public readonly bool KeepOrder;

        public CsvParserOptions(bool skipHeader, char[] fieldsSeparator)
            : this(skipHeader, fieldsSeparator, Environment.ProcessorCount, true)
        {
        }

        public CsvParserOptions(bool skipHeader, char[] fieldsSeparator, int degreeOfParallelism, bool keepOrder)
        {
            SkipHeader = skipHeader;
            FieldsSeparator = fieldsSeparator;
            DegreeOfParallelism = degreeOfParallelism;
            KeepOrder = keepOrder;
        }

        public override string ToString()
        {
            return string.Format("CsvParserOptions (FieldsSeparator = ({0}), SkipHeader = {1}, DegreeOfParallelism = {2}, KeepOrder = {3})",
                string.Join(", ", FieldsSeparator), SkipHeader, DegreeOfParallelism, KeepOrder);
        }
    }
}
