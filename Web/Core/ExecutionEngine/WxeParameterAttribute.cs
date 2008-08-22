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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
  public class WxeParameterAttribute : Attribute
  {
    public static WxeParameterAttribute GetAttribute (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      WxeParameterAttribute attribute = AttributeUtility.GetCustomAttribute<WxeParameterAttribute> (property, false);
      if (attribute == null)
        return null;

      if (!attribute._required.HasValue)
        attribute._required = property.PropertyType.IsValueType;
      return attribute;
    }

    private readonly int _index;
    private bool? _required;
    private readonly WxeParameterDirection _direction;

    public WxeParameterAttribute (int index, WxeParameterDirection direction)
        : this (index, null, direction)
    {
    }

    public WxeParameterAttribute (int index, bool required)
        : this (index, required, WxeParameterDirection.In )
    {
    }

    public WxeParameterAttribute (int index)
        : this (index, null, WxeParameterDirection.In)
    {
    }

    /// <summary>
    /// Declares a property as WXE function parameter.
    /// </summary>
    /// <param name="index"> Index of the parameter within the function's parameter list. </param>
    /// <param name="required"> Speficies whether this parameter must be specified (an not 
    ///     be <see langword="null"/>). Default is <see langword="true"/> for value types
    ///     and <see langword="false"/> for reference types. </param>
    /// <param name="direction"> Declares the parameter as input or output parameter, or both. </param>
    public WxeParameterAttribute (int index , bool required, WxeParameterDirection direction)
        : this (index, (bool?) required, direction)
    {
    }

    private WxeParameterAttribute (int index, bool? required, WxeParameterDirection direction)
    {
      _index = index;
      _required = required;
      _direction = direction;
    }

    public int Index
    {
      get { return _index; }
    }

    public bool Required
    {
      get { return _required.Value; }
    }

    public WxeParameterDirection Direction
    {
      get { return _direction; }
    }
  }
}