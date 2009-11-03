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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IExtensibleEnumValueDiscoveryService"/> by discovering and invoking extension methods defining extensible enum values
  /// via reflection and <see cref="ITypeDiscoveryService"/>.
  /// </summary>
  public class ExtensibleEnumValueDiscoveryService : IExtensibleEnumValueDiscoveryService
  {
    public static IEnumerable<T> GetValuesForTypes<T> (ExtensibleEnumDefinition<T> definition, IEnumerable<Type> typeCandidates) 
        where T: ExtensibleEnum<T>
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("typeCandidates", typeCandidates);

      return from type in GetStaticTypes (typeCandidates) // optimization: only static types can have extension methods
             from value in GetValuesForType (definition, type)
             select value;
    }

    public static IEnumerable<T> GetValuesForType<T> (ExtensibleEnumDefinition<T> definition, Type typeDeclaringMethods) 
        where T: ExtensibleEnum<T>
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("typeDeclaringMethods", typeDeclaringMethods);

      var methods = typeDeclaringMethods.GetMethods (BindingFlags.Static | BindingFlags.Public);
      var extensionMethods = GetValueExtensionMethods (typeof (T), methods);
      return Enumerable.Select (extensionMethods, mi => (T) mi.Invoke (null, new object[] { definition }));
    }

    public static IEnumerable<Type> GetStaticTypes (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return types.Where (t => t.IsAbstract && t.IsSealed && !t.IsGenericTypeDefinition);
    }

    public static IEnumerable<MethodInfo> GetValueExtensionMethods (Type extensibleEnumType, IEnumerable<MethodInfo> methodCandidates)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);
      ArgumentUtility.CheckNotNull ("methodCandidates", methodCandidates);

      var extensibleEnumValuesType = typeof (ExtensibleEnumDefinition<>).MakeGenericType (extensibleEnumType);
      return from m in methodCandidates
             where m.IsPublic
                   && !m.IsGenericMethod
                   && extensibleEnumType.IsAssignableFrom (m.ReturnType)
                   && m.IsDefined (typeof (ExtensionAttribute), false)
             let parameters = m.GetParameters ()
             where parameters.Length == 1
                   && parameters[0].ParameterType == extensibleEnumValuesType
             select m;
    }

    private readonly ITypeDiscoveryService _typeDiscoveryService;

    public ExtensibleEnumValueDiscoveryService (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
    }

    public ITypeDiscoveryService TypeDiscoveryService
    {
      get { return _typeDiscoveryService; }
    }

    public IEnumerable<T> GetValues<T> (ExtensibleEnumDefinition<T> definition) where T: ExtensibleEnum<T>
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var types = _typeDiscoveryService.GetTypes (null, false).Cast<Type>();
      return GetValuesForTypes (definition, types);
    }
  }
}