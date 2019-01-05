// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public class NullableGuidConverter : NullableInnerConverter<Guid>
    {
        public NullableGuidConverter()
            : base(new GuidConverter())
        {
        }

        public NullableGuidConverter(string format)
            : base(new GuidConverter(format))
        {
        }
    }
}