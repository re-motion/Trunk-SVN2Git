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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Builds a proxy object which exposes only selected methods/properties, as decided by its <see cref="ITypeArbiter"/>. 
  /// </summary>
  /// <remarks>
  /// What methods/properties are to be exposed is dependent on whether the method/property comes from a type which is
  /// classified as "valid" by the <see cref="ITypeArbiter"/> of the class.
  /// <para/> 
  /// Used by <see cref="StableBindingProxyProvider"/>.
  /// <para/> 
  /// Uses <see cref="ForwardingProxyBuilder"/>.
  /// </remarks>
  public class StableBindingProxyBuilder
  {
    private readonly Type _proxiedType;
    private readonly ITypeArbiter _typeArbiter;
    private ForwardingProxyBuilder _forwardingProxyBuilder;
    private readonly Dictionary<MemberInfo, HashSet<MemberInfo>> _classMethodToInterfaceMethodsMap = new Dictionary<MemberInfo, HashSet<MemberInfo>> ();

    public StableBindingProxyBuilder (Type proxiedType, ITypeArbiter typeArbiter)
    {
      ArgumentUtility.CheckNotNull ("proxiedType", proxiedType);
      ArgumentUtility.CheckNotNull ("typeArbiter", typeArbiter);
      _typeArbiter = typeArbiter;
      _proxiedType = proxiedType;
      BuildClassMethodToInterfaceMethodsMap();
    }

    public Type ProxiedType
    {
      get { return _proxiedType; }
    }



    private Dictionary<MemberInfo, HashSet<MemberInfo>> BuildClassMethodToInterfaceMethodsMap ()
    {
      var knownInterfaces = ProxiedType.GetInterfaces ().Where (i => _typeArbiter.IsTypeValid (i));
      foreach (var knownInterface in knownInterfaces)
      {
        var interfaceMapping = ProxiedType.GetInterfaceMap (knownInterface);
        var classMethods = interfaceMapping.TargetMethods;
        var interfaceMethods = interfaceMapping.InterfaceMethods;
        for (int i = 0; i < classMethods.Length; i++) 
        {
          AddTo_MethodToInterfaceMethodsMap (classMethods[i], interfaceMethods[i]);
        }
      }
      return _classMethodToInterfaceMethodsMap;
    }

    private void AddTo_MethodToInterfaceMethodsMap (MethodInfo classMethod, MethodInfo interfaceMethod)
    {
      if (!_classMethodToInterfaceMethodsMap.ContainsKey (classMethod))
      {
        _classMethodToInterfaceMethodsMap[classMethod] = new HashSet<MemberInfo> ();
      }
      _classMethodToInterfaceMethodsMap[classMethod].Add (interfaceMethod);
    }

    public IEnumerable<MemberInfo> GetInterfaceMethodsToClassMethod (MethodInfo classMethod)
    {
      HashSet<MemberInfo> interfaceMethodsToClassMethod;
      _classMethodToInterfaceMethodsMap.TryGetValue (classMethod, out interfaceMethodsToClassMethod);
      return (IEnumerable<MemberInfo>) interfaceMethodsToClassMethod ?? new MemberInfo[0];
    }
  }
}