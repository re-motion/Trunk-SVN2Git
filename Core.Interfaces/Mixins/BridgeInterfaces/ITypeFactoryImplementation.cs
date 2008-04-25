using System;
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Mixins.BridgeImplementations.TypeFactoryImplementation, Remotion, Version = <version>")]
  public interface ITypeFactoryImplementation
  {
    Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy);
    void InitializeUnconstructedInstance (object mixinTarget);
  }
}