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
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides services used for the discovery of extensible enum values. This class is used by <see cref="ExtensibleEnumValues{T}"/>.
  /// </summary>
  public class ExtensibleEnumValueDiscoveryService
  {
    public IEnumerable<Type> GetStaticClasses (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return types.Where (t => t.IsAbstract && t.IsSealed && !t.IsGenericTypeDefinition);
    }

    public IEnumerable<MethodInfo> GetValueExtensionMethods (Type extensibleEnumType, Type typeDeclaringMethods)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);
      ArgumentUtility.CheckNotNull ("typeDeclaringMethods", typeDeclaringMethods);

      var methods = typeDeclaringMethods.GetMethods (BindingFlags.Static | BindingFlags.Public);
      return GetValueExtensionMethods (extensibleEnumType, methods);
    }

    public IEnumerable<MethodInfo> GetValueExtensionMethods (Type extensibleEnumType, IEnumerable<MethodInfo> methodCandidates)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);
      ArgumentUtility.CheckNotNull ("methodCandidates", methodCandidates);

      return from m in methodCandidates
             where extensibleEnumType.IsAssignableFrom (m.ReturnType)
                && !m.IsGenericMethod
                && m.IsPublic
                && m.IsDefined (typeof (ExtensionAttribute), false)
             let parameters = m.GetParameters ()
             where parameters.Length == 1
                && parameters[0].ParameterType == typeof (ExtensibleEnumValues<>).MakeGenericType (extensibleEnumType)
             select m;
    }
  }
}