using System;
using System.Buffers;
using IToken = System.Buffers.IMemoryOwner<char>;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer.Decorators
{
    public class TokenizerProcessingDecorator : ITokenizer
    {
        private readonly ITokenizer tokenizer;

        public delegate ReadOnlySpan<char> ProcessorDelegate(ReadOnlySpan<char> input);

        public class Preprocessor
        {
            public static readonly Preprocessor Default = new Preprocessor(x => x, true);

            public readonly ProcessorDelegate Processor;

            public readonly bool IsDefault;

            public Preprocessor(ProcessorDelegate processor) : this(processor, false) { }

            private Preprocessor(ProcessorDelegate processor, bool isDefault)
            {
                Processor = processor;
                IsDefault = isDefault;
            }
        }

        public class Postprocessor
        {
            public static readonly Postprocessor Default = new Postprocessor(x => x, true);

            public readonly ProcessorDelegate Processor;

            public readonly bool IsDefault;

            public Postprocessor(ProcessorDelegate processor) : this(processor, false) { }

            private Postprocessor(ProcessorDelegate processor, bool isDefault)
            {
                Processor = processor;
                IsDefault = isDefault;
            }
        }

        private readonly Preprocessor preprocessor;
        private readonly Postprocessor postprocessor;

        public TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor)
            : this(tokenizer, preprocessor, Postprocessor.Default)
        {
        }

        public TokenizerProcessingDecorator(ITokenizer tokenizer, Postprocessor postprocessor)
            : this(tokenizer, Preprocessor.Default, postprocessor)
        {
        }

        public TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor, Postprocessor postprocessor)
        {
            this.tokenizer = tokenizer;

            this.preprocessor = preprocessor;
            this.postprocessor = postprocessor;
        }

        public ITokens Tokenize(ReadOnlySpan<char> input)
        {
            var preprocessed_input = preprocessor.IsDefault ? input : preprocessor.Processor(input);
            var tokenized_input = tokenizer.Tokenize(preprocessed_input);
            var inputSpan = tokenized_input.Memory.Span;
            var pool = SizedMemoryPool<char>.Instance;

            for (int i = 0, len = inputSpan.Length; i < len; i++)
            {
                if (!postprocessor.IsDefault)
                {
                    var token = inputSpan[i];
                    var tokenSpan = token.Memory.Span;
                    var processed = postprocessor.Processor(tokenSpan);
                    var newToken = pool.Rent(processed.Length);
                    processed.CopyTo(newToken.Memory.Span);
                    token.Dispose();
                    inputSpan[i] = newToken;
                }
            }

            return tokenized_input;
        }

        public override string ToString()
        {
            return string.Format("TokenizerProcessingDecorator (Preprocessor = {0}, Postprocessor = {1})", preprocessor, postprocessor);
        }
    }
}
