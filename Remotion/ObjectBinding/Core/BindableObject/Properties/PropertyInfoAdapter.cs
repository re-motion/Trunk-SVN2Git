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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  /// <summary>
  /// Implements the <see cref="IPropertyInformation"/> interface to wrap a <see cref="PropertyInfo"/> instance.
  /// </summary>
  public class PropertyInfoAdapter : IPropertyInformation
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly PropertyInfo _interfacePropertyInfo;
    private Type _type;
    private readonly Delegate _getter;
    private readonly Delegate _setter;

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
      if (_getter is Func<object, object>)
      {
        return ((Func<object, object>) _getter) (instance);
      }
      else if (_getter is Func<object, object, object>)
      {
        if (indexParameters == null || indexParameters.Length != 1)
          throw new TargetParameterCountException ("Parameter count mismatch.");
        return ((Func<object, object, object>) _getter) (instance, indexParameters[0]);
      }
      else if (_getter is Func<object, object, object, object>)
      {
        if (indexParameters == null || indexParameters.Length != 2)
          throw new TargetParameterCountException ("Parameter count mismatch.");
        return ((Func<object, object, object, object>) _getter) (instance, indexParameters[0], indexParameters[1]);
      }
      else
      {
        try
        {
          return ValuePropertyInfo.GetValue (instance, indexParameters);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      if (_setter is Action<object, object>)
      {
        ((Action<object, object>) _setter) (instance, value);
      }
      else if (_setter is Action<object, object, object>)
      {
        if (indexParameters == null || indexParameters.Length != 1)
          throw new TargetParameterCountException ("Parameter count mismatch.");
        ((Action<object, object, object>) _setter) (instance, indexParameters[0], value);
      }
      else if (_setter is Action<object, object, object, object>)
      {
        if (indexParameters == null || indexParameters.Length != 2)
          throw new TargetParameterCountException ("Parameter count mismatch.");
        ((Action<object, object, object, object>) _setter) (instance, indexParameters[0], indexParameters[1], value);
      }
      else
      {
        try
        {
          ValuePropertyInfo.SetValue (instance, value, indexParameters);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
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

    private Delegate CreateGetterDelegate ()
    {
      var getMethod = ValuePropertyInfo.GetGetMethod (true);
      if (getMethod == null)
        return null;

      switch (getMethod.GetParameters().Length)
      {
        case 0:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (getMethod, typeof (Func<object, object>));
        case 1:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (getMethod, typeof (Func<object, object, object>));
        case 2:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (getMethod, typeof (Func<object, object, object, object>));
        default:
          return null;
      }
    }

    private Delegate CreateSetterDelegate ()
    {
      var setMethod = ValuePropertyInfo.GetSetMethod (true);
      if (setMethod == null)
        return null;

      switch (setMethod.GetParameters().Length)
      {
        case 1:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (setMethod, typeof (Action<object, object>));
        case 2:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (setMethod, typeof (Action<object, object, object>));
        case 3:
          return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (setMethod, typeof (Action<object, object, object, object>));
        default:
          return null;
      }
    }
  }
}