using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Tokenizer.Rfc4180
{
    public class Token
    {
        public readonly TokenTypeEnum Type;

        public readonly string Content;

        public Token(TokenTypeEnum type)
            : this(type, string.Empty)
        {
        }

        public Token(TokenTypeEnum type, String content)
        {
            Type = type;
            Content = content;
        }
    }
}
