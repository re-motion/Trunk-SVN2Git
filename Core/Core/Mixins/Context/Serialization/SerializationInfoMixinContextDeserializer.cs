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
using System.Linq;

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoMixinContextDeserializer : IMixinContextDeserializer
  {
    private readonly SerializationInfo _info;

    public SerializationInfoMixinContextDeserializer (SerializationInfo info)
    {
      _info = info;
    }

    public Type GetMixinType()
    {
      return Type.GetType (_info.GetString ("mixinType.AssemblyQualifiedName"));
    }

    public MixinKind GetMixinKind()
    {
      return (MixinKind) _info.GetValue ("mixinKind", typeof (MixinKind));
    }

    public MemberVisibility GetIntroducedMemberVisibility()
    {
      return (MemberVisibility) _info.GetValue ("introducedMemberVisibility", typeof (MemberVisibility));
    }

    public IEnumerable<Type> GetExplicitDependencies()
    {
      var typeNames = (string[]) _info.GetValue ("explicitDependencies.AssemblyQualifiedNames", typeof (string[]));
      return typeNames.Select (s => Type.GetType (s));
    }
  }
}