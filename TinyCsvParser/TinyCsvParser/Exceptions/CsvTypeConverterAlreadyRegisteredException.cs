﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Exceptions
{
  public class CsvTypeConverterAlreadyRegisteredException : Exception
  {
    public CsvTypeConverterAlreadyRegisteredException()
    {
    }

    public CsvTypeConverterAlreadyRegisteredException(string message)
        : base(message)
    {
    }

    public CsvTypeConverterAlreadyRegisteredException(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}
