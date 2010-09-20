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
  /// Null-object implementation of <see cref="IMethodInformation"/>.
  /// </summary>
  public class NullMethodInformation : IMethodInformation
  {
    public string Name
    {
      get { return null; }
    }

    public Type DeclaringType
    {
      get { return null; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return null;
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return null;
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return new T[] { };
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return false;
    }

    public Type ReturnType
    {
      get { return null; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      return null;
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      throw new InvalidOperationException();
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      return null;
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      return null;
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T)(object)GetFastInvoker (typeof (T));
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof(Delegate));
      
      var methodInfo = typeof (NullMethodInformation).GetMethod ("GetNull", BindingFlags.Static | BindingFlags.NonPublic);
      return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (methodInfo, delegateType);
    }

    public ParameterInfo[] GetParameters ()
    {
      return new ParameterInfo[0];
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
      if (obj == null)
        return false;
      return obj.GetType() == GetType();
    }

    public override int GetHashCode ()
    {
      return 0;
    }

    private static object GetNull ()
    {
      return null;
    }

    public override string ToString ()
    {
      return "NullMethodInformation";
    }
  }
}