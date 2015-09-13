// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser
{
    public class CsvReaderOptions
    {
        public readonly string[] NewLine;

        public CsvReaderOptions(string[] newLine)
        {
            NewLine = newLine;
        }

        public override string ToString()
        {
            return string.Format("CsvReaderOptions (NewLineSeparator = ({0})", string.Join(", ", NewLine));
        }
    }
}
