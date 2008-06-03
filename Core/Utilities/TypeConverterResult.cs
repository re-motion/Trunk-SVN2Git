/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel;

namespace Remotion.Utilities
{
  /// <summary>
  /// The <see cref="TypeConverterResult"/> structure encapsulates the reult of the <see cref="TypeConversionProvider.GetTypeConverter(Type,Type)"/>
  /// method.
  /// </summary>
  public struct TypeConverterResult
  {
    public static readonly TypeConverterResult Empty = new TypeConverterResult();

    public readonly TypeConverterType TypeConverterType;
    public readonly TypeConverter TypeConverter;

    public TypeConverterResult (TypeConverterType typeConverterType, TypeConverter typeConverter)
    {
      TypeConverterType = typeConverterType;
      TypeConverter = typeConverter;
    }
  }
}
