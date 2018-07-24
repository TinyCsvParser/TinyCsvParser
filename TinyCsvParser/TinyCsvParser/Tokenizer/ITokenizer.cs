// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using IToken = System.Buffers.IMemoryOwner<char>;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer
{
    public sealed class EmptyToken : IToken
    {
        public static readonly IToken Instance = new EmptyToken();

        private EmptyToken() { }

        Memory<char> IToken.Memory => Memory<char>.Empty;

        void IDisposable.Dispose() { }
    }

    public sealed class EmptyTokens : ITokens
    {
        public static readonly ITokens Instance = new EmptyTokens();

        private readonly Memory<IToken> _memory;

        private EmptyTokens()
        {
            _memory = new[] { EmptyToken.Instance }.AsMemory();
        }

        Memory<IToken> ITokens.Memory => _memory;

        void IDisposable.Dispose() { }
    }

    public interface ITokenizer
    {
        ITokens Tokenize(ReadOnlySpan<char> input);
    }

    public interface ITokenizer2
    {
        TokenEnumerable Tokenize(ReadOnlySpan<char> input);
    }
}
