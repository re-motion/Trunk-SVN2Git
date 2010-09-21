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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
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
      get { return _implementationPropertyInfo.CanBeSetFromOutside; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _declarationPropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      _declarationPropertyInfo.SetValue (instance, value, indexParameters);
    }

    public InterfaceImplementationMethodInformation GetGetMethod (bool nonPublic)
    {
      return
          Maybe.ForValue (_implementationPropertyInfo.GetGetMethod (nonPublic)).Select (
              mi => new InterfaceImplementationMethodInformation (mi, _declarationPropertyInfo.GetGetMethod (nonPublic))).ValueOrDefault ();
    }

    public InterfaceImplementationMethodInformation GetSetMethod (bool nonPublic)
    {
      return
          Maybe.ForValue (_implementationPropertyInfo.GetSetMethod (nonPublic)).Select (
              mi => new InterfaceImplementationMethodInformation (mi, _declarationPropertyInfo.GetSetMethod (nonPublic))).ValueOrDefault();
    }

    IMethodInformation IPropertyInformation.GetGetMethod (bool nonPublic)
    {
      return GetGetMethod (nonPublic);
    }

    IMethodInformation IPropertyInformation.GetSetMethod (bool nonPublic)
    {
      return GetSetMethod (nonPublic);
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
      return string.Format ("{0}(impl of '{1}'", Name, _declarationPropertyInfo.DeclaringType.Name);
    }
  }
}