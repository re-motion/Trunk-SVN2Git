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

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Deserializes instances of <see cref="ConcreteMixinTypeIdentifier"/> serialized with <see cref="AttributeConcreteMixinTypeIdentifierSerializer"/>.
  /// </summary>
  public class AttributeConcreteMixinTypeIdentifierSerializer : IConcreteMixinTypeIdentifierSerializer
  {
    private readonly object[] _values = new object[3];

    public object[] Values
    {
      get { return _values; }
    }

    public void AddMixinType (Type mixinType)
    {
      _values[0] = mixinType;
    }

    public void AddExternalOverriders (HashSet<MethodInfo> externalOverriders)
    {
      _values[1] = (from ovr in externalOverriders select new object[] { ovr.DeclaringType, ovr.MetadataToken }).ToArray();
    }

    public void AddWrappedProtectedMembers (HashSet<MethodInfo> wrappedProtectedMembers)
    {
      _values[2] = (from member in wrappedProtectedMembers select member.MetadataToken).ToArray ();
    }
  }
}