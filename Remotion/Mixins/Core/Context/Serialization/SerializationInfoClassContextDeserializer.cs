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

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Deserializes the data serialized by a <see cref="SerializationInfoClassContextSerializer"/>.
  /// </summary>
  public class SerializationInfoClassContextDeserializer : SerializationInfoDeserializerBase, IClassContextDeserializer
  {
    public SerializationInfoClassContextDeserializer (SerializationInfo info, string prefix)
        : base(info, prefix)
    {
    }

    public Type GetClassType()
    {
      return GetType ("ClassType");
    }

    public IEnumerable<MixinContext> GetMixins()
    {
      int mixinCount = GetValue<int> ("Mixins.Count");
      for (int i = 0; i < mixinCount; ++i)
      {
        var deserializer = new SerializationInfoMixinContextDeserializer (Info, Prefix + ".Mixins[" + i + "]");
        yield return MixinContext.Deserialize (deserializer);
      }
    }

    public IEnumerable<Type> GetCompleteInterfaces ()
    {
      return GetTypes("CompleteInterfaces");
    }
  }
}
