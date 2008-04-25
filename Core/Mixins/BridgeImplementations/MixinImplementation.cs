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