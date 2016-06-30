// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Reader
    {
        public enum TokenType
        {
            Token,
            EndOfRecord
        }

        public class Token
        {
            public readonly TokenType Type;

            public readonly string Content;

            public Token(TokenType type)
                : this(type, string.Empty)
            {
            }

            public Token(TokenType type, string content)
            {
                Type = type;
                Content = content;
            }
        }

        private Options options;

        public Reader(Options options)
        {
            this.options = options;
        }

        public IList<Token> ReadTokens(StringReader reader)
        {
            var tokens = new List<Token>();
            while (true)
            {
                Token token = NextToken(reader);
                if (token.Type == TokenType.EndOfRecord)
                {
                    break;
                }
                tokens.Add(token);
            }
            return tokens;
        }

        private Token NextToken(StringReader reader)
        {
            Skip(reader);

            string result = string.Empty;

            int c = reader.Peek();
            if (c == options.DelimiterCharacter)
            {
                reader.Read();

                return new Token(TokenType.Token);
            }
            else
            {
                if (IsQuoteCharacter(c))
                {
                    result = ReadQuoted(reader);

                    Skip(reader);

                    if (IsDelimiter(reader.Peek()))
                    {
                        reader.Read();
                    }

                    return new Token(TokenType.Token, result);
                }

                if (IsCarriageReturn(c))
                {
                    if (reader.Peek() == '\n')
                    {
                        reader.Read();
                    }
                }

                if (IsLineFeed(c))
                {
                    reader.Read();
                    return new Token(TokenType.EndOfRecord);
                } 
                
                if (IsEndOfStream(c)) 
                {
                    return new Token(TokenType.EndOfRecord);
                } 
                else
                {
                    result = reader.ReadTo(options.DelimiterCharacter);
                    if(IsDelimiter(reader.Peek())) 
                    {
                        reader.Read();
                    }
                    return new Token(TokenType.Token, result);
                }
            }
        }

        private string ReadQuoted(StringReader reader)
        {
            reader.Read();

            string result = reader.ReadTo(options.QuoteCharacter);

            reader.Read();

            if (reader.Peek() != options.QuoteCharacter)
            {
                return result;
            }
         
            StringBuilder buffer = new StringBuilder(result);
            do
            {
                buffer.Append((char)reader.Read());
                buffer.Append(reader.ReadTo(options.QuoteCharacter));

                reader.Read();
            } while (reader.Peek() == options.QuoteCharacter);

            return buffer.ToString();
        }

        private void Skip(StringReader reader)
        {
            while (IsWhiteSpace(reader.Peek()))
            {
                reader.Read();
            }
        }

        private bool IsQuoteCharacter(int c) {
            return c == options.QuoteCharacter;
        }

        private bool IsCarriageReturn(int c)
        {
            return c == '\r';
        }

        private bool IsLineFeed(int c)
        {
            return c == '\n';
        }

        private bool IsEndOfStream(int c)
        {
            return c == -1;
        }

        private bool IsDelimiter(int c)
        {
            return c == options.DelimiterCharacter;
        }

        private bool IsWhiteSpace(int c)
        {
            return c == ' ' || c == '\t';
        }
    }
}