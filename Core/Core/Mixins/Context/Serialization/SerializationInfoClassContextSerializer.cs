// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Runtime.Serialization;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoClassContextSerializer : IClassContextSerializer
  {
    private readonly SerializationInfo _info;
    private readonly string _prefix;

    public SerializationInfoClassContextSerializer (SerializationInfo info, string prefix)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("prefix", prefix);
      _info = info;
      _prefix = prefix;
    }

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _info.AddValue (_prefix + "ClassType.AssemblyQualifiedName", type.AssemblyQualifiedName);
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);
      int index = 0;
      foreach (var mixinContext in mixinContexts)
      {
        var serializer = new SerializationInfoMixinContextSerializer (_info, _prefix + "Mixins[" + index + "].");
        mixinContext.Serialize (serializer);
        ++index;
      }
      _info.AddValue (_prefix + "Mixins.Count", index);
    }

    public void AddCompleteInterfaces(IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);
      var typeNames = EnumerableUtility.SelectToArray (completeInterfaces, t => t.AssemblyQualifiedName);
      _info.AddValue (_prefix + "CompleteInterfaces.AssemblyQualifiedNames", typeNames);
    }
  }
}
