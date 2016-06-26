// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Token
    {
        public TokenTypeEnum Type;

        public StringBuilder Content;

        public Token()
        {
            Content = new StringBuilder();
        }

        public override string ToString()
        {
            return string.Format("Token (Type = {0}, Content = {1})", Type, Content);
        }
    }
}
