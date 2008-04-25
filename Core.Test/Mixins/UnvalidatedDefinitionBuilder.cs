using System;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;

namespace Remotion.UnitTests.Mixins
{
  public static class UnvalidatedDefinitionBuilder
  {
    public static TargetClassDefinition BuildUnvalidatedDefinition (Type baseType, params Type[] mixinTypes)
    {
      ClassContext context = new ClassContext (baseType, mixinTypes);
      return BuildUnvalidatedDefinition(context);
    }

    public static TargetClassDefinition BuildUnvalidatedDefinition (ClassContext context)
    {
      TargetClassDefinitionBuilder builder = new TargetClassDefinitionBuilder();
      return builder.Build (context);
    }
  }
}
