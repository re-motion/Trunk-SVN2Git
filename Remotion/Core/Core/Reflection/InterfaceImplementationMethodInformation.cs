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
  /// Represents a method that implements a method declared by an interface. <see cref="Invoke"/> and <see cref="GetFastInvoker"/> call the method
  /// via the interface.
  /// </summary>
  public class InterfaceImplementationMethodInformation : IMethodInformation
  {
    private readonly IMethodInformation _implementationMethodInfo;
    private readonly IMethodInformation _declarationMethodInfo;

    public InterfaceImplementationMethodInformation (IMethodInformation implementationMethodInfo, IMethodInformation declarationMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("implementationMethodInfo", implementationMethodInfo);
      ArgumentUtility.CheckNotNull ("declarationMethodInfo", declarationMethodInfo);

      _implementationMethodInfo = implementationMethodInfo;
      _declarationMethodInfo = declarationMethodInfo;
    }

    public string Name
    {
      get { return _implementationMethodInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _implementationMethodInfo.DeclaringType; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _implementationMethodInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.GetCustomAttribute<T>(inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.IsDefined<T> (inherited);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);
      
      return _implementationMethodInfo.FindInterfaceImplementation (implementationType);
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      return _declarationMethodInfo;
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T)(object)GetFastInvoker (typeof (T));
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("delegateType", delegateType);

      return _declarationMethodInfo.GetFastInvoker (delegateType);
    }

    public ParameterInfo[] GetParameters ()
    {
      return _implementationMethodInfo.GetParameters();
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      return _implementationMethodInfo.FindDeclaringProperty();
    }

    public Type ReturnType
    {
      get { return _implementationMethodInfo.ReturnType; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      
      return _declarationMethodInfo.Invoke (instance, parameters);
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration();
    }

    public override bool Equals (object obj)
    {
      // TODO Review 3351: Please rewrite using the standard pattern for classes that may derived from; see task description
      var other = obj as InterfaceImplementationMethodInformation;

      if (other == null)
        return false;

      return _implementationMethodInfo.Equals (other._implementationMethodInfo) && _declarationMethodInfo.Equals(other._declarationMethodInfo);
    }

    public override int GetHashCode ()
    {
      return _implementationMethodInfo.GetHashCode() ^ _declarationMethodInfo.GetHashCode();
    }

    public override string ToString ()
    {
      return string.Format ("{0} (impl of '{1}')", _implementationMethodInfo.Name, _declarationMethodInfo.DeclaringType.Name);
    }
  }
}