// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace TinyCsvParser.Models;

public readonly record struct CsvOptions(
    char Delimiter,
    char QuoteChar,
    char EscapeChar,
    Encoding? Encoding = null,
    bool SkipHeader = false
)
{
    public static CsvOptions Default => new(';', '"', '\\', System.Text.Encoding.UTF8, false);

    public static CsvOptions Rfc4180 => new(';', '"', '"', System.Text.Encoding.UTF8, false);
}


