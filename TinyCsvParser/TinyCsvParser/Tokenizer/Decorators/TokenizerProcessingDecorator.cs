using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCsvParser.Tokenizer.Decorators
{
  public class TokenizerProcessingDecorator : ITokenizer
  {
    private readonly Postprocessor _postprocessor;

    private readonly Preprocessor _preprocessor;
    private readonly ITokenizer _tokenizer;

    public TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor)
      : this(tokenizer, preprocessor, new Postprocessor())
    {
    }

    public TokenizerProcessingDecorator(ITokenizer tokenizer, Postprocessor postprocessor)
      : this(tokenizer, new Preprocessor(), postprocessor)
    {
    }

    public TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor, Postprocessor postprocessor)
    {
      _tokenizer = tokenizer;

      _preprocessor = preprocessor;
      _postprocessor = postprocessor;
    }

    public string[] Tokenize(string input)
    {
      var preprocessedInput = _preprocessor.Processor(input);

      var tokenizedInput = _tokenizer.Tokenize(preprocessedInput);

      return tokenizedInput
        .Select(token => _postprocessor.Processor(token))
        .ToArray();
    }

    public KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input)
    {
      return new KeyValuePair<int, string[]>(input.Key, Tokenize(input.Value));
    }

    public override string ToString()
    {
      return string.Format("TokenizerProcessingDecorator (Preprocessor = {0}, Postprocessor = {1})", _preprocessor, _postprocessor);
    }

    public class Preprocessor
    {
      public readonly Func<string, string> Processor;

      public Preprocessor()
        : this(x => x)
      {
      }

      public Preprocessor(Func<string, string> preprocessor)
      {
        Processor = preprocessor;
      }
    }

    public class Postprocessor
    {
      public readonly Func<string, string> Processor;

      public Postprocessor()
        : this(x => x)
      {
      }

      public Postprocessor(Func<string, string> preprocessor)
      {
        Processor = preprocessor;
      }
    }
  }
}