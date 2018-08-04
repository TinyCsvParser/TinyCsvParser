// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Issues
{

    [TestFixture]
    public class CsvMappingTests
    {
        private class SampleEntity
        {
            public int PropertyInt { get; set; }
        }

        private class DuplicateMapping : CsvMapping<SampleEntity>
        {
            public DuplicateMapping()
            {
                MapProperty(0, x => x.PropertyInt);
                MapProperty(0, x => x.PropertyInt);
            }
        }

        private class WrongColumnMapping : CsvMapping<SampleEntity>
        {
            public WrongColumnMapping()
            {
                MapProperty(1, x => x.PropertyInt);
            }
        }

        private static ReadOnlySpan<char> ReadOneToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining, out bool foundToken)
        {
            remaining = ReadOnlySpan<char>.Empty;
            foundToken = !chars.IsEmpty;
            return chars;
        }

        [Test]
        public void DuplicateMappingTest()
        {
            Assert.Throws<InvalidOperationException>(() => new DuplicateMapping());
        }

        [Test]
        public void MapEntity_Invalid_Column_Test()
        {
            var mapping = new WrongColumnMapping();

            var result = mapping.Map(new TokenEnumerable("1", ReadOneToken), 0);

            Assert.IsFalse(result.IsValid);
        }

        private class CorrectColumnMapping : CsvMapping<SampleEntity>
        {
            public CorrectColumnMapping()
            {
                MapProperty(0, x => x.PropertyInt);
            }
        }
        
        [Test]
        public void MapEntity_ConversionError_Test()
        {
            var mapping = new CorrectColumnMapping();

            var result = mapping.Map(new TokenEnumerable("a", ReadOneToken), 0);

            Assert.IsFalse(result.IsValid);

            Assert.AreEqual("Column 0 with Value 'a' cannot be converted.", result.Error.Message);
            Assert.AreEqual(0, result.Error.ColumnIndex);

            Assert.DoesNotThrow(() => result.ToString());
        }

        [Test]
        public void MapEntity_ConversionSuccess_Test()
        {
            var mapping = new CorrectColumnMapping();

            var result = mapping.Map(new TokenEnumerable("1", ReadOneToken), 0);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Result.PropertyInt);

            Assert.DoesNotThrow(() => result.ToString());
        }
    }
}
