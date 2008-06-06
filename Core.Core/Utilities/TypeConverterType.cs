/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.ComponentModel;

namespace Remotion.Utilities
{
  /// <summary>
  /// The <see cref="TypeConverterType"/> enum describes whether a <see cref="TypeConverter"/> is to be used fro mthe source or the destination type.
  /// </summary>
  public enum TypeConverterType
  {
    SourceTypeConverter,
    DestinationTypeConverter
  }
}
