// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer
    {
        private readonly Options options;

        public RFC4180Tokenizer(Options options)
        {
            this.options = options;
        }

        private enum State
        {
            START_RECORD,
            START_FIELD,
            ESCAPED_CHAR,
            IN_FIELD,
            IN_QUOTED_FIELD,
            ESCAPE_IN_QUOTED_FIELD,
            QUOTE_IN_QUOTED_FIELD,
            EAT_CRNL
        }

        public IEnumerable<TokenizedRow> Tokenize(IEnumerable<string> data)
        {
            var state = State.START_RECORD;

            var fields = new List<string>();
            var lines = new List<RowData>();
            var builder = new StringBuilder();

            int line_no = 0;

            foreach (var line in data)
            {
                line_no = line_no + 1;

                if(state != State.START_FIELD)
                {
                    lines.Add(new RowData { LineNo = line_no, Data = line });
                }

                lines.Add(new RowData { LineNo = line_no, Data = line });

                foreach (var c in line)
                {
                    if (c == '\0')
                    {
                        yield return new TokenizedRow()
                        {
                            Rows = lines.ToArray(),
                            Error = new TokenizeError
                            {
                                LineNo = line_no,
                                Reason = "Line contains NULL byte"
                            }
                        };
                    }

                    if (state == State.START_RECORD)
                    {
                        if (c == '\n' || c == '\r')
                        {
                            state = State.EAT_CRNL;
                            continue;
                        }

                        // Normal Character, handle as START_FIELD:
                        state = State.START_FIELD;

                        builder.Clear();
                        fields.Clear();
                        lines.Clear();

                        lines.Add(new RowData { LineNo = line_no, Data = line });
                    }

                    if (state == State.START_FIELD)
                    {
                        if (c == '\n' || c == '\r')
                        {
                            AddField(fields, builder);
                            state = State.EAT_CRNL;
                        }
                        else if (c == options.QuoteCharacter)
                        {
                            state = State.IN_QUOTED_FIELD;
                        }
                        else if (c == options.EscapeCharacter)
                        {
                            state = State.ESCAPED_CHAR;
                        }
                        else if (c == ' ' && options.SkipInitialWhitespaces)
                        {
                            // Nothing to do...
                        }
                        else if (c == options.DelimiterCharacter)
                        {
                            AddField(fields, builder);
                        }
                        else
                        {
                            AddChar(builder, c);
                            state = State.IN_FIELD;
                        }
                    }
                    else if (state == State.ESCAPED_CHAR)
                    {
                        AddChar(builder, c);
                        state = State.IN_FIELD;
                    }
                    else if (state == State.IN_FIELD)
                    {
                        if (c == '\r' || c == '\n')
                        {
                            AddField(fields, builder);
                            state = State.EAT_CRNL;
                        }
                        else if (c == options.EscapeCharacter)
                        {
                            state = State.ESCAPED_CHAR;
                        }
                        else if (c == options.DelimiterCharacter)
                        {
                            AddField(fields, builder);
                            state = State.START_FIELD;
                        }
                        else
                        {
                            AddChar(builder, c);
                        }
                    }
                    else if (state == State.IN_QUOTED_FIELD)
                    {
                        if (c == options.EscapeCharacter)
                        {
                            state = State.ESCAPE_IN_QUOTED_FIELD;
                        }
                        else if (c == options.QuoteCharacter)
                        {
                            state = State.QUOTE_IN_QUOTED_FIELD;
                        }
                        else
                        {
                            AddChar(builder, c);
                        }
                    }
                    else if (state == State.ESCAPE_IN_QUOTED_FIELD)
                    {
                        AddChar(builder, c);
                        state = State.IN_QUOTED_FIELD;
                    }
                    else if (state == State.QUOTE_IN_QUOTED_FIELD)
                    {
                        if (c == options.QuoteCharacter)
                        {
                            AddChar(builder, c);
                            state = State.IN_QUOTED_FIELD;
                        }
                        else if (c == options.DelimiterCharacter)
                        {
                            AddField(fields, builder);
                            state = State.START_FIELD;
                        }
                        else if (c == '\r' || c == '\n')
                        {
                            AddField(fields, builder);
                            state = State.EAT_CRNL;
                        }
                        else
                        {
                            yield return new TokenizedRow()
                            {
                                Rows = lines.ToArray(),
                                Error = new TokenizeError
                                {
                                    LineNo = line_no,
                                    Reason = $"'{options.DelimiterCharacter}' expected after '{options.QuoteCharacter}'"
                                }
                            };
                        }
                    }
                    else if (state == State.EAT_CRNL)
                    {
                        if (!(c == '\r' || c == '\n'))
                        {
                            yield return new TokenizedRow()
                            {
                                Rows = lines.ToArray(),
                                Error = new TokenizeError
                                {
                                    LineNo = line_no,
                                    Reason = "Newline Character found in unquoted field."
                                }
                            };
                        }
                    }
                }

                if (state == State.IN_FIELD || state == State.QUOTE_IN_QUOTED_FIELD)
                {
                    AddField(fields, builder);

                    state = State.START_RECORD;

                    yield return new TokenizedRow
                    {
                        Rows = lines.ToArray(),
                        Tokens = fields.ToArray()
                    };
                }
                else if (state == State.ESCAPED_CHAR)
                {
                    AddChar(builder, '\n');

                    state = State.IN_FIELD;
                }
                else if (state == State.IN_QUOTED_FIELD)
                {
                    AddChar(builder, '\n');
                }
                else if (state == State.ESCAPE_IN_QUOTED_FIELD)
                {
                    AddChar(builder, '\n');
                    state = State.IN_QUOTED_FIELD;
                }
                else if (state == State.START_FIELD)
                {
                    builder.Clear();

                    AddField(fields, builder);
                    state = State.START_RECORD;

                    yield return new TokenizedRow
                    {
                        Rows = lines.ToArray(),
                        Tokens = fields.ToArray()
                    };
                }
            }

            if (state != State.START_RECORD
                && state != State.EAT_CRNL
                && (builder.Length > 0 || state == State.IN_QUOTED_FIELD))
            {
                yield return new TokenizedRow()
                {
                    Rows = lines.ToArray(),
                    Error = new TokenizeError
                    {
                        LineNo = line_no,
                        Reason = "File ended in a Quoted Field or has data left in the buffer."
                    }
                };
            }
        }

        private static void AddChar(StringBuilder builder, char c)
        {
            builder.Append(c);
        }

        private static void AddField(List<string> fields, StringBuilder builder)
        {
            if (builder.Length == 0)
            {
                fields.Add(string.Empty);
            }
            else
            {
                fields.Add(builder.ToString());
                builder.Clear();
            }
        }
    }
}
