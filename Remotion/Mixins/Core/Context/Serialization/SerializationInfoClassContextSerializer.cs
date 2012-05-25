// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="ClassContext"/> object into a <see cref="SerializationInfo"/> object in such a way that deserialization can occur in
  /// a single pass (required by mixed object serialization - <see cref="SerializationHelper"/>).
  /// </summary>
  public class SerializationInfoClassContextSerializer : SerializationInfoSerializerBase, IClassContextSerializer
  {
    public SerializationInfoClassContextSerializer (SerializationInfo info, string prefix)
        : base(info, prefix)
    {
    }

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      AddType ("ClassType", type);
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);
      int index = 0;
      foreach (var mixinContext in mixinContexts)
      {
        var serializer = new SerializationInfoMixinContextSerializer (Info, Prefix + ".Mixins[" + index + "]");
        mixinContext.Serialize (serializer);
        ++index;
      }

      AddValue ("Mixins.Count", index);
    }

    public void AddCompleteInterfaces(IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);

      // Types must be serialized as a string; otherwise, SerializationHelper won't be able to use them immediately after deserialization.
      AddTypes ("CompleteInterfaces", completeInterfaces);
    }
  }
}
