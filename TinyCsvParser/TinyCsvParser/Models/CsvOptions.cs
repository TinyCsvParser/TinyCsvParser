// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace TinyCsvParser.Models;

public readonly record struct CsvOptions(
    char Delimiter,
    char QuoteChar,
    char EscapeChar,
    Encoding? Encoding = null,
    bool SkipHeader = false,
    char? CommentCharacter = null
)
{
    public static CsvOptions Default => new(';', '"', '\\', Encoding.UTF8, false, null);

    public static CsvOptions Rfc4180 => new(';', '"', '"', Encoding.UTF8, false, null);
}


