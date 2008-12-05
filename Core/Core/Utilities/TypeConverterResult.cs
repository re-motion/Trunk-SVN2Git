// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
