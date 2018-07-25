using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System
{
    // Based on: https://gist.github.com/LordJZ/e0b5245d69497f2a43a5f09c1d26e34c

    public ref struct SpanSplitEnumerable
    {
        private readonly char _separator;
        private readonly ReadOnlySpan<char> _separators;
        private readonly ReadOnlySpan<char> _span;
        private readonly StringSplitOptions _options;

        public SpanSplitEnumerable(ReadOnlySpan<char> span, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
        {
            _span = span;
            _separators = separators;
            _separator = (char)0;
            _options = options;
        }

        public SpanSplitEnumerable(ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            _span = span;
            _separators = ReadOnlySpan<char>.Empty;
            _separator = separator;
            _options = options;
        }

        public SpanSplitEnumerator GetEnumerator() => new SpanSplitEnumerator(_span, _separators, _separator, _options);

        /// <summary>
        ///     Converts the span enumerable to a string array.
        ///     Useful for storing results - do not use when avoiding allocations.
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

    public ref struct SpanSplitEnumerator
    {
        static private long _sentinel;

        private readonly char _separator;
        private readonly ReadOnlySpan<char> _separators;
        private ReadOnlySpan<char> _span;
        private readonly StringSplitOptions _options;

        public SpanSplitEnumerator(ReadOnlySpan<char> span, ReadOnlySpan<char> separators, char separator, StringSplitOptions options)
        {
            _span = span;
            _separators = separators;
            _separator = separator;
            _options = options;
            Current = default;

            if (_span.IsEmpty)
                TrailingEmptyItem = true;
        }

        unsafe ReadOnlySpan<char> TrailingEmptyItemSentinel =>
            new ReadOnlySpan<char>(Unsafe.AsPointer(ref _sentinel), 1);

        bool TrailingEmptyItem
        {
            get => _span == TrailingEmptyItemSentinel;
            set => _span = value ? TrailingEmptyItemSentinel : default;
        }

        public bool MoveNext()
        {
            if (TrailingEmptyItem)
            {
                TrailingEmptyItem = false;
                Current = default;
                return _options == 0;
            }

        next:
            if (_span.IsEmpty)
            {
                _span = Current = default;
                return false;
            }

            int idx = _separators.IsEmpty ? _span.IndexOf(_separator) : _span.IndexOfAny(_separators);

            if (idx < 0)
            {
                Current = _span;
                _span = default;
            }
            else
            {
                Current = _span.Slice(0, idx);
                _span = _span.Slice(idx + 1);

                if (_options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
                    goto next;

                if (_span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            return true;
        }

        public ReadOnlySpan<char> Current { get; private set; }
    }

    public static class SpanSplitExtensions
    {
        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitEnumerable(span, separator, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitEnumerable(span, separators, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, char[] separators, StringSplitOptions options = StringSplitOptions.None)
           => new SpanSplitEnumerable(span, separators, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, params char[] separators)
            => new SpanSplitEnumerable(span, separators);
    }
}