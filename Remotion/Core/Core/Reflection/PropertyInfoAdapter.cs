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
using System.Linq;
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
    private Type _type;
    
    public PropertyInfoAdapter (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _propertyInfo = propertyInfo;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    private PropertyInfo ValuePropertyInfo
    {
      get { return PropertyInfo; }
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
      try
      {
        return ValuePropertyInfo.GetValue (instance, indexParameters);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    public void SetValue (object instance, object value, object[] indexParameters)
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

    public IMethodInformation GetGetMethod ()
    {
      return new MethodInfoAdapter (_propertyInfo.GetGetMethod());
    }

    public IMethodInformation GetSetMethod ()
    {
      return new MethodInfoAdapter (_propertyInfo.GetSetMethod());
    }

    public override bool Equals (object obj)
    {
      var other = obj as PropertyInfoAdapter;
      return other != null && _propertyInfo.Equals (other._propertyInfo);
    }

    public override int GetHashCode ()
    {
      return _propertyInfo.GetHashCode();
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      if (!DeclaringType.IsInterface)
        throw new InvalidOperationException ("This property is not an interface property.");

      if (implementationType.IsInterface)
        throw new ArgumentException ("The implementationType parameter must not be an interface.", "implementationType");

      if (!DeclaringType.IsAssignableFrom (implementationType))
        return null;
      
      var interfaceMap = implementationType.GetInterfaceMap (DeclaringType);
      var interfaceAccessorMethod = PropertyInfo.GetGetMethod (false) ?? PropertyInfo.GetSetMethod (false);

      var accessorIndex = interfaceMap.InterfaceMethods
          .Select ((m, i) => new { Method = m, Index = i })
          .Single (tuple => tuple.Method == interfaceAccessorMethod)
          .Index;
      var implementationMethod = interfaceMap.TargetMethods[accessorIndex];

      var implementationProperty = implementationType
          .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Single (pi => (pi.GetGetMethod (true) ?? pi.GetSetMethod (true)) == implementationMethod);
      return new PropertyInfoAdapter(implementationProperty);
    }
  }
}