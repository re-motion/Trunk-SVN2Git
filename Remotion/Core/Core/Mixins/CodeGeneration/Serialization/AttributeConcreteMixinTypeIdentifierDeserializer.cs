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
using Remotion.Collections;

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
      var externalOverriderArray = (object[]) _values[1];
      return new HashSet<MethodInfo> (from object[] typeAndMethodData in externalOverriderArray
                                      let declaringType = (Type) typeAndMethodData[0]
                                      let name = (string) typeAndMethodData[1]
                                      let signature = (string) typeAndMethodData[2]
                                      select MethodResolver.ResolveMethod (declaringType, name, signature));
    }

    public HashSet<MethodInfo> GetWrappedProtectedMembers (Type mixinType)
    {
      var protectedMemberArray = (object[]) _values[2];
      return new HashSet<MethodInfo> (from object[] methodData in protectedMemberArray
                                      let name = (string) methodData[0]
                                      let signature = (string) methodData[1]
                                      select MethodResolver.ResolveMethod (mixinType, name, signature));
    }

    
  }
}