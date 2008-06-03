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
  /// This exception is thrown if an argument is empty although it must have a content.
  /// </summary>
  [Serializable]
  public class ArgumentEmptyException : ArgumentException
  {
    public ArgumentEmptyException (string argumentName)
        : base (FormatMessage (argumentName))
    {
    }

    public ArgumentEmptyException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    private static string FormatMessage (string argumentName)
    {
      return string.Format ("Argument {0} is empty.", argumentName);
    }
  }
}
