using System;

namespace TinyCsvParser.Tokenizer
{
    public delegate ReadOnlySpan<char> NextTokenDelegate(ReadOnlySpan<char> input, out ReadOnlySpan<char> remaining);

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
            if (_line.IsEmpty)
            {
                _line = Current = default;
                return false;
            }

            Current = NextToken(_line, out _line);
            return !(Current.IsEmpty && _line.IsEmpty);
        }
    }
}
