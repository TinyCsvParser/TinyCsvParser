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
               
                tokens.Add(token);

                if (token.Type == TokenType.EndOfRecord)
                {
                    break;
                }
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
                if (!options.StrictDelimitation)
                {
                    if (IsQuoteCharacter(c))
                    {
                        result = ReadQuoted(reader);

                        Skip(reader);

                        if (IsEndOfStream(reader.Peek()))
                        {
                            return new Token(TokenType.EndOfRecord, result);
                        }

                        if (IsDelimiter(reader.Peek()))
                        {
                            reader.Read();
                        }

                        return new Token(TokenType.Token, result);
                    }
                }

                if (IsEndOfStream(c)) 
                {
                    return new Token(TokenType.EndOfRecord);
                } 
                else
                {
                    result = reader.ReadTo(options.DelimiterCharacter).Trim();

                    if (options.StrictDelimitation)
                    {
                        result = result.TrimStart(options.QuoteCharacter).TrimEnd(options.QuoteCharacter);
                    }

                    Skip(reader);

                    if (IsEndOfStream(reader.Peek()))
                    {
                        return new Token(TokenType.EndOfRecord, result);
                    }

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
            if (IsDelimiter(c))
            {
                return false;
            }

            return c == ' ' || c == '\t';
        }

        public override string ToString()
        {
            return $"Reader (Options = {options})";
        }
    }
}