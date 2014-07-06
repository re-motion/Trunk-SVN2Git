// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IPropertyInformation"/> interface to wrap a <see cref="PropertyInfo"/> instance.
  /// </summary>
  [TypeConverter (typeof (PropertyInfoAdapterConverter))]
  public sealed class PropertyInfoAdapter : IPropertyInformation
  {
    //If this is changed to an (expiring) cache, equals implementation must be updated.
    private static readonly IDataStore<PropertyInfo, PropertyInfoAdapter> s_dataStore =
        new LockingDataStoreDecorator<PropertyInfo, PropertyInfoAdapter> (
            new SimpleDataStore<PropertyInfo, PropertyInfoAdapter> (MemberInfoEqualityComparer<PropertyInfo>.Instance));

    private static readonly Func<PropertyInfo, PropertyInfoAdapter> s_ctorFunc = pi => new PropertyInfoAdapter (pi); 

    public static PropertyInfoAdapter Create (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      return s_dataStore.GetOrCreateValue (propertyInfo, s_ctorFunc);
    }

    private readonly PropertyInfo _propertyInfo;
    private readonly Lazy<IMethodInformation> _publicGetMethod;
    private readonly Lazy<IMethodInformation> _publicOrNonPublicGetMethod;
    private readonly Lazy<IMethodInformation> _publicSetMethod;
    private readonly Lazy<IMethodInformation> _publicOrNonPublicSetMethod;

    private readonly Lazy<ITypeInformation> _cachedOriginalDeclaringType;
    private readonly Lazy<IPropertyInformation> _cachedOriginalDeclaration;

    private PropertyInfoAdapter (PropertyInfo propertyInfo)
    {
      _propertyInfo = propertyInfo;

      _publicGetMethod =
          new Lazy<IMethodInformation> (() => Maybe.ForValue (_propertyInfo.GetGetMethod (false)).Select (MethodInfoAdapter.Create).ValueOrDefault());
      _publicOrNonPublicGetMethod =
          new Lazy<IMethodInformation> (() => Maybe.ForValue (_propertyInfo.GetGetMethod (true)).Select (MethodInfoAdapter.Create).ValueOrDefault());
      _publicSetMethod =
          new Lazy<IMethodInformation> (() => Maybe.ForValue (_propertyInfo.GetSetMethod (false)).Select (MethodInfoAdapter.Create).ValueOrDefault());
      _publicOrNonPublicSetMethod =
          new Lazy<IMethodInformation> (() => Maybe.ForValue (_propertyInfo.GetSetMethod (true)).Select (MethodInfoAdapter.Create).ValueOrDefault());

      _cachedOriginalDeclaringType = new Lazy<ITypeInformation> (
          () => TypeAdapter.Create (_propertyInfo.GetOriginalDeclaringType()),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _cachedOriginalDeclaration = new Lazy<IPropertyInformation> (
          () => PropertyInfoAdapter.Create (_propertyInfo.GetBaseDefinition()),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    public string Name
    {
      get { return _propertyInfo.Name; }
    }

    public ITypeInformation DeclaringType
    {
      get { return Maybe.ForValue (_propertyInfo.DeclaringType).Select (TypeAdapter.Create).ValueOrDefault (); }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _cachedOriginalDeclaringType.Value;
    }

    public IPropertyInformation GetOriginalDeclaration ()
    {
      return _cachedOriginalDeclaration.Value;
    }

    public bool CanBeSetFromOutside
    {
      get { return PropertyInfo.GetSetMethod (false) != null; }
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
      ArgumentUtility.CheckNotNull ("instance", instance);

      return PropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      PropertyInfo.SetValue (instance, value, indexParameters);
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      if (nonPublic)
        return _publicOrNonPublicGetMethod.Value;
      else
        return _publicGetMethod.Value;
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      if (nonPublic)
        return _publicOrNonPublicSetMethod.Value;
      else
        return _publicSetMethod.Value;
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _propertyInfo.GetIndexParameters();
    }

    public IMethodInformation[] GetAccessors (bool nonPublic)
    {
      return Array.ConvertAll (_propertyInfo.GetAccessors (nonPublic), mi => (IMethodInformation) MethodInfoAdapter.Create(mi));
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      if (!DeclaringType.IsInterface)
        throw new InvalidOperationException ("This property is not an interface property.");

      var interfaceAccessorMethod = GetGetMethod (false) ?? GetSetMethod (false);
      var implementationMethod = interfaceAccessorMethod.FindInterfaceImplementation (implementationType);
      if (implementationMethod == null)
        return null;
      
      var implementationProperty = implementationMethod.FindDeclaringProperty ();
      
      Assertion.IsNotNull (
          implementationProperty, 
          "We assume that property acessor '" + implementationMethod + "' must be found on '" + implementationType + "'.");

      return implementationProperty;
    }

    public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
    {
      if (DeclaringType.IsInterface)
        throw new InvalidOperationException ("This property is itself an interface member, so it cannot have an interface declaration.");

      var accessorMethod = GetGetMethod (true) ?? GetSetMethod (true);
      var interfaceAccessorMethods = accessorMethod.FindInterfaceDeclarations ();
      return interfaceAccessorMethods.Select (m => m.FindDeclaringProperty());
    }

    public override bool Equals (object obj)
    {
      return ReferenceEquals (this, obj);
    }

    public override int GetHashCode ()
    {
      return base.GetHashCode();
    }

    public override string ToString ()
    {
      return _propertyInfo.ToString ();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}