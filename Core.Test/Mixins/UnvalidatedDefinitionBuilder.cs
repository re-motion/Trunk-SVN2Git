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
