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
  /// Represents a mixin method that is introduced to its target classes.
  /// </summary>
  /// <remarks>
  /// This is mainly just a wrapper around a <see cref="IMethodInformation"/>
  /// describing the method as it is implemented on the mixin type itself. The <see cref="Invoke"/> and <see cref="GetFastInvoker"/> methods, however,
  /// always call the method via its interface declaration. (Every member introduced by a mixin is declared by an interface implemented by the mixin.)
  /// This means that a <see cref="MixinIntroducedMethodInformation"/> can invoke the method both via the mixin instance and via an instance of the
  /// target class.
  /// </remarks>
  /// <seealso cref="MixinIntroducedPropertyInformation"/>
  public class MixinIntroducedMethodInformation : IMethodInformation
  {
    private readonly IMethodInformation _mixinMethodInfo;
    private readonly DoubleCheckedLockingContainer<IMethodInformation> _methodInterfaceDeclarationCache;

    public MixinIntroducedMethodInformation (IMethodInformation mixinMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("mixinMethodInfo", mixinMethodInfo);

      _mixinMethodInfo = mixinMethodInfo;
      _methodInterfaceDeclarationCache = new DoubleCheckedLockingContainer<IMethodInformation> (FindInterfaceDeclaration);
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
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _mixinMethodInfo.FindInterfaceImplementation (implementationType);
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      return _mixinMethodInfo.FindInterfaceDeclaration();
    }

    public T GetFastInvoker<T> () where T: class
    {
      // TODO Review 3285: This check can probably be removed; the called GetFastInvoker performs it anyway. (If you want to keep it, change it to throw an ArgumentException instead.)
      if (!typeof (T).IsSubclassOf (typeof (Delegate)))
        throw new InvalidOperationException ("'" + typeof (T) + "' is not a delegate type.");

      return GetFastInvoker (typeof (T)) as T; // TODO Review 3285: Use an ordinary cast
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var methodInterfaceDeclaration = FindInterfaceDeclaration();
      return methodInterfaceDeclaration.GetFastInvoker (delegateType);
    }

    public ParameterInfo[] GetParameters ()
    {
      return _mixinMethodInfo.GetParameters();
    }

    public IPropertyInformation FindDeclaringProperty (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _mixinMethodInfo.FindDeclaringProperty (implementationType);
    }

    public Type ReturnType
    {
      get { return _mixinMethodInfo.ReturnType; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      // TODO Review 3279: Do not catch the TargetInvocationException here, the inner IMethodInformation is responsible for this. Add a test showing that if the inner Invoke throws a TargetInvocationException, that exception is bubbled to the outside.
      try
      {
        // TODO Review 3282: Move the cache access to FindInterfaceDeclaration (change the cache to go to _mixinMethodInfo.FindInterfaceDeclaration() to avoid an infinite loop); use FindInterfaceDeclaration here.
        return _methodInterfaceDeclarationCache.Value.Invoke (instance, parameters);
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