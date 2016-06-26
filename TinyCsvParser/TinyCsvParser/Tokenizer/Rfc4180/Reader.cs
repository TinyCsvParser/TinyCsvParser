// Adapted from: https://github.com/apache/commons-csv/blob/trunk/src/main/java/org/apache/commons/csv/Lexer.java
//
// This needs to be turned into a more generic state machine for simpler verification of the Lexer. It is 
// not finished yet and may not be RFC 4180-compliant already.
//
// The Original License:
/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.IO;

namespace TinyCsvParser.Tokenizer.Rfc4180
{
    public class Reader
    {
        private Options options;

        public Reader(Options options)
        {
            this.options = options;
        }

        public IList<Token> ReadTokens(StringReader reader)
        {
            var result = new List<Token>();

            while (true)
            {
                // Get the Next Token from the Stream:
                Token token = NextToken(reader);

                // Add to the Result list:
                result.Add(token);

                // Break on End of Stream:
                if (token.Type != TokenTypeEnum.Token)
                {
                    break;
                }
            }

            return result;
        }
        
        private Token NextToken(StringReader reader)
        {
            Token token = new Token();

            int c = reader.Read();
            if (IsQuoteChar(c))
            {
                ParseEncapsulatedToken(reader, token);
            }
            else
            {
                ParseSimpleToken(reader, token, c);
            }
            return token;
        }

        private void ParseSimpleToken(StringReader reader, Token token, int ch)
        {
            while (true)
            {
                if (ch == -1)
                {
                    token.Type = TokenTypeEnum.EndOfRecord;
                    break;
                }
                else if (IsDelimiter(ch))
                {
                    token.Type = TokenTypeEnum.Token;
                    break;
                }
                else if (IsEscape(ch))
                {
                    token.Content.Append((char)ReadEscaped(reader));
                }
                else
                {
                    token.Content.Append((char)ch);
                }
                ch = reader.Read();
            }
        }

        private Token ParseEncapsulatedToken(StringReader reader, Token token)
        {
            int c;
            while (true)
            {
                c = reader.Read();
                // Stop Processing, if we encounter the end of the Stream:
                if (c == -1)
                {
                    token.Type = TokenTypeEnum.EndOfRecord;
                    return token;
                } 
                else if (IsEscape(c))
                {
                    int escaped = ReadEscaped(reader);
                    if(escaped == -1) {
                        token.Type = TokenTypeEnum.EndOfRecord;
                        return token;
                    }
                    token.Content.Append((char) escaped);
                }
                else if (IsQuoteChar(c))
                {
                    // If the next character is also a quote, add it to the result:
                    if (IsQuoteChar(reader.Peek()))
                    {
                        c = reader.Read();
                        token.Content.Append((char)c);
                    }
                    else
                    {
                        while (true)
                        {
                            c = reader.Read();

                            // Stop processing, if we hit the end of the line:
                            if (c == -1)
                            {
                                token.Type = TokenTypeEnum.EndOfRecord;
                                return token;
                            }

                            // Stop processing this token, if we encounter the delimiter:
                            else if (IsDelimiter(c))
                            {
                                token.Type = TokenTypeEnum.Token;
                                return token;
                            }
                        }
                    }
                }
                else
                {
                    token.Content.Append((char)c);
                }
            }
        }

        private int ReadEscaped(StringReader reader)
        {
            int ch = reader.Read();
            switch (ch)
            {
                case 't':
                    return Constants.TAB;
                case 'b':
                    return Constants.BACKSPACE;
                case 'n':
                    return Constants.LF;
                case 'r':
                    return Constants.CR;
                case 'f':
                    return Constants.FF;
                default:
                    return ch;
            }
        }

        private bool IsEscape(int c)
        {
            return c == options.EscapeCharacter;
        }

        private bool IsQuoteChar(int c)
        {
            return c == options.QuoteCharacter;
        }

        private bool IsDelimiter(int c)
        {
            return c == options.DelimiterCharacter;
        }
    }
}
