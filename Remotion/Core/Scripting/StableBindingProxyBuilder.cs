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
      BuildClassMethodToInterfaceMethodsMap ();

      _forwardingProxyBuilder = new ForwardingProxyBuilder (_proxiedType.Name, _moduleScope, _proxiedType, _knownInterfaces);
      //_forwardingProxyBuilder = new ForwardingProxyBuilder (_proxiedType.Name, _moduleScope, _proxiedType, new Type[0]);

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
    /// Builds the proxy <see cref="Type"/> which exposes all known methods and properties and forwards calls to the proxied <see cref="Type"/>.
    /// </summary>
    public Type BuildProxyType ()
    {
      ImplementKnownMethods ();
      ImplementKnownProperties ();
      return _forwardingProxyBuilder.BuildProxyType ();
    }

    private void ImplementKnownProperties ()
    {
      //throw new NotImplementedException();
      var specialMethodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (mi => mi.IsSpecialName);

      var implementedProperties = new HashSet<PropertyInfo>();

      // TODO: Turn into field, move initialization into ctor
      MethodInfo[] methodsInFirstKnownBaseType = new MethodInfo[0];
      if (_firstKnownBaseType != null)
      {
        methodsInFirstKnownBaseType = _firstKnownBaseType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      }

      // TODO: Turn into field, move initialization into ctor
      //Dictionary<StableMetadataToken,HashSet<PropertyInfo>> firstKnownBaseTypeSpecialMethodsToPropertyMap = null;
      Dictionary<StableMetadataToken, PropertyInfo> firstKnownBaseTypeSpecialMethodsToPropertyMap = null;

      if (_firstKnownBaseType != null)
      {
        firstKnownBaseTypeSpecialMethodsToPropertyMap = BuildSpecialMethodsToPropertyMap(_firstKnownBaseType);
        
        //To.ConsoleLine.e ("firstKnownBaseTypeSpecialMethodsToPropertyMap(multiple entries)", 
        //  firstKnownBaseTypeSpecialMethodsToPropertyMap.Where(x => x.Value.Count > 1).Select(y => new object[] { y.Key, y.Value.Select(z => z.Name).ToArray() }));

        To.ConsoleLine.e ("firstKnownBaseTypeSpecialMethodsToPropertyMap", firstKnownBaseTypeSpecialMethodsToPropertyMap.Keys);
        //To.ConsoleLine.e ("firstKnownBaseTypeSpecialMethodsToPropertyMap", firstKnownBaseTypeSpecialMethodsToPropertyMap).nl (2).e (proxiedTypeSpecialMethodsToPropertyMap);
      }

      // TODO: Turn into field, move initialization into ctor
      //var proxiedTypeSpecialMethodsToPropertyMap = BuildSpecialMethodsToPropertyMap (_proxiedType);


      foreach (var proxiedTypeMethod in specialMethodsInProxiedType)
      {
        if (proxiedTypeMethod.Name == "get_NameProperty")
        {
          To.ConsoleLine.e ("proxiedTypeMethod", new StableMethodMetadataToken (proxiedTypeMethod)).e ("proxiedTypeMethod.MetadataToken",proxiedTypeMethod.MetadataToken);
        }

        PropertyInfo knownBaseTypeProperty = null;
        if (_firstKnownBaseType != null)
        {
          firstKnownBaseTypeSpecialMethodsToPropertyMap.TryGetValue (new StableMethodMetadataToken(proxiedTypeMethod), out knownBaseTypeProperty);
          //knownBaseTypeProperty = GetHashSetFromMap (firstKnownBaseTypeSpecialMethodsToPropertyMap, new StableMethodMetadataToken (proxiedTypeMethod)).SingleOrDefault();
        }

        //if (knownBaseTypeProperty != null && // property exists in first known base type
        //    IsMethodBound (proxiedTypeMethod, methodsInFirstKnownBaseType)) // accessor method is visible in first known base type
        if (knownBaseTypeProperty != null)
        {
          // property exists in first known base type
          if (
              IsMethodBound (proxiedTypeMethod, methodsInFirstKnownBaseType)) // accessor method is visible in first known base type
          {
            To.ConsoleLine.s (">>>>>>>>>>>> Implementing public property: ").e (knownBaseTypeProperty.Name);
            _forwardingProxyBuilder.AddForwardingPropertyFromClassOrInterfacePropertyInfoCopy (knownBaseTypeProperty);
          }
        }
      }


      //var proxiedTypeNonPublicProperties =  _proxiedType.CreateSequence(t => t.BaseType).
      //  SelectMany(t => t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic));

      //To.ConsoleLine.nl().e (() => _classMethodToInterfaceMethodsMap).nl();

      //foreach (var proxiedTypeNonPublicProperty in proxiedTypeNonPublicProperties)
      //{
      //  var proxiedTypeNonPublicPropertyAccessors =
      //      proxiedTypeNonPublicProperty.GetAccessors (true);
      //  if (proxiedTypeNonPublicPropertyAccessors.Any (mi => GetInterfaceMethodsToClassMethod (mi).Any ()))
      //  {
      //    _forwardingProxyBuilder.AddForwardingExplicitInterfaceProperty (proxiedTypeNonPublicProperty);
      //  }
      //}


      Type type = _proxiedType;
      while (type != null)
      {
        var typeNonPublicProperties = type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);

        To.ConsoleLine.nl ().e(type.Name).e (() => typeNonPublicProperties).nl ();

        //var typeKnownInterfaceMaps = _knownInterfaces.Select(i => type.GetInterfaceMap(i));
        //foreach (var knownInterfaceMap in typeKnownInterfaceMaps)
        //{
        //  for
        //}

        // Build class method to interface method map for current type
        // TODO: Optimize (cache)
        var classMethodToInterfaceMethodsMap = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
        var knownInterfacesInType = _knownInterfaces.Intersect(type.GetInterfaces());
        foreach (var knownInterface in knownInterfacesInType)
        {
          var interfaceMapping = type.GetInterfaceMap (knownInterface);
          var classMethods = interfaceMapping.TargetMethods;
          var interfaceMethods = interfaceMapping.InterfaceMethods;

          for (int i = 0; i < classMethods.Length; i++)
          {
            var classMethod = classMethods[i];
            if (classMethod.IsSpecialName)
            {
              if (!classMethodToInterfaceMethodsMap.ContainsKey (classMethod))
              {
                classMethodToInterfaceMethodsMap[classMethod] = new HashSet<MethodInfo>();
              }
              classMethodToInterfaceMethodsMap[classMethod].Add (interfaceMethods[i]);
            }
          }
        }

        To.ConsoleLine.e (classMethodToInterfaceMethodsMap);

        foreach (var nonPublicProperty in typeNonPublicProperties)
        {
          // implementedProperties.Contains(property)

          var typeNonPublicPropertyAccessors = nonPublicProperty.GetAccessors (true);
          if (typeNonPublicPropertyAccessors.Any (mi => classMethodToInterfaceMethodsMap.ContainsKey (mi)))
          {
            var getter = GetInterfaceMethodsToClassMethod (nonPublicProperty.GetGetMethod (true),classMethodToInterfaceMethodsMap).Single (); 
            var setter = GetInterfaceMethodsToClassMethod (nonPublicProperty.GetSetMethod (true),classMethodToInterfaceMethodsMap).Single ();

            To.ConsoleLine.s (">>>>>>>>>>>> Implementing property: ").e (nonPublicProperty.Name);
            _forwardingProxyBuilder.AddForwardingExplicitInterfaceProperty (nonPublicProperty, getter, setter);
            //_forwardingProxyBuilder.AddForwardingPropertyFromClassOrInterfacePropertyInfoCopy (property); // !!!!!!! TEST !!!!!!!!!!!!!!!!
          }
        }

        type = type.BaseType;
      }
    }


   private HashSet<T1> GetHashSetFromMap<T0,T1> (Dictionary<T0,HashSet<T1>> map, T0 classMethod)
    {
      HashSet<T1> hashSet;
      map.TryGetValue (classMethod, out hashSet);
      return hashSet ?? new HashSet<T1>();
    }


    //private Dictionary<StableMetadataToken, HashSet<PropertyInfo>> BuildSpecialMethodsToPropertyMap (Type startType)
   private Dictionary<StableMetadataToken, PropertyInfo> BuildSpecialMethodsToPropertyMap (Type startType)
    {
      //const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public; 
      
      //var specialMethodsToPropertyMap = startType.CreateSequence (t => t.BaseType).SelectMany (
      //    t => t.GetProperties (bindingFlags).
      //             SelectMany (
      //             pi =>
      //             new[] { new Tuple<MethodInfo, PropertyInfo> (pi.GetGetMethod (), pi), new Tuple<MethodInfo, PropertyInfo> (pi.GetSetMethod (), pi) })).
      //    Where(tu => tu.A != null).
      //    ToDictionary (x => (StableMetadataToken) new StableMethodMetadataToken (x.A), x => x.B);

      //var specialMethodsToPropertiesMap = new Dictionary<StableMetadataToken , HashSet<PropertyInfo>>();

      //var declaredOnlyProperties = _firstKnownBaseType.CreateSequence (t => t.BaseType).SelectMany (t => t.GetProperties (bindingFlags));
      //foreach (var property in declaredOnlyProperties)
      //{
      //  foreach (var getterSetter in property.GetAccessors(true))
      //  {
      //    AddTo_HashSetMap (specialMethodsToPropertiesMap, new StableMethodMetadataToken (getterSetter), property);
      //  }
      //}

      var specialMethodsToPropertiesMap = new Dictionary<StableMetadataToken, PropertyInfo> ();

      var declaredOnlyProperties = _firstKnownBaseType.CreateSequence (t => t.BaseType).SelectMany (t => t.GetProperties (bindingFlags));
      foreach (var property in declaredOnlyProperties)
      {
        foreach (var getterSetter in property.GetAccessors (true))
        {
          var stableMethodMetadataToken = new StableMethodMetadataToken (getterSetter);
          // Only store first (= nearest to proxiedType) property.
          if (!specialMethodsToPropertiesMap.ContainsKey (stableMethodMetadataToken))
          {
            specialMethodsToPropertiesMap[stableMethodMetadataToken] = property;
          }
        }
      }

      return specialMethodsToPropertiesMap;
    }

    private void AddTo_HashSetMap<T0,T1> (Dictionary<T0,HashSet<T1>> map, T0 classMethod, T1 interfaceMethod)
    {
      if (!map.ContainsKey (classMethod))
      {
        map[classMethod] = new HashSet<T1> ();
      }
      map[classMethod].Add (interfaceMethod);
    }



    private void ImplementKnownMethods ()
    {
      var regularMethodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(mi => !mi.IsSpecialName);
      foreach (var proxiedTypeMethod in regularMethodsInProxiedType)
      {
        //// Skip property getter/setter
        //if (proxiedTypeMethod.IsSpecialName)
        //{
        //  continue; 
        //}

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

      To.ConsoleLine.e ("method", new StableMethodMetadataToken (method)).e ("boundMethod", new StableMethodMetadataToken ((MethodInfo) boundMethod));

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

      // TODO: Use call below
      // return GetInterfaceMethodsToClassMethod (classMethod, _classMethodToInterfaceMethodsMap);
    }

    private IEnumerable<MethodInfo> GetInterfaceMethodsToClassMethod (MethodInfo classMethod, 
      Dictionary<MethodInfo, HashSet<MethodInfo>> classMethodToInterfaceMethodsMap)
    {
      HashSet<MethodInfo> interfaceMethodsToClassMethod;
      classMethodToInterfaceMethodsMap.TryGetValue (classMethod, out interfaceMethodsToClassMethod);
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
