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
  /// This exception is thrown if an argument has an invalid type.
  /// </summary>
  [Serializable]
  public class ArgumentTypeException : ArgumentException
  {
    public readonly Type ExpectedType;
    public readonly Type ActualType;

    public ArgumentTypeException (string message, string argumentName, Type expectedType, Type actualType)
        : base (message, argumentName)
    {
      ExpectedType = expectedType;
      ActualType = actualType;
    }

    public ArgumentTypeException (string argumentName, Type expectedType, Type actualType)
        : this (FormatMessage (argumentName, expectedType, actualType), argumentName, actualType, expectedType)
    {
    }

    public ArgumentTypeException (string argumentName, Type actualType)
        : this (argumentName, null, actualType)
    {
    }

    public ArgumentTypeException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      ExpectedType = (Type) info.GetValue ("ExpectedType", typeof (Type));
      ActualType = (Type) info.GetValue ("ActualType", typeof (Type));
    }

    private static string FormatMessage (string argumentName, Type expectedType, Type actualType)
    {
      string actualTypeName = actualType != null ? actualType.ToString() : "<null>";
      if (expectedType == null)
        return string.Format ("Argument {0} has unexpected type {1}", argumentName, actualTypeName);
      else
        return string.Format ("Argument {0} has type {2} when type {1} was expected.", argumentName, expectedType, actualTypeName);
    }
  }
}
