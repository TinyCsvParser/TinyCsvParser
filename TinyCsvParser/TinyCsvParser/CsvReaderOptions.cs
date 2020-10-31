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
            return $"CsvReaderOptions (NewLineSeparator = ({string.Join(", ", NewLine)})";
        }
    }
}
