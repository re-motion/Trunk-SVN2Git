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

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Serializes instances of <see cref="ConcreteMixinTypeIdentifier"/> into a format that can be used as a custom attribute parameter.
  /// </summary>
  public class AttributeConcreteMixinTypeIdentifierDeserializer : IConcreteMixinTypeIdentifierDeserializer
  {
    private readonly object[] _values;

    public AttributeConcreteMixinTypeIdentifierDeserializer (object[] values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      _values = values;
    }

    public Type GetMixinType ()
    {
      return (Type) _values[0];
    }

    public HashSet<MethodInfo> GetExternalOverriders ()
    {
      var externalOverriderData = (object[]) _values[1];
      return new HashSet<MethodInfo> (from object[] typeAndToken in externalOverriderData
                                      let declaringType = (Type) typeAndToken[0]
                                      let token = (int) typeAndToken[1]
                                      select ResolveMethod (token, declaringType));
    }

    public HashSet<MethodInfo> GetWrappedProtectedMembers (Type mixinType)
    {
      var protectedMemberTokens = (int[]) _values[2];
      return new HashSet<MethodInfo> (from token in protectedMemberTokens
                                      select ResolveMethod (token, mixinType));
    }

    private MethodInfo ResolveMethod (int token, Type declaringType)
    {
      var method = (MethodInfo) declaringType.Module.ResolveMethod (token);
      if (declaringType.IsGenericType)
        return (MethodInfo) MethodInfo.GetMethodFromHandle (method.MethodHandle, declaringType.TypeHandle);
      else
        return method;
    }
  }
}