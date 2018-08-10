﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser
{
    public class CsvReaderOptions
    {
        public readonly string NewLine;

        public CsvReaderOptions(string newLine)
        {
            NewLine = newLine;
        }

        public CsvReaderOptions(char newLine)
        {
            NewLine = new string(newLine, 1);
        }

        public override string ToString()
        {
            return $"CsvReaderOptions (NewLineSeparator = '{NewLine}')";
        }
    }
}
