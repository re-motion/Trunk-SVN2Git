// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Map between <see cref="StableMetadataToken"/> and <see cref="MethodInfo"/>.
  /// </summary>
  public class StableMetadataTokenToMethodInfoMap
  {
    private readonly Dictionary<StableMetadataToken, MethodInfo> _map = new Dictionary<StableMetadataToken, MethodInfo>();

    public StableMetadataTokenToMethodInfoMap (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _map = type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToDictionary (
        mi => new StableMetadataToken(mi), mi => mi);
    }


    public MethodInfo GetMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      return GetMethod (new StableMetadataToken (method));
    }

    public MethodInfo GetMethod (StableMetadataToken stableMetadataToken)
    {
      ArgumentUtility.CheckNotNull ("stableMetadataToken", stableMetadataToken);
      MethodInfo correspondingMethod;
      _map.TryGetValue (stableMetadataToken, out correspondingMethod);
      return correspondingMethod;
    }
  }
}