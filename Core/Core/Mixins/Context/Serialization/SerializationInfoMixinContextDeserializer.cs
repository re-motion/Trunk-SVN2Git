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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  public class SerializationInfoMixinContextDeserializer : IMixinContextDeserializer
  {
    private readonly SerializationInfo _info;
    private readonly string _prefix;

    public SerializationInfoMixinContextDeserializer (SerializationInfo info, string prefix)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("prefix", prefix);

      _info = info;
      _prefix = prefix;
    }

    public Type GetMixinType()
    {
      return Type.GetType (_info.GetString (_prefix + "MixinType.AssemblyQualifiedName"));
    }

    public MixinKind GetMixinKind()
    {
      return (MixinKind) _info.GetValue (_prefix + "MixinKind", typeof (MixinKind));
    }

    public MemberVisibility GetIntroducedMemberVisibility()
    {
      return (MemberVisibility) _info.GetValue (_prefix + "IntroducedMemberVisibility", typeof (MemberVisibility));
    }

    public IEnumerable<Type> GetExplicitDependencies()
    {
      var typeNames = (string[]) _info.GetValue (_prefix + "ExplicitDependencies.AssemblyQualifiedNames", typeof (string[]));
      return typeNames.Select (s => Type.GetType (s));
    }
  }
}