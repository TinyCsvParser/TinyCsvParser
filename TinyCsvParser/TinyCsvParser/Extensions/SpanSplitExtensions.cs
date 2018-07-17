using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace TinyCsvParser.Extensions
{
    // Based on: https://gist.github.com/LordJZ/e0b5245d69497f2a43a5f09c1d26e34c

    public static class SpanSplitExtensions
    {
        static private long _sentinel;

        #region SpanSplitChar

        public ref struct SpanSplitCharEnumerable
        {
            public SpanSplitCharEnumerable(ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
            {
                Span = span;
                Separator = separator;
                Options = options;
            }

            ReadOnlySpan<char> Span { get; }
            char Separator { get; }
            StringSplitOptions Options { get; }

            public SpanSplitCharEnumerator GetEnumerator() => new SpanSplitCharEnumerator(Span, Separator, Options);

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

        public ref struct SpanSplitCharEnumerator
        {
            public SpanSplitCharEnumerator(ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
            {
                Span = span;
                Separator = separator;
                Options = options;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<char> Span { get; set; }
            char Separator { get; }
            StringSplitOptions Options { get; }
            int SeparatorLength => 1;

            unsafe ReadOnlySpan<char> TrailingEmptyItemSentinel =>
                new ReadOnlySpan<char>(Unsafe.AsPointer(ref _sentinel), SeparatorLength);

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return Options == 0;
                }

            next:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOf(Separator);

                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);

                    if (Options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
                        goto next;

                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<char> Current { get; private set; }
        }

        #endregion

        #region SpanSplitAnyChar 

        public ref struct SpanSplitAnyCharEnumerable
        {
            public SpanSplitAnyCharEnumerable(ReadOnlySpan<char> span, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
            {
                Span = span;
                Separators = separators;
                Options = options;
            }

            ReadOnlySpan<char> Span { get; }
            ReadOnlySpan<char> Separators { get; }
            StringSplitOptions Options { get; }

            public SpanSplitAnyCharEnumerator GetEnumerator() => new SpanSplitAnyCharEnumerator(Span, Separators, Options);

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

        public ref struct SpanSplitAnyCharEnumerator
        {
            public SpanSplitAnyCharEnumerator(ReadOnlySpan<char> span, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
            {
                Span = span;
                Separators = separators;
                Options = options;
                Current = default;

                if (Span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            ReadOnlySpan<char> Span { get; set; }
            ReadOnlySpan<char> Separators { get; }
            StringSplitOptions Options { get; }
            int SeparatorLength => 1;

            unsafe ReadOnlySpan<char> TrailingEmptyItemSentinel =>
                new ReadOnlySpan<char>(Unsafe.AsPointer(ref _sentinel), SeparatorLength);

            bool TrailingEmptyItem
            {
                get => Span == TrailingEmptyItemSentinel;
                set => Span = value ? TrailingEmptyItemSentinel : default;
            }

            public bool MoveNext()
            {
                if (TrailingEmptyItem)
                {
                    TrailingEmptyItem = false;
                    Current = default;
                    return Options == 0;
                }

            next:
                if (Span.IsEmpty)
                {
                    Span = Current = default;
                    return false;
                }

                int idx = Span.IndexOfAny(Separators);

                if (idx < 0)
                {
                    Current = Span;
                    Span = default;
                }
                else
                {
                    Current = Span.Slice(0, idx);
                    Span = Span.Slice(idx + SeparatorLength);

                    if (Options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
                        goto next;

                    if (Span.IsEmpty)
                        TrailingEmptyItem = true;
                }

                return true;
            }

            public ReadOnlySpan<char> Current { get; private set; }
        }

        #endregion

        [Pure]
        public static SpanSplitCharEnumerable Split(this ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitCharEnumerable(span, separator, options);

        [Pure]
        public static SpanSplitAnyCharEnumerable Split(this ReadOnlySpan<char> span, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitAnyCharEnumerable(span, separators, options);

        [Pure]
        public static SpanSplitAnyCharEnumerable Split(this ReadOnlySpan<char> span, char[] separators, StringSplitOptions options = StringSplitOptions.None)
           => new SpanSplitAnyCharEnumerable(span, separators, options);

        [Pure]
        public static SpanSplitAnyCharEnumerable Split(this ReadOnlySpan<char> span, params char[] separators)
            => new SpanSplitAnyCharEnumerable(span, separators);
    }
}