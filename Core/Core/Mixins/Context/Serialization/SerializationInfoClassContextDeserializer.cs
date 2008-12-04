/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

    public SerializationInfoClassContextDeserializer (SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _info = info;
    }

    public Type GetClassType()
    {
      return Type.GetType (_info.GetString ("ClassType.AssemblyQualifiedName"));
    }

    public IEnumerable<MixinContext> GetMixins()
    {
      int mixinCount = _info.GetInt32 ("Mixins.Count");
      for (int i = 0; i < mixinCount; ++i)
      {
        var deserializer = new SerializationInfoMixinContextDeserializer (_info, "Mixins[" + i + "].");
        yield return MixinContext.Deserialize (deserializer);
      }
    }

    public IEnumerable<Type> GetCompleteInterfaces ()
    {
      var typeNames = (string[]) _info.GetValue ("CompleteInterfaces.AssemblyQualifiedNames", typeof (string[]));
      return typeNames.Select (s => Type.GetType (s));
    }
  }
}