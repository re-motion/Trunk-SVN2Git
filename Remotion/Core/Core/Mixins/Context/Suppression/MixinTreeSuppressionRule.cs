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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.Suppression
{
  /// <summary>
  /// Suppresses all mixins that can be ascribed to a given base class.
  /// </summary>
  public class MixinTreeSuppressionRule : IMixinSuppressionRule
  {
    public MixinTreeSuppressionRule (Type baseTypeToSuppress)
    {
      ArgumentUtility.CheckNotNull ("baseTypeToSuppress", baseTypeToSuppress);
      BaseTypeToSuppress = baseTypeToSuppress;
    }

    public Type BaseTypeToSuppress { get; private set; }

    public void RemoveAffectedMixins (Dictionary<Type, MixinContext> configuredMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("configuredMixinTypes", configuredMixinTypes);

      foreach (var configuredMixinType in configuredMixinTypes.Keys.ToList()) // need to clone collection, otherwise we can't remove
      {
        if (ReflectionUtility.CanAscribe (configuredMixinType, BaseTypeToSuppress))
          configuredMixinTypes.Remove (configuredMixinType);
      }
    }
  }
}