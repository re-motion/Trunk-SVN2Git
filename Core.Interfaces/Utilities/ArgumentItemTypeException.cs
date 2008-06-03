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
using System.Runtime.Serialization;

namespace Remotion.Utilities
{
  /// <summary>
  /// This exception is thrown if a list argument contains an item of the wrong type.
  /// </summary>
  [Serializable]
  public class ArgumentItemTypeException : ArgumentException
  {
    public ArgumentItemTypeException (string argumentName, int index, Type expectedType, Type actualType)
        : base ( FormatMessage (argumentName, index, expectedType, actualType))
    {
    }

    public ArgumentItemTypeException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    private static string FormatMessage (string argumentName, int index, Type expectedType, Type actualType)
    {
      return string.Format ("Item {0} of argument {1} has the type {2} instead of {3}.", index, argumentName, actualType.FullName, expectedType.FullName);
    }
  }
}
