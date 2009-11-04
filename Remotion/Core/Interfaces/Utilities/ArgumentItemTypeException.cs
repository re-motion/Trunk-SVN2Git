// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
  /// This exception is thrown if a list argument contains an item of the wrong type.
  /// </summary>
  [Serializable]
  public class ArgumentItemTypeException : ArgumentException
  {
    private readonly string _argumentName;
    private readonly int _index;
    private readonly Type _expectedType;
    private readonly Type _actualType;

    public ArgumentItemTypeException (string argumentName, int index, Type expectedType, Type actualType)
        : base ( FormatMessage (argumentName, index, expectedType, actualType))
    {
      _argumentName = argumentName;
      _index = index;
      _expectedType = expectedType;
      _actualType = actualType;
    }

    public ArgumentItemTypeException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _argumentName = info.GetString ("_argumentName");
      _index = info.GetInt32 ("_index");
      _expectedType = (Type) info.GetValue ("_expectedType", typeof (Type));
      _actualType = (Type) info.GetValue ("_actualType", typeof (Type));
    }

    public string ArgumentName
    {
      get { return _argumentName; }
    }

    public int Index
    {
      get { return _index; }
    }

    public Type ExpectedType
    {
      get { return _expectedType; }
    }

    public Type ActualType
    {
      get { return _actualType; }
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);

      info.AddValue ("_argumentName", _argumentName);
      info.AddValue ("_index", _index);
      info.AddValue ("_expectedType", _expectedType);
      info.AddValue ("_actualType", _actualType);
    }

    private static string FormatMessage (string argumentName, int index, Type expectedType, Type actualType)
    {
      return string.Format ("Item {0} of argument {1} has the type {2} instead of {3}.", index, argumentName, actualType.FullName, expectedType.FullName);
    }
  }
}
