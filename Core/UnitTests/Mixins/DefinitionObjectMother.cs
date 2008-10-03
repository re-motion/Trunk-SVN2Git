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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using BT1Mixin1=Remotion.UnitTests.Mixins.SampleTypes.BT1Mixin1;
using System.Linq;

namespace Remotion.UnitTests.Mixins
{
  public static class DefinitionObjectMother
  {
    public static TargetClassDefinition CreateTargetClassDefinition (Type classType, params Type[] mixinTypes)
    {
      var result = new TargetClassDefinition (new ClassContext (classType, mixinTypes));
      foreach (var type in mixinTypes)
        CreateMixinDefinition (result, type);
      return result;
    }

    public static MixinDefinition CreateMixinDefinition (TargetClassDefinition targetClassDefinition, Type mixinType)
    {
      var mixinDefinition = new MixinDefinition (MixinKind.Used, mixinType, targetClassDefinition, true);
      PrivateInvoke.InvokeNonPublicMethod (targetClassDefinition.Mixins, "Add", mixinDefinition);
      return mixinDefinition;
    }
  }
}