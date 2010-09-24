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
  /// Represents a property that implements a property declared by an interface. The accessors returned by <see cref="GetGetMethod(bool)"/> and 
  /// <see cref="GetSetMethod(bool)"/> will usually be instances <see cref="InterfaceImplementationMethodInformation"/>, but since a property can
  /// add accessors not declared by the interface property, they don't have to be.
  /// </summary>
  public class InterfaceImplementationPropertyInformation : IPropertyInformation
  {
    private readonly IPropertyInformation _implementationPropertyInfo;
    private readonly IPropertyInformation _declarationPropertyInfo;

    public InterfaceImplementationPropertyInformation (IPropertyInformation implementationPropertyInfo, IPropertyInformation declarationPropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("implementationPropertyInfo", implementationPropertyInfo);
      ArgumentUtility.CheckNotNull ("declarationPropertyInfo", declarationPropertyInfo);

      _implementationPropertyInfo = implementationPropertyInfo;
      _declarationPropertyInfo = declarationPropertyInfo;
    }

    public string Name
    {
      get { return _implementationPropertyInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _implementationPropertyInfo.DeclaringType; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _implementationPropertyInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _implementationPropertyInfo.GetCustomAttribute<T> (inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _implementationPropertyInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _implementationPropertyInfo.IsDefined<T> (inherited);
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _implementationPropertyInfo.FindInterfaceImplementation (implementationType);
    }

    public IPropertyInformation FindInterfaceDeclaration ()
    {
      return _declarationPropertyInfo;
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _implementationPropertyInfo.GetIndexParameters();
    }

    public Type PropertyType
    {
      get { return _implementationPropertyInfo.PropertyType; }
    }

    public bool CanBeSetFromOutside
    {
      get { return GetSetMethod (false) != null; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      // TODO Review 3334: This is not correct - it will not work if the implementation added the get accessor. Add a respective failing test, then use GetGetMethod().Invoke() instead.
      return _declarationPropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      // TODO Review 3334: This is not correct - it will not work if the implementation added the set accessor. Add a respective failing test, then use GetSetMethod().Invoke() instead.
      _declarationPropertyInfo.SetValue (instance, value, indexParameters);
    }

    /// <summary>
    /// Gets the get accessor for this <see cref="InterfaceImplementationPropertyInformation"/>. If the interface declaring this 
    /// <see cref="InterfaceImplementationPropertyInformation"/> also declares the accessor, an <see cref="InterfaceImplementationMethodInformation"/>
    /// for that accessor is returned. Otherwise, the <see cref="IMethodInformation"/> (if any) on the type implementing the interface is returned.
    /// </summary>
    /// <param name="nonPublic">Indicates whether a non-public accessor method may also be returned. If the interface declares the accessor, 
    /// this flag has no effect since interface methods are always public. If only the implementation type declares the accessor, the flag is used to
    /// determine whether to return that accessor.</param>
    /// <returns>
    /// An instance of <see cref="InterfaceImplementationMethodInformation"/> for the get method if the accessor is delared by the interface.
    /// Otherwise, an instance of <see cref="IMethodInformation"/> for the get method on the implementation type, or <see langword="null" /> if 
    /// there is no such method or its visibility does not match the <paramref name="nonPublic"/> flag.
    /// </returns>
    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      var interfaceAccessor = _declarationPropertyInfo.GetGetMethod (nonPublic);

      if (interfaceAccessor != null)
        return new InterfaceImplementationMethodInformation (_implementationPropertyInfo.GetGetMethod (true), interfaceAccessor);
      else
        return _implementationPropertyInfo.GetGetMethod (nonPublic);
    }

    /// <summary>
    /// Gets the set accessor for this <see cref="InterfaceImplementationPropertyInformation"/>. If the interface declaring this 
    /// <see cref="InterfaceImplementationPropertyInformation"/> also declares the accessor, an <see cref="InterfaceImplementationMethodInformation"/>
    /// for that accessor is returned. Otherwise, the <see cref="IMethodInformation"/> (if any) on the type implementing the interface is returned.
    /// </summary>
    /// <param name="nonPublic">Indicates whether a non-public accessor method may also be returned. If the interface declares the accessor, 
    /// this flag has no effect since interface methods are always public. If only the implementation type declares the accessor, the flag is used to
    /// determine whether to return that accessor.</param>
    /// <returns>
    /// An instance of <see cref="InterfaceImplementationMethodInformation"/> for the set method if the accessor is delared by the interface.
    /// Otherwise, an instance of <see cref="IMethodInformation"/> for the set method on the implementation type, or <see langword="null" /> if 
    /// there is no such method or its visibility does not match the <paramref name="nonPublic"/> flag.
    /// </returns>
    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      var interfaceAccessor = _declarationPropertyInfo.GetSetMethod (nonPublic);

      if (interfaceAccessor != null)
        return new InterfaceImplementationMethodInformation (_implementationPropertyInfo.GetSetMethod (true), interfaceAccessor);
      else
        return _implementationPropertyInfo.GetSetMethod (nonPublic);
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration();
    }

    public override string ToString ()
    {
      // TODO Review 3334: Use _implementationProperty.ToString(), add a blank before the opening parenthesis
      return string.Format ("{0}(impl of '{1}')", Name, _declarationPropertyInfo.DeclaringType.Name);
    }
  }
}