// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public static class StringReaderExtensions
    {
        public static string ReadTo(this StringReader reader, HashSet<char> readTo)
        {
            StringBuilder buffer = new StringBuilder();
            while (reader.Peek() != -1 && !readTo.Contains((char)reader.Peek()))
            {
                buffer.Append((char)reader.Read());
            }
            return buffer.ToString();
        }
        public static string ReadTo(this StringReader reader, char readTo)
            => reader.ReadTo(new HashSet<char>(new char[] { readTo }));
    }
}
