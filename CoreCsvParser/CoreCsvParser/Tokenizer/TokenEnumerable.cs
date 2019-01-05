using System;
using System.Collections.Generic;

namespace CoreCsvParser.Tokenizer
{
    public delegate ReadOnlySpan<char> NextTokenDelegate(ReadOnlySpan<char> input, out ReadOnlySpan<char> remaining, out bool foundToken);

    public ref struct TokenEnumerable
    {
        public TokenEnumerable(ReadOnlySpan<char> line, NextTokenDelegate nextToken)
        {
            Line = line;
            NextToken = nextToken;
        }

        private ReadOnlySpan<char> Line { get; }
        private NextTokenDelegate NextToken { get; }
        public TokenEnumerator GetEnumerator() => new TokenEnumerator(Line, NextToken);

        /// <summary>
        ///     Converts the token enumerable to a string array.
        ///     Useful for storing results - do not use in high-performance scenarios.
        /// </summary>
        public string[] ToArray()
        {
            var list = new List<string>();
            foreach (var part in this)
            {
                list.Add(part.ToString());
            }
            return list.ToArray();
        }
    }

    public ref struct TokenEnumerator
    {
        private ReadOnlySpan<char> _line;

        public TokenEnumerator(ReadOnlySpan<char> line, NextTokenDelegate nextToken)
        {
            _line = line;
            NextToken = nextToken;
            Current = default;
        }

        private NextTokenDelegate NextToken { get; }

        public ReadOnlySpan<char> Current { get; private set; }

        public bool MoveNext()
        {
            Current = NextToken(_line, out _line, out bool foundToken);
            return foundToken;
        }
    }
}
