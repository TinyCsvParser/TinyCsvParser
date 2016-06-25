// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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

        public override string ToString()
        {
            return string.Format("Token (Type = {0}, Content = {1})", Type, Content);
        }
    }
}
