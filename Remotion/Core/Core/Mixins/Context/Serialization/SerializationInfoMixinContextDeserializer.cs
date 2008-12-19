// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
