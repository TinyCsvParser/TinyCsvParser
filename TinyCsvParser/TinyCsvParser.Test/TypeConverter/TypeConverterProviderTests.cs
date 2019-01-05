// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.Exceptions;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class TypeConverterProviderTests
    {
        TypeConverterProvider provider;

        [SetUp]
        public void SetUp()
        {
            provider = new TypeConverterProvider();
        }

        private enum SomeEnum
        {
            A = 1
        }

        [Test]
        public void AddTypeRegistrationTest()
        {
            var typeConverter = provider
                .Add(new EnumConverter<SomeEnum>())
                .Resolve<SomeEnum>();

            Assert.AreEqual(typeof(EnumConverter<SomeEnum>), typeConverter.GetType());
        }

        [Test]
        public void PreventDuplicateTypeRegistrationTest()
        {
            Assert.Throws<CsvTypeConverterAlreadyRegisteredException>(() => provider.Add(new Int32Converter()));
        }

        [Test]
        public void ResolveTypeConverter_Registered_Test()
        {
            var typeRegistration = provider.Resolve<Int16>();
            
            Assert.AreEqual(typeof(Int16Converter), typeRegistration.GetType());
        }

        [Test]
        public void ResolveTypeConverter_NotRegistered_Test()
        {
            Assert.Throws<CsvTypeConverterNotRegisteredException>(() => provider.Resolve<SomeEnum>());
        }

    }
}
