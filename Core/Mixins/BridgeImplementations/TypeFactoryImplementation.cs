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
