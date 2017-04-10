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

    private readonly Options _options;

    public Reader(Options options) => _options = options;

    public IList<Token> ReadTokens(StringReader reader)
    {
      var tokens = new List<Token>();
      while (true)
      {
        var token = NextToken(reader);

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

      string result;

      var c = reader.Peek();

      if (c == _options.DelimiterCharacter)
      {
        reader.Read();

        return new Token(TokenType.Token);
      }
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

      if (IsEndOfStream(c))
      {
        return new Token(TokenType.EndOfRecord);
      }
      result = reader.ReadTo(_options.DelimiterCharacter).Trim();

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

    private string ReadQuoted(StringReader reader)
    {
      reader.Read();

      var result = reader.ReadTo(_options.QuoteCharacter);

      reader.Read();

      if (reader.Peek() != _options.QuoteCharacter)
      {
        return result;
      }

      var buffer = new StringBuilder(result);
      do
      {
        buffer.Append((char) reader.Read());
        buffer.Append(reader.ReadTo(_options.QuoteCharacter));

        reader.Read();
      } while (reader.Peek() == _options.QuoteCharacter);

      return buffer.ToString();
    }

    private void Skip(StringReader reader)
    {
      while (IsWhiteSpace(reader.Peek()))
      {
        reader.Read();
      }
    }

    private bool IsQuoteCharacter(int c) => c == _options.QuoteCharacter;

    private static bool IsEndOfStream(int c) => c == -1;

    private bool IsDelimiter(int c) => c == _options.DelimiterCharacter;

    private static bool IsWhiteSpace(int c) => c == ' ' || c == '\t';

    public override string ToString() => $"Reader (Options = {_options})";

    public class Token
    {
      public readonly string Content;
      public readonly TokenType Type;

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
  }
}