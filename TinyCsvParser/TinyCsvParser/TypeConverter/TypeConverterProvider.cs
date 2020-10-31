// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using TinyCsvParser.Exceptions;
using TinyCsvParser.Reflection;

namespace TinyCsvParser.TypeConverter
{
    public class TypeConverterProvider : ITypeConverterProvider
    {
        private readonly IDictionary<Type, ITypeConverter> typeConverters;

        public TypeConverterProvider()
        {
            typeConverters = new Dictionary<Type, ITypeConverter>();

            // Single Converters:
            Add(new BoolConverter());
            Add(new ByteConverter());
            Add(new DateTimeConverter());
            Add(new DecimalConverter());
            Add(new DoubleConverter());
            Add(new GuidConverter());
            Add(new Int16Converter());
            Add(new Int32Converter());
            Add(new Int64Converter());
            Add(new NullableBoolConverter());
            Add(new NullableByteConverter());
            Add(new NullableDateTimeConverter());
            Add(new NullableDecimalConverter());
            Add(new NullableDoubleConverter());
            Add(new NullableGuidConverter());
            Add(new NullableInt16Converter());
            Add(new NullableInt32Converter());
            Add(new NullableInt64Converter());
            Add(new NullableSByteConverter());
            Add(new NullableSingleConverter());
            Add(new NullableTimeSpanConverter());
            Add(new NullableUInt16Converter());
            Add(new NullableUInt32Converter());
            Add(new NullableUInt64Converter());
            Add(new SByteConverter());
            Add(new SingleConverter());
            Add(new StringConverter());
            Add(new TimeSpanConverter());
            Add(new UInt16Converter());
            Add(new UInt32Converter());
            Add(new UInt64Converter());

            // Collection Converters:
            Add(new ArrayConverter<bool>(new BoolConverter()));
            Add(new ArrayConverter<byte>(new ByteConverter()));
            Add(new ArrayConverter<DateTime>(new DateTimeConverter()));
            Add(new ArrayConverter<decimal>(new DecimalConverter()));
            Add(new ArrayConverter<double>(new DoubleConverter()));
            Add(new ArrayConverter<Guid>(new GuidConverter()));
            Add(new ArrayConverter<Int16>(new Int16Converter()));
            Add(new ArrayConverter<Int32>(new Int32Converter()));
            Add(new ArrayConverter<Int64>(new Int64Converter()));
            Add(new ArrayConverter<bool?>(new NullableBoolConverter()));
            Add(new ArrayConverter<Byte?>(new NullableByteConverter()));
            Add(new ArrayConverter<DateTime?>(new NullableDateTimeConverter()));
            Add(new ArrayConverter<Decimal?>(new NullableDecimalConverter()));
            Add(new ArrayConverter<double?>(new NullableDoubleConverter()));
            Add(new ArrayConverter<Guid?>(new NullableGuidConverter()));
            Add(new ArrayConverter<Int16?>(new NullableInt16Converter()));
            Add(new ArrayConverter<Int32?>(new NullableInt32Converter()));
            Add(new ArrayConverter<Int64?>(new NullableInt64Converter()));
            Add(new ArrayConverter<SByte?>(new NullableSByteConverter()));
            Add(new ArrayConverter<float?>(new NullableSingleConverter()));
            Add(new ArrayConverter<TimeSpan?>(new NullableTimeSpanConverter()));
            Add(new ArrayConverter<UInt16?>(new NullableUInt16Converter()));
            Add(new ArrayConverter<UInt32?>(new NullableUInt32Converter()));
            Add(new ArrayConverter<UInt64?>(new NullableUInt64Converter()));
            Add(new ArrayConverter<SByte>(new SByteConverter()));
            Add(new ArrayConverter<Single>(new SingleConverter()));
            Add(new ArrayConverter<String>(new StringConverter()));
            Add(new ArrayConverter<TimeSpan>(new TimeSpanConverter()));
            Add(new ArrayConverter<UInt16>(new UInt16Converter()));
            Add(new ArrayConverter<UInt32>(new UInt32Converter()));
            Add(new ArrayConverter<UInt64>(new UInt64Converter()));
        }

        public TypeConverterProvider Add<TTargetType>(ITypeConverter<TTargetType> typeConverter)
        {
            if (typeConverters.ContainsKey(typeConverter.TargetType))
            {
                throw new CsvTypeConverterAlreadyRegisteredException($"Duplicate TypeConverter registration for Type {typeConverter.TargetType}");
            }

            typeConverters[typeConverter.TargetType] = typeConverter;

            return this;
        }

        public TypeConverterProvider Add<TTargetType>(IArrayTypeConverter<TTargetType> typeConverter)
        {
            if (typeConverters.ContainsKey(typeConverter.TargetType))
            {
                throw new CsvTypeConverterAlreadyRegisteredException($"Duplicate TypeConverter registration for Type {typeConverter.TargetType}");
            }

            typeConverters[typeConverter.TargetType] = typeConverter;

            return this;
        }

        public ITypeConverter<TTargetType> Resolve<TTargetType>()
        {
            Type targetType = typeof(TTargetType);

            if (!typeConverters.TryGetValue(targetType, out var typeConverter))
            {
                throw new CsvTypeConverterNotRegisteredException($"No TypeConverter registered for Type {targetType}");
            }

            return typeConverter as ITypeConverter<TTargetType>;
        }

        public IArrayTypeConverter<TTargetType> ResolveCollection<TTargetType>()
        {
            Type targetType = typeof(TTargetType);
            
            if (!typeConverters.TryGetValue(targetType, out var typeConverter))
            {
                throw new CsvTypeConverterNotRegisteredException($"No TypeConverter registered for Type {targetType}");
            }

            return typeConverter as IArrayTypeConverter<TTargetType>;
        }
    }
}