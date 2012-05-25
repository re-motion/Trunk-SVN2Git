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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="MixinContextOrigin"/> object into a <see cref="SerializationInfo"/> object in such a way that deserialization can occur in
  /// a single pass (required by mixed object serialization - <see cref="SerializationHelper"/>).
  /// </summary>
  public class SerializationInfoMixinContextOriginSerializer : SerializationInfoSerializerBase, IMixinContextOriginSerializer
  {
    public SerializationInfoMixinContextOriginSerializer (SerializationInfo info, string prefix)
        : base (info, prefix)
    {
    }

    public void AddKind (string kind)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);

      AddValue ("Kind", kind);
    }

    public void AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      // Assembly needs to be serialized as a string; otherwise, SerializationHelper won't be able to use it immediately after deserialization.
      AddValue ("Assembly.FullName", assembly.FullName);
    }

    public void AddLocation (string location)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("location", location);

      AddValue ("Location", location);
    }
  }
}