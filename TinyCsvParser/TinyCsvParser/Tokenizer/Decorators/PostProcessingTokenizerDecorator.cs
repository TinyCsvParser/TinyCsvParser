using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Tokenizer.Decorators
{
    public class PostProcessingTokenizerDecorator : ITokenizer
    {
        private readonly ITokenizer tokenizer;

        private readonly Func<string, string> processor;

        public PostProcessingTokenizerDecorator(ITokenizer tokenizer, Func<string, string> processor)
        {
            this.tokenizer = tokenizer;
            this.processor = processor;
        }

        public string[] Tokenize(string input)
        {
            var originalResult = tokenizer.Tokenize(input);

            return originalResult
                .Select(token => processor(token))
                .ToArray();
        }
    }
}
