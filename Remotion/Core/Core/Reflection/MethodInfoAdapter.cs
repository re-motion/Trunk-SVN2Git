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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IMethodInformation"/> to wrap a <see cref="MethodInfo"/> instance.
  /// </summary>
  public class MethodInfoAdapter : IMethodInformation
  {
    private readonly MethodInfo _methodInfo;
    private DoubleCheckedLockingContainer<Type> _cachedOriginalDeclaringType;
    
    public MethodInfoAdapter (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      _methodInfo = methodInfo;
    }

    public MethodInfo MethodInfo
    {
      get { return _methodInfo; }
    }

    public Type ReturnType
    {
      get { return _methodInfo.ReturnType; }
    }

    public string Name
    {
      get { return _methodInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _methodInfo.DeclaringType; } 
    }

    public Type GetOriginalDeclaringType ()
    {
      if (_cachedOriginalDeclaringType == null)
        _cachedOriginalDeclaringType = new DoubleCheckedLockingContainer<Type>(() => ReflectionUtility.GetOriginalDeclaringType (_methodInfo));
      return _cachedOriginalDeclaringType.Value;
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttribute<T> (_methodInfo, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttributes<T> (_methodInfo, inherited);
    }

    public object Invoke (object instance, object[] parameters)
    {
      try
      {
        return _methodInfo.Invoke (instance, parameters);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      if (!DeclaringType.IsInterface)
        throw new InvalidOperationException ("This method is not an interface method.");

      if (implementationType.IsInterface)
        throw new ArgumentException ("The implementationType parameter must not be an interface.", "implementationType");

      if (!DeclaringType.IsAssignableFrom (implementationType))
        return null;

      var interfaceMap = implementationType.GetInterfaceMap (DeclaringType);
      var methodIndex = interfaceMap.InterfaceMethods
          .Select ((m, i) => new { Method = m, Index = i })
          .Single (tuple => tuple.Method == _methodInfo)
          .Index;
      return new MethodInfoAdapter(interfaceMap.TargetMethods[methodIndex]);
    }

    public PropertyInfo FindDeclaringProperty (Type implementationType)
    {
      // Note: We scan the hierarchy ourselves because private (eg. explicit) property implementations in base types are ignored by GetProperties
      return implementationType.CreateSequence (t => t.BaseType)
            .SelectMany (t => t.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .SingleOrDefault (pi => IsAccessorMatch (_methodInfo, (pi.GetGetMethod (true) ?? pi.GetSetMethod (true))));
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T> (_methodInfo, inherited);
    }

    public override bool Equals (object obj)
    {
      var other = obj as MethodInfoAdapter;

      return other != null && _methodInfo.Equals (other._methodInfo);
    }

    public override int GetHashCode ()
    {
      return _methodInfo.GetHashCode();
    }

    private bool IsAccessorMatch (MethodInfo accessor1, MethodInfo accessor2)
    {
      // Equals won't work here because our algorithm manually iterates over the base type hierarchy, so accessor2.ReflectedType will be the exact
      // declaring type whereas GetInterfaceMap gets all the accessors from the original type, so accessor1.ReflectedType will be the original type.
      // Therefore, we compare declaring type and metadata token, which is unique per method overload.
      return accessor1.DeclaringType == accessor2.DeclaringType && accessor1.MetadataToken == accessor2.MetadataToken;
    }
  }
}