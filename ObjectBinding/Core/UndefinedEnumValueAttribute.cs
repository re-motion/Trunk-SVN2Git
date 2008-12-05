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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Indicates an value of an enum that should be the undefined value.
  /// </summary>
  /// <remarks>
  ///   Use this Attribute if you need to have one value of an enum that represents the undefined value.
  ///   This value is then mapped the undefined value for displaying in Business Object Controls controls.
  /// </remarks>
  [AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
  public sealed class UndefinedEnumValueAttribute : Attribute
  {
    private readonly Enum _value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value">The enum value that should be the undefined value. Must not be <see langword="null"/>.</param>
    public UndefinedEnumValueAttribute (object value)
    {
      ArgumentUtility.CheckNotNullAndType<Enum> ("value", value);
      ArgumentUtility.CheckValidEnumValue ("value", (Enum) value);

      _value = (Enum) value;
    }

    /// <summary>
    /// Gets the undefined value of the enum.
    /// </summary>
    public Enum Value
    {
      get { return _value; }
    }
  }
}
