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
