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
using System.Reflection;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class MixinImplementation : IMixinImplementation
  {
    public TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return (TMixin) Get (typeof (TMixin), mixinTarget);
    }

    public object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      var castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        return FindMixin (castMixinTarget, mixinType);
      }
      return null;
    }

    private static object FindMixin (IMixinTarget mixinTarget, Type mixinType)
    {
      object mixin = null;
      foreach (var potentialMixin in mixinTarget.Mixins)
      {
        if (IsTypeMatch (potentialMixin.GetType (), mixinType))
        {
          if (mixin != null)
          {
            string message = string.Format (
                "Both mixins '{0}' and '{1}' match the given type '{2}'.",
                mixin.GetType ().FullName,
                potentialMixin.GetType ().FullName,
                mixinType.Name);
            throw new AmbiguousMatchException (message);
          }

          mixin = potentialMixin;
        }
      }

      return mixin;
    }

    private static bool IsTypeMatch (Type potentialMixinType, Type searchedMixinType)
    {
      return searchedMixinType.IsAssignableFrom (potentialMixinType) 
          || (searchedMixinType.IsGenericTypeDefinition 
              && potentialMixinType.IsGenericType 
              && potentialMixinType.GetGenericTypeDefinition() == searchedMixinType);
    }
  }
}
