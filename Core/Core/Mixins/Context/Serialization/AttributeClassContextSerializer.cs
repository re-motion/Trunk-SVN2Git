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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  public class AttributeClassContextSerializer : IClassContextSerializer
  {
    private readonly object[] _values = new object[3];

    public object[] Values
    {
      get { return _values; }
    }

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      Values[0] = type;
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);
      Values[1] = EnumerableUtility.SelectToArray<MixinContext, object[]> (mixinContexts, SerializeMixinContext);
    }

    public void AddCompleteInterfaces(IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);
      Values[2] = EnumerableUtility.ToArray (completeInterfaces);
    }

    private object[] SerializeMixinContext (MixinContext m)
    {
      var serializer = new AttributeMixinContextSerializer ();
      m.Serialize (serializer);
      return serializer.Values;
    }
  }
}