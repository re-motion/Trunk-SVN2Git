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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="MixinContext"/> into an array suitable for use as a custom attribute parameter.
  /// </summary>
  public class AttributeMixinContextSerializer : IMixinContextSerializer
  {
    private readonly object[] _values = new object[4];

    public object[] Values
    {
      get { return _values; }
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      Values[0] = mixinType;
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      Values[1] = mixinKind;
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      Values[2] = introducedMemberVisibility;
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      Values[3] = explicitDependencies.ToArray ();
    }
  }
}
