// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class TimeSpanConverterTest : BaseConverterTest<TimeSpan>
    {
        protected override ITypeConverter<TimeSpan> Converter
        {
            get { return new TimeSpanConverter(); }
        }

        protected override Tuple<string, TimeSpan>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
                    MakeTuple("14", TimeSpan.FromDays(14)),
                    MakeTuple("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }

    [TestFixture]
    public class TimeSpanConverterCustomFormatTest : BaseConverterTest<TimeSpan>
    {
        protected override ITypeConverter<TimeSpan> Converter
        {
            get { return new TimeSpanConverter(@"hh\:mm\:ss"); }
        }

        protected override Tuple<string, TimeSpan>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("01:02:03", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}
