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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IMethodInformation"/> to wrap a <see cref="MethodInfo"/> instance.
  /// </summary>
  [TypeConverter (typeof (MethodInfoAdapterConverter))]
  public sealed class MethodInfoAdapter : IMethodInformation
  {
    //If this is changed to an (expiring) cache, equals implementation must be updated.
    private static readonly IDataStore<MethodInfo, MethodInfoAdapter> s_dataStore = new InterlockedDataStore<MethodInfo, MethodInfoAdapter> ();

    public static MethodInfoAdapter Create (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      return s_dataStore.GetOrCreateValue (methodInfo, mi=> new MethodInfoAdapter (mi));
    }

    private readonly MethodInfo _methodInfo;
    private readonly DoubleCheckedLockingContainer<Type> _cachedOriginalDeclaringType;

    private MethodInfoAdapter (MethodInfo methodInfo)
    {
      _methodInfo = methodInfo;
      _cachedOriginalDeclaringType = new DoubleCheckedLockingContainer<Type> (() => ReflectionUtility.GetOriginalDeclaringType (_methodInfo));
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
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _methodInfo.Invoke (instance, parameters);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      if (!DeclaringType.IsInterface)
        throw new InvalidOperationException ("This method is not an interface method.");

      if (implementationType.IsInterface)
        throw new ArgumentException ("The implementationType parameter must not be an interface.", "implementationType");

      if (!DeclaringType.IsAssignableFrom (implementationType))
        return null;

      var interfaceMap = implementationType.GetInterfaceMap (DeclaringType);
      var methodIndex = interfaceMap.InterfaceMethods
          .Select ((m, i) => new { InterfaceMethod = m, Index = i })
          .First (tuple => tuple.InterfaceMethod.Equals (_methodInfo)) // actually Single, but we can stop at the first matching method
          .Index;
      return Create(interfaceMap.TargetMethods[methodIndex]);
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      if (DeclaringType.IsInterface)
        throw new InvalidOperationException ("This method is itself an interface member, so it cannot have an interface declaration.");

      var resultMethodInfo =
          (from ifc in DeclaringType.GetInterfaces()
           let map = DeclaringType.GetInterfaceMap (ifc)
           from index in Enumerable.Range (0, map.TargetMethods.Length)
           where MemberInfoEqualityComparer.Instance.Equals(map.TargetMethods[index], _methodInfo)
           select map.InterfaceMethods[index]).FirstOrDefault();
      return Maybe.ForValue (resultMethodInfo).Select (Create).ValueOrDefault();
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T) (object) GetFastInvoker (typeof (T));
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (_methodInfo, delegateType);
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      // Note: We scan the hierarchy ourselves because private (eg., explicit) property implementations in base types are ignored by GetProperties
      // We use AreEqualMethodsWithoutReflectedType because our algorithm manually iterates over the base type hierarchy, so the accesor's
      // ReflectedType will be the declaring type, whereas _methodInfo might have a different ReflectedType.
      // AreEqualMethodsWithoutReflectedType can't deal with closed generic methods, but property accessors aren't generic anyway.

      var propertyInfo = 
          (from t in DeclaringType.CreateSequence (t => t.BaseType)
           from pi in t.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
           from accessor in new[] { pi.GetGetMethod (true), pi.GetSetMethod (true) }
           where accessor != null && MemberInfoEqualityComparer.Instance.Equals (_methodInfo, accessor)
           select pi).FirstOrDefault();
      return propertyInfo != null ? PropertyInfoAdapter.Create(propertyInfo) : null;
    }

    public ParameterInfo[] GetParameters ()
    {
      return _methodInfo.GetParameters();
    }

    public IMethodInformation GetBaseDefinition ()
    {
      return Create (_methodInfo.GetBaseDefinition());
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration ();
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T> (_methodInfo, inherited);
    }

    public override bool Equals (object obj)
    {
      return ReferenceEquals (this, obj);
    }

    public override int GetHashCode ()
    {
      return MemberInfoEqualityComparer.Instance.GetHashCode (_methodInfo);
    }

    public override string ToString ()
    {
      return _methodInfo.ToString ();
    }
    
  }
}