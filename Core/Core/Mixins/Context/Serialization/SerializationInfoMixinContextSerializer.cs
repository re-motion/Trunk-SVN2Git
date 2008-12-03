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

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoMixinContextSerializer : IMixinContextSerializer
  {
    private readonly SerializationInfo _info;

    public SerializationInfoMixinContextSerializer (SerializationInfo info)
    {
      _info = info;
    }

    public void AddMixinType(Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      _info.AddValue ("mixinType.AssemblyQualifiedName", mixinType.AssemblyQualifiedName);
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      _info.AddValue ("mixinKind", mixinKind);
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      _info.AddValue ("introducedMemberVisibility", introducedMemberVisibility);
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      var typeNames = EnumerableUtility.SelectToArray (explicitDependencies, t => t.AssemblyQualifiedName);
      _info.AddValue ("explicitDependencies.AssemblyQualifiedNames", typeNames);
    }
  }
}