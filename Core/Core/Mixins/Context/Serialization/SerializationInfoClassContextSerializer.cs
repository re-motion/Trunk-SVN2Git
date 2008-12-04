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
using System.Runtime.Serialization;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoClassContextSerializer : IClassContextSerializer
  {
    private readonly SerializationInfo _info;

    public SerializationInfoClassContextSerializer (SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _info = info;
    }

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _info.AddValue ("ClassType.AssemblyQualifiedName", type.AssemblyQualifiedName);
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);
      int index = 0;
      foreach (var mixinContext in mixinContexts)
      {
        var serializer = new SerializationInfoMixinContextSerializer(_info, "Mixins[" + index + "].");
        mixinContext.Serialize (serializer);
        ++index;
      }
      _info.AddValue ("Mixins.Count", index);
    }

    public void AddCompleteInterfaces(IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);
      var typeNames = EnumerableUtility.SelectToArray (completeInterfaces, t => t.AssemblyQualifiedName);
      _info.AddValue ("CompleteInterfaces.AssemblyQualifiedNames", typeNames);
    }
  }
}