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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class PropertyInfoAdapter : IPropertyInformation
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly bool _isExplicitInterfaceProperty;
    private readonly PropertyInfo _interfacePropertyInfo;

    public PropertyInfoAdapter (PropertyInfo propertyInfo, PropertyInfo interfacePropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      if (interfacePropertyInfo != null && !interfacePropertyInfo.DeclaringType.IsInterface)
        throw new ArgumentException ("Parameter must be a property declared on an interface.", "interfacePropertyInfo");

      _propertyInfo = propertyInfo;
      _interfacePropertyInfo = interfacePropertyInfo;
      _isExplicitInterfaceProperty = _interfacePropertyInfo != null && ReflectionUtility.GuessIsExplicitInterfaceProperty (propertyInfo);
    }

    public PropertyInfoAdapter (PropertyInfo propertyInfo)
      : this (propertyInfo, null)
    {
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    private PropertyInfo ValuePropertyInfo
    {
      get { return InterfacePropertyInfo ?? PropertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    public string Name
    {
      get { return _propertyInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _propertyInfo.DeclaringType; }
    }

    public PropertyInfo InterfacePropertyInfo
    {
      get { return _interfacePropertyInfo; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return ReflectionUtility.GetOriginalDeclaringType (_propertyInfo);
    }

    public bool CanBeSetFromOutside
    {
      get
      {
        if (_isExplicitInterfaceProperty)
          return _propertyInfo.GetSetMethod (true) != null; // for explicit interface properties, we allow for private setters
        else
          return _propertyInfo.GetSetMethod (false) != null; // for normal properties, we want a public setter
      }
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttribute<T> (_propertyInfo, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttributes<T> (_propertyInfo, inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T> (_propertyInfo, inherited);
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      return ValuePropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ValuePropertyInfo.SetValue (instance, value, indexParameters);
    }

    public override bool Equals (object obj)
    {
      var other = obj as PropertyInfoAdapter;
      return other != null && _propertyInfo.Equals (other._propertyInfo) && object.Equals (_interfacePropertyInfo, other._interfacePropertyInfo);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_propertyInfo, _interfacePropertyInfo);
    }
  }
}
