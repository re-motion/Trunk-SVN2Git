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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public class AttributeBasedMetadataImporter : IConcreteTypeMetadataImporter
  {
    // TODO: Test
    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual IEnumerable<TargetClassDefinition> GetMetadataForMixedType (_Type concreteMixedType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      foreach (ConcreteMixedTypeAttribute typeDescriptor in concreteMixedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))
      {
        TargetClassDefinition targetClassDefinition = typeDescriptor.GetTargetClassDefinition (targetClassDefinitionCache);
        yield return targetClassDefinition;
      }
    }

    // TODO: Test
    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual IEnumerable<MixinDefinition> GetMetadataForMixinType (_Type concreteMixinType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      foreach (ConcreteMixinTypeAttribute typeDescriptor in concreteMixinType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))
      {
        MixinDefinition mixinDefinition = typeDescriptor.GetMixinDefinition (targetClassDefinitionCache);
        yield return mixinDefinition;
      }
    }

    // TODO: Test
    public IEnumerable<TargetClassDefinition> GetMetadataForMixedType (Type concreteMixedType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      return GetMetadataForMixedType ((_Type) concreteMixedType, targetClassDefinitionCache);
    }

    // TODO: Test
    public IEnumerable<MixinDefinition> GetMetadataForMixinType (Type concreteMixinType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      return (GetMetadataForMixinType ((_Type) concreteMixinType, targetClassDefinitionCache));
    }
  }
}