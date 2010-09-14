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
  public class MixinIntroducedMethodInformation : IMethodInformation
  {
    private readonly IMethodInformation _mixinMethodInfo;
    private DoubleCheckedLockingContainer<IMethodInformation> methodInterfaceDeclarationCache;

    public MixinIntroducedMethodInformation (IMethodInformation mixinMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("mixinMethodInfo", mixinMethodInfo);

      _mixinMethodInfo = mixinMethodInfo;
    }

    public string Name
    {
      get { return _mixinMethodInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _mixinMethodInfo.DeclaringType; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _mixinMethodInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.GetCustomAttribute<T> (inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.IsDefined<T> (inherited);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      return _mixinMethodInfo.FindInterfaceImplementation (implementationType);
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      return _mixinMethodInfo.FindInterfaceDeclaration();
    }

    public T GetFastInvoker<T> () where T: class
    {
      if (!typeof (T).IsSubclassOf (typeof (Delegate)))
        throw new InvalidOperationException (typeof (T).Name + " is not a delegate type.");

      return GetFastInvoker (typeof (T)) as T;
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      var methodInterfaceDeclaration = FindInterfaceDeclaration();

      return methodInterfaceDeclaration.GetFastInvoker (delegateType);
    }

    public IPropertyInformation FindDeclaringProperty (Type implementationType)
    {
      return _mixinMethodInfo.FindDeclaringProperty (implementationType);
    }

    public Type ReturnType
    {
      get { return _mixinMethodInfo.ReturnType; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      if (methodInterfaceDeclarationCache == null)
        methodInterfaceDeclarationCache = new DoubleCheckedLockingContainer<IMethodInformation>(FindInterfaceDeclaration);

      try
      {
        return methodInterfaceDeclarationCache.Value.Invoke (instance, parameters);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration();
    }
  }
}