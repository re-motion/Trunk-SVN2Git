// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      _info.AddValue (_prefix + ".MixinType.AssemblyQualifiedName", mixinType.AssemblyQualifiedName);
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      _info.AddValue (_prefix + ".MixinKind", mixinKind);
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      _info.AddValue (_prefix + ".IntroducedMemberVisibility", introducedMemberVisibility);
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      var typeNames = EnumerableUtility.SelectToArray (explicitDependencies, t => t.AssemblyQualifiedName);
      _info.AddValue (_prefix + ".ExplicitDependencies.AssemblyQualifiedNames", typeNames);
    }
  }
}
