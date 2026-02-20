// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace TinyCsvParser.TypeConverters;

public class TypeConverterProvider : ITypeConverterProvider
{
    private readonly Dictionary<Type, ITypeConverter> _converters = new Dictionary<Type, ITypeConverter>();

    public TypeConverterProvider()
    {
        AddStandardConverters();
    }

    private void AddStandardConverters()
    {
        Register(new SByteConverter());
        Register(new ByteConverter());
        Register(new Int16Converter());
        Register(new UInt16Converter());
        Register(new Int32Converter());
        Register(new UInt32Converter());
        Register(new Int64Converter());
        Register(new UInt64Converter());
        Register(new SingleConverter());
        Register(new DoubleConverter());
        Register(new DecimalConverter());
        Register(new BoolConverter());
        Register(new StringConverter());
        Register(new GuidConverter());
        Register(new TimeSpanConverter());
        Register(new DateTimeConverter());

        Register(new NullableSByteConverter());
        Register(new NullableByteConverter());
        Register(new NullableInt16Converter());
        Register(new NullableUInt16Converter());
        Register(new NullableInt32Converter());
        Register(new NullableUInt32Converter());
        Register(new NullableInt64Converter());
        Register(new NullableUInt64Converter());
        Register(new NullableSingleConverter());
        Register(new NullableDoubleConverter());
        Register(new NullableDecimalConverter());
        Register(new NullableBoolConverter());
        Register(new NullableGuidConverter());
        Register(new NullableTimeSpanConverter());
        Register(new NullableDateTimeConverter());
    }

    public TypeConverterProvider Register<T>(ITypeConverter<T> converter)
    {
        _converters[typeof(T)] = converter;
        return this;
    }

    public ITypeConverter<T> Resolve<T>()
    {
        var type = typeof(T);

        if (_converters.TryGetValue(type, out var converter))
        {
            return (ITypeConverter<T>)converter;
        }

        throw new NotSupportedException($"No TypeConverter for Type '{type.FullName}'.");
    }
}


