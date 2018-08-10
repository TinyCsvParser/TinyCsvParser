﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class StringConverter : BaseConverter<string>
    {
        public override bool TryConvert(ReadOnlySpan<char> value, out string result)
        {
            result = value.ToString();

            return true;
        }
    }
}