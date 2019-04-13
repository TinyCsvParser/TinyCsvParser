// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.Exceptions;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class TypeConverterProviderTests
    {
        TypeConverterProvider provider;

        [TestInitialize]
        public void SetUp()
        {
            provider = new TypeConverterProvider();
        }

        private enum SomeEnum
        {
            A = 1
        }

        [TestMethod]
        public void AddTypeRegistrationTest()
        {
            var typeConverter = provider
                .Add(new EnumConverter<SomeEnum>())
                .Resolve<SomeEnum>();

            Assert.AreEqual(typeof(EnumConverter<SomeEnum>), typeConverter.GetType());
        }

        [TestMethod]
        public void PreventDuplicateTypeRegistrationTest()
        {
            Assert.ThrowsException<CsvTypeConverterAlreadyRegisteredException>(() => provider.Add(new Int32Converter()));
        }

        [TestMethod]
        public void ResolveTypeConverter_Registered_Test()
        {
            var typeRegistration = provider.Resolve<Int16>();
            
            Assert.AreEqual(typeof(Int16Converter), typeRegistration.GetType());
        }

        [TestMethod]
        public void ResolveTypeConverter_NotRegistered_Test()
        {
            Assert.ThrowsException<CsvTypeConverterNotRegisteredException>(() => provider.Resolve<SomeEnum>());
        }

    }
}
