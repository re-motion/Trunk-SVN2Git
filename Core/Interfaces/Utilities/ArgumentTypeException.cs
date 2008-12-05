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
