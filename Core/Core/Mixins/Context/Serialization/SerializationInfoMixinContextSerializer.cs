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
    private readonly string _prefix;

    public SerializationInfoMixinContextSerializer (SerializationInfo info, string prefix)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("prefix", prefix);

      _info = info;
      _prefix = prefix;
    }

    public void AddMixinType(Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      _info.AddValue (_prefix + "MixinType.AssemblyQualifiedName", mixinType.AssemblyQualifiedName);
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      _info.AddValue (_prefix + "MixinKind", mixinKind);
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      _info.AddValue (_prefix + "IntroducedMemberVisibility", introducedMemberVisibility);
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      var typeNames = EnumerableUtility.SelectToArray (explicitDependencies, t => t.AssemblyQualifiedName);
      _info.AddValue (_prefix + "ExplicitDependencies.AssemblyQualifiedNames", typeNames);
    }
  }
}