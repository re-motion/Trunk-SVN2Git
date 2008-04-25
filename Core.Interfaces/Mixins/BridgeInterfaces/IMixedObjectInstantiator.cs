using System;
using Remotion.Implementation;
using Remotion.Reflection;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation("Remotion.Mixins.BridgeImplementations.MixedObjectInstantiator, Remotion, Version = <version>")]
  public interface IMixedObjectInstantiator
  {
    FuncInvokerWrapper<T> CreateConstructorInvoker<T> (Type baseTypeOrInterface, GenerationPolicy generationPolicy, bool allowNonPublic,
        params object[] preparedMixins);
  }
}