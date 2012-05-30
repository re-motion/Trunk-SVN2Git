// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="MixinContext"/> object into a <see cref="SerializationInfo"/> object in such a way that deserialization can occur in
  /// a single pass (required by mixed object serialization - <see cref="SerializationHelper"/>).
  /// </summary>
  public class SerializationInfoMixinContextSerializer : SerializationInfoSerializerBase, IMixinContextSerializer
  {
    public SerializationInfoMixinContextSerializer (SerializationInfo info, string prefix) : base (info, prefix)
    {
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      AddType ("MixinType", mixinType);
    }

    public void AddMixinKind (MixinKind mixinKind)
    {
      AddValue ("MixinKind", mixinKind);
    }

    public void AddIntroducedMemberVisibility (MemberVisibility introducedMemberVisibility)
    {
      AddValue ("IntroducedMemberVisibility", introducedMemberVisibility);
    }

    public void AddExplicitDependencies (IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);

      AddTypes ("ExplicitDependencies", explicitDependencies);
    }

    public void AddOrigin (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      var originSerializer = new SerializationInfoMixinContextOriginSerializer (Info, Prefix + ".Origin");
      origin.Serialize (originSerializer);
    }
  }
}