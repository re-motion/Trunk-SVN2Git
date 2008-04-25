using System;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class TypeFactoryImplementation : ITypeFactoryImplementation
  {
    public Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      TargetClassDefinition configuration = TargetClassDefinitionUtility.GetActiveConfiguration (targetType, generationPolicy);
      if (configuration == null)
        return targetType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (configuration);
    }

    public void InitializeUnconstructedInstance (object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ArgumentUtility.CheckType<IMixinTarget> ("mixinTarget", mixinTarget);
      ConcreteTypeBuilder.Current.InitializeUnconstructedInstance ((IMixinTarget) mixinTarget);
    }
  }
}