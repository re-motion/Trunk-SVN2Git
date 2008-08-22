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

namespace Remotion.Web.ExecutionEngine.Obsolete
{
  /// <summary>
  /// Specifies that a WXE function should automatically be created by the WXE function generator (wxegen.exe).
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class WxeFunctionPageAttribute : Attribute
  {
    private string _axpxPageName;
    private Type _baseClass;

    public WxeFunctionPageAttribute (string aspxPageName)
      : this (aspxPageName, typeof (WxeFunction))
    {
    }

    public WxeFunctionPageAttribute (string aspxPageName, Type baseClass)
    {
      _axpxPageName = aspxPageName;
      _baseClass = baseClass;
    }

    public string AspxPageName
    {
      get { return _axpxPageName; }
    }

    public Type BaseClass
    {
      get { return _baseClass; }
    }
  }

  /// <summary>
  /// Specifies a WXE function parameter that should automatically be created by the WXE function generator (wxegen.exe). 
  /// Requires <see cref="WxeFunctionPageAttribute"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class WxePageParameterAttribute: Attribute
  {
    private int _index;
    private string _name;
    private Type _type;
    private bool _required;
    private WxeParameterDirection _direction;

    public WxePageParameterAttribute (int index, string name, Type type, bool required, WxeParameterDirection direction)
    {
      _index = index;
      _name = name;
      _type = type;
      _required = required;
      _direction = direction;
    }

    public WxePageParameterAttribute (int index, string name, Type type)
      : this (index, name, type, false, WxeParameterDirection.In)
    {
    }

    public WxePageParameterAttribute (int index, string name, Type type, WxeParameterDirection direction)
      : this (index, name, type, false, direction)
    {
    }

    public WxePageParameterAttribute (int index, string name, Type type, bool required)
      : this (index, name, type, required, WxeParameterDirection.In)
    {
    }

    public int Index
    {
      get { return _index; }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool Required
    {
      get { return _required; }
    }

    public WxeParameterDirection Direction
    {
      get { return _direction; }
    }
  }
}
