// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        // TODO: implementations could use array pooling, if there's a reliable place to return the arrays when finished.
        // TODO??: System.Buffers.IMemoryOwner<char>[] so the memory and the array get rented? What happens if the pool is bigger than we asked for?
        // Might need custom version of ArrayMemoryPool that slices the returned Memory to the requested size.
        // https://github.com/dotnet/corefx/blob/master/src/System.Memory/src/System/Buffers/ArrayMemoryPool.cs
        ReadOnlyMemory<char>[] Tokenize(ReadOnlySpan<char> input);
    }
}
