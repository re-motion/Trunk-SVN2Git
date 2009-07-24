// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.Collections;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Builds a proxy object which exposes only selected methods/properties, as decided by its <see cref="ITypeFilter"/>. 
  /// </summary>
  /// <remarks>
  /// What methods/properties are to be exposed is dependent on whether the method/property comes from a type which is
  /// classified as "valid" by the <see cref="ITypeFilter"/> of the class.
  /// <para/> 
  /// Used by <see cref="StableBindingProxyProvider"/>.
  /// <para/> 
  /// Uses <see cref="ForwardingProxyBuilder"/>.
  /// </remarks>
  public class StableBindingProxyBuilder
  {
    private readonly Type _proxiedType;
    private readonly ITypeFilter _typeFilter;
    private readonly ForwardingProxyBuilder _forwardingProxyBuilder;
    private readonly Dictionary<MethodInfo, HashSet<MethodInfo>> _classMethodToInterfaceMethodsMap = new Dictionary<MethodInfo, HashSet<MethodInfo>> ();
    private readonly ModuleScope _moduleScope;
    private readonly Type[] _knownInterfaces;
    private readonly MethodInfo[] _publicMethodsInFirstKnownBaseType;
    private readonly Type _firstKnownBaseType;
    private readonly StableMetadataTokenToMethodInfoMap _knownBaseTypeStableMetadataTokenToMethodInfoMap;

    public StableBindingProxyBuilder (Type proxiedType, ITypeFilter typeFilter, ModuleScope moduleScope)
    {
      ArgumentUtility.CheckNotNull ("proxiedType", proxiedType);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);
      _typeFilter = typeFilter;
      _moduleScope = moduleScope;
      _proxiedType = proxiedType;
      
      _knownInterfaces = FindKnownInterfaces();
      _forwardingProxyBuilder = new ForwardingProxyBuilder (_proxiedType.Name, _moduleScope, _proxiedType, _knownInterfaces);
      //_forwardingProxyBuilder = new ForwardingProxyBuilder (_proxiedType.Name, _moduleScope, _proxiedType, new Type[0]);

      BuildClassMethodToInterfaceMethodsMap();
      _firstKnownBaseType = GetFirstKnownBaseType();
      if (_firstKnownBaseType != null)
      {
        _publicMethodsInFirstKnownBaseType = _firstKnownBaseType.GetMethods().ToArray();
        _knownBaseTypeStableMetadataTokenToMethodInfoMap = new StableMetadataTokenToMethodInfoMap (_firstKnownBaseType);
      }
    }

 
    public Type ProxiedType
    {
      get { return _proxiedType; }
    }

    /// <summary>
    /// Builds the proxy <see cref="Type"/>.
    /// </summary>
    public Type BuildProxyType ()
    {
#if(false)
      var methodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      foreach (var proxiedTypeMethod in methodsInProxiedType)
      {
        if (IsMethodKnownInBaseType(proxiedTypeMethod))
        {
           _forwardingProxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (proxiedTypeMethod);
        }
        else
        {
          var interfaceMethodsToClassMethod = GetInterfaceMethodsToClassMethod (proxiedTypeMethod);
          foreach (var interfaceMethod in interfaceMethodsToClassMethod)
          {
            // Add forwarding interface implementations for methods whose target method info has not already been implemented.
            _forwardingProxyBuilder.AddForwardingExplicitInterfaceMethod (interfaceMethod);
          }
        }
      }
#else
      var methodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      foreach (var proxiedTypeMethod in methodsInProxiedType)
      {
        MethodInfo proxiedTypeMethodInKnownBaseType = null;
        if (_firstKnownBaseType != null)
        {
          proxiedTypeMethodInKnownBaseType = _knownBaseTypeStableMetadataTokenToMethodInfoMap.GetMethod (proxiedTypeMethod);
        }

        //if (IsMethodKnownInBaseType (proxiedTypeMethod) && 
        //  IsMethodBound (proxiedTypeMethod, _publicMethodsInFirstKnwonBaseType))
        if (proxiedTypeMethodInKnownBaseType != null && // method exists in first known base type
          IsMethodBound (proxiedTypeMethodInKnownBaseType, _publicMethodsInFirstKnownBaseType)) // method is visible in first known base type
        {
          //_forwardingProxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (proxiedTypeMethod);
          _forwardingProxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (proxiedTypeMethodInKnownBaseType);
        }
        else
        {
          var interfaceMethodsToClassMethod = GetInterfaceMethodsToClassMethod (proxiedTypeMethod);
          foreach (var interfaceMethod in interfaceMethodsToClassMethod)
          {
            // Add forwarding interface implementations for methods whose target method info has not already been implemented.
            _forwardingProxyBuilder.AddForwardingExplicitInterfaceMethod (interfaceMethod);
          }
        }
      }

#endif

      return _forwardingProxyBuilder.BuildProxyType ();
    }

    /// <summary>
    /// Returns <see langword="true"/> if the passed <paramref name="method"/> would be picked by C#
    /// out of the passed methods when calling a method with the <paramref name="method"/>|s name and parameter <see cref="Type"/>|s.
    /// </summary>
    public static bool IsMethodBound (MethodInfo method, MethodInfo[] candidateMethods)
    {
      var parameterTypes = method.GetParameters ().Select (pi => pi.ParameterType).ToArray ();
      //To.ConsoleLine.e (method.Name).nl ().e (candidateMethods).nl ().e (parameterTypes);

      // Note: SelectMethod needs the candidateMethods already to have been filtered by name, otherwise AmbiguousMatchException|s may occur.
      candidateMethods = candidateMethods.Where (mi => (mi.Name == method.Name)).ToArray ();

      // Binder.SelectMethod throws when candidateMethods are empty.
      if (candidateMethods.Length == 0)
      {
        return false;
      }

      var boundMethod = Type.DefaultBinder.SelectMethod (BindingFlags.Instance | BindingFlags.Public, 
        candidateMethods, parameterTypes, null);

      return Object.ReferenceEquals (method, boundMethod);
    }


    // TODO: If IsMethodEqualToBaseTypeMethod can be expressed as a CompoundValueEqualityComparer<MethodInfo>,
    // (MethodInfoFromRelatedTypesEqualityComparer) refactor back to initial implementation using HashSet, 
    // to get rid of quadratic runtime behavior.
    public bool IsMethodKnownInBaseType (MethodInfo method)
    {
      foreach (var baseTypeMethod in _publicMethodsInFirstKnownBaseType)
      {
        if (!baseTypeMethod.IsSpecialName)
        {
          if (IsMethodEqualToBaseTypeMethod (method, baseTypeMethod))
          {
            return true;
          }
        }
      }

      return false; 
    }

    public bool IsMethodEqualToBaseTypeMethod (MethodInfo method, MethodInfo baseTypeMethod)
    {
      #if(true)
        return method.GetBaseDefinition().MetadataToken == baseTypeMethod.GetBaseDefinition().MetadataToken;
      #else
        if (!method.GetBaseDefinition ().DeclaringType.IsAssignableFrom (baseTypeMethod.GetBaseDefinition ().DeclaringType))
        {
          return false;
        }
        else if(MethodInfoEqualityComparer.Get.Equals(method, baseTypeMethod))
        //else if (MethodInfoFromRelatedTypesEqualityComparer.Get.Equals (method, baseTypeMethod))
        {
          return true;
        }

        return false;
      #endif
    }

 


    //private HashSet<MethodInfo> CreateMethodsKnownInBaseTypeSet ()
    //{
    //  HashSet<MethodInfo> methodsKnownInBaseTypeSet = new HashSet<MethodInfo> (MethodInfoEqualityComparer.Get);
    //  var firstKnownBaseType = GetFirstKnownBaseType ();
    //  foreach (var method in firstKnownBaseType.GetMethods ())
    //  {
    //    methodsKnownInBaseTypeSet.Add (method.GetBaseDefinition ());
    //  }
    //  return methodsKnownInBaseTypeSet;
    //}


    ///// <summary>
    ///// Calls <see cref="BuildProxyType"/> and returns an instance of the generated proxy type proxying the passed <see cref="object"/>.
    ///// </summary>
    ///// <param name="proxied">The <see cref="object"/> to be proxied. Must be of the <see cref="Type"/> 
    ///// the <see cref="ForwardingProxyBuilder"/> was initialized with.</param>
    //public object CreateInstance (Object proxied)
    //{
    //  ArgumentUtility.CheckNotNullAndType ("proxied", proxied, _proxiedType);
    //  return Activator.CreateInstance (BuildProxyType (), proxied);
    //}


    private Type GetFirstKnownBaseType ()
    {
      return ProxiedType.CreateSequence (t => t.BaseType).FirstOrDefault (_typeFilter.IsTypeValid);
    }

    private Type[] FindKnownInterfaces ()
    {
      return ProxiedType.GetInterfaces ().Where (i => _typeFilter.IsTypeValid (i)).ToArray ();
    }

    private void BuildClassMethodToInterfaceMethodsMap ()
    {
      foreach (var knownInterface in _knownInterfaces)
      {
        var interfaceMapping = ProxiedType.GetInterfaceMap (knownInterface);
        var classMethods = interfaceMapping.TargetMethods;
        var interfaceMethods = interfaceMapping.InterfaceMethods;
        for (int i = 0; i < classMethods.Length; i++) 
        {
          AddTo_MethodToInterfaceMethodsMap (classMethods[i], interfaceMethods[i]);
        }
      }
    }

    private void AddTo_MethodToInterfaceMethodsMap (MethodInfo classMethod, MethodInfo interfaceMethod)
    {
      if (!_classMethodToInterfaceMethodsMap.ContainsKey (classMethod))
      {
        _classMethodToInterfaceMethodsMap[classMethod] = new HashSet<MethodInfo> ();
      }
      _classMethodToInterfaceMethodsMap[classMethod].Add (interfaceMethod);
    }

    private IEnumerable<MethodInfo> GetInterfaceMethodsToClassMethod (MethodInfo classMethod)
    {
      HashSet<MethodInfo> interfaceMethodsToClassMethod;
      _classMethodToInterfaceMethodsMap.TryGetValue (classMethod, out interfaceMethodsToClassMethod);
      return (IEnumerable<MethodInfo>) interfaceMethodsToClassMethod ?? new MethodInfo[0];
    }
  }

  /// <summary>
  /// Approximate equality for <see cref="MethodInfo"/>.
  /// </summary>
  public class MethodInfoApproximateEqualityComparer : CompoundValueEqualityComparer<MethodInfo>, IComparer<MethodInfo>
  {
    private static readonly MethodInfoApproximateEqualityComparer s_equalityComparer = new MethodInfoApproximateEqualityComparer ();

    public MethodInfoApproximateEqualityComparer (MethodAttributes methodAttributeMask)
      : base (
          x => new object[] {
                x.Name, x.ReturnType, x.Attributes & methodAttributeMask, 
                x.GetParameters().Length,
                x.IsGenericMethod ? x.GetGenericArguments().Length : 0
            })
    {
    }

    public MethodInfoApproximateEqualityComparer ()
      : this (~MethodAttributes.ReservedMask)
    {
    }


    public static MethodInfoApproximateEqualityComparer Get
    {
      get { return s_equalityComparer; }
    }


    public int Compare (MethodInfo x, MethodInfo y)
    {
      return Comparer<int>.Default.Compare (
          MethodInfoApproximateEqualityComparer.Get.GetHashCode (x), MethodInfoApproximateEqualityComparer.Get.GetHashCode (y));  
    }
  }
}
