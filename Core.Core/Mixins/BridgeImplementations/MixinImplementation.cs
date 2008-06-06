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
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class MixinImplementation : IMixinImplementation
  {
    public TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get<TMixin> (mixinTarget);
    }

    public object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return MixinReflector.Get (mixinType, mixinTarget);
    }
  }
}
