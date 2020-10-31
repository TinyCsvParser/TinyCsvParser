// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public static class StringReaderExtensions
    {
        public static string ReadTo(this StringReader reader, char readTo)
        {
            StringBuilder buffer = new StringBuilder();
            while(reader.Peek() != -1 && reader.Peek() != readTo) 
            {
                buffer.Append((char) reader.Read());
            }
            return buffer.ToString();
        }
    }
}
