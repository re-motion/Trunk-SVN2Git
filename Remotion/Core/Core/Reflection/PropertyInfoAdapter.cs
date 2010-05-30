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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IPropertyInformation"/> interface to wrap a <see cref="PropertyInfo"/> instance.
  /// </summary>
  public class PropertyInfoAdapter : IPropertyInformation
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly PropertyInfo _interfacePropertyInfo;
    private Type _type;
    private readonly Func<object, object> _getter;
    private readonly Action<object, object> _setter;

    public PropertyInfoAdapter (PropertyInfo propertyInfo, PropertyInfo interfacePropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      if (interfacePropertyInfo != null && !interfacePropertyInfo.DeclaringType.IsInterface)
        throw new ArgumentException ("Parameter must be a property declared on an interface.", "interfacePropertyInfo");

      _propertyInfo = propertyInfo;
      _interfacePropertyInfo = interfacePropertyInfo;

      _getter = CreateGetterDelegate();
      _setter = CreateSetterDelegate();
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
      if (_type == null)
        _type = ReflectionUtility.GetOriginalDeclaringType (_propertyInfo);
      return _type;
    }

    public bool CanBeSetFromOutside
    {
      get { return ValuePropertyInfo.GetSetMethod (false) != null; }
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
      if (_getter != null)
        return _getter (instance);
      else
        return ValuePropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      if (_setter != null)
        _setter (instance, value);
      else
        ValuePropertyInfo.SetValue (instance, value, indexParameters);
    }

    public override bool Equals (object obj)
    {
      var other = obj as PropertyInfoAdapter;
      return other != null && _propertyInfo.Equals (other._propertyInfo) && Equals (_interfacePropertyInfo, other._interfacePropertyInfo);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_propertyInfo, _interfacePropertyInfo);
    }

    private Func<object, object> CreateGetterDelegate ()
    {
      var getMethod = ValuePropertyInfo.GetGetMethod (false);
      if (getMethod == null)
        return null;
      if (getMethod.GetParameters().Length > 0)
        return null;

      return (Func<object, object>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (getMethod, typeof (Func<object, object>));
    }

    private Action<object, object> CreateSetterDelegate ()
    {
      var setMethod = ValuePropertyInfo.GetSetMethod (false);
      if (setMethod == null)
        return null;
      if (setMethod.GetParameters().Length > 1)
        return null;

      return (Action<object, object>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (setMethod, typeof (Action<object, object>));
    }
  }
}