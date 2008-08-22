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
    public static IEnumerable<IPropertyInformation> AdaptCollection (IEnumerable<PropertyInfo> infos)
    {
      foreach (PropertyInfo info in infos)
        yield return new PropertyInfoAdapter (info, info);
    }

    public static IEnumerable<PropertyInfo> UnwrapCollection (IEnumerable<IPropertyInformation> adapters)
    {
      foreach (PropertyInfoAdapter adapter in adapters)
        yield return adapter.PropertyInfo;
    }

    private readonly PropertyInfo _propertyInfo;
    private readonly bool _isExplicitInterfaceProperty;
    private readonly PropertyInfo _valuePropertyInfo;

    public PropertyInfoAdapter (PropertyInfo propertyInfo, PropertyInfo valuePropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
      _valuePropertyInfo = valuePropertyInfo;
      _isExplicitInterfaceProperty = ReflectionUtility.GuessIsExplicitInterfaceProperty (propertyInfo);
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public PropertyInfo ValuePropertyInfo
    {
      get { return _valuePropertyInfo; }
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
      return _valuePropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      _valuePropertyInfo.SetValue (instance, value, indexParameters);
    }

    public override bool Equals (object obj)
    {
      var other = obj as PropertyInfoAdapter;
      return other != null && _propertyInfo.Equals (other._propertyInfo) && _valuePropertyInfo.Equals (other._valuePropertyInfo);
    }

    public override int GetHashCode ()
    {
      return _propertyInfo.GetHashCode () ^ _valuePropertyInfo.GetHashCode();
    }
  }
}
