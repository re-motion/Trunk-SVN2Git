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
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoClassContextDeserializer : IClassContextDeserializer
  {
    private readonly SerializationInfo _info;
    private readonly string _prefix;

    public SerializationInfoClassContextDeserializer (SerializationInfo info, string prefix)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("prefix", prefix);
      _info = info;
      _prefix = prefix;
    }

    public Type GetClassType()
    {
      return Type.GetType (_info.GetString (_prefix + "ClassType.AssemblyQualifiedName"));
    }

    public IEnumerable<MixinContext> GetMixins()
    {
      int mixinCount = _info.GetInt32 (_prefix + "Mixins.Count");
      for (int i = 0; i < mixinCount; ++i)
      {
        var deserializer = new SerializationInfoMixinContextDeserializer (_info, _prefix + "Mixins[" + i + "].");
        yield return MixinContext.Deserialize (deserializer);
      }
    }

    public IEnumerable<Type> GetCompleteInterfaces ()
    {
      var typeNames = (string[]) _info.GetValue (_prefix + "CompleteInterfaces.AssemblyQualifiedNames", typeof (string[]));
      return typeNames.Select (s => Type.GetType (s));
    }
  }
}
