// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class NullableSingleConverter : NullableInnerConverter<Single>
    {
        public NullableSingleConverter()
            : base(new SingleConverter())
        {
        }

        public NullableSingleConverter(IFormatProvider formatProvider)
            : base(new SingleConverter(formatProvider))
        {
        }

        public NullableSingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new SingleConverter(formatProvider, numberStyles))
        {
        }
   }
}