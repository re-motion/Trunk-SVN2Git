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
using System.Collections;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Globalization
{
  public class MixedResourceManagerResolver<TAttribute> : ResourceManagerResolver<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    public override IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {
      if (TypeUtility.IsGeneratedConcreteMixedType (objectType))
        objectType = TypeUtility.GetUnderlyingTargetType (objectType);

      return base.GetResourceManager (objectType, includeHierarchy, out definingType);
    }

    protected override object GetResourceManagerSetCacheKey (Type definingType, bool includeHierarchy)
    {
      return Tuple.NewTuple (
          base.GetResourceManagerSetCacheKey (definingType, includeHierarchy),
          TargetClassDefinitionUtility.GetContext (definingType, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured));
    }

    protected override TAttribute[] FindFirstResourceDefinitionsInBaseTypes (Type derivedType, out Type definingType)
    {
      ArgumentUtility.CheckNotNull ("derivedType", derivedType);

      TargetClassDefinition mixinConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (derivedType);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
        {
          TAttribute[] attributes;
          
          Type mixinDefiningType;
          FindFirstResourceDefinitions (mixinDefinition.Type, true, out mixinDefiningType, out attributes);
          if (attributes.Length != 0)
          {
            definingType = mixinDefiningType;
            return attributes;
          }
        }
      }
      return base.FindFirstResourceDefinitionsInBaseTypes (derivedType, out definingType);
    }

    protected override void WalkHierarchyAndPrependResourceManagers (ArrayList resourceManagers, Type definingType)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);
      ArgumentUtility.CheckNotNull ("definingType", definingType);

      TargetClassDefinition mixinConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (definingType);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
          PrependMixinResourceManagers (resourceManagers, mixinDefinition.Type);
      }

      base.WalkHierarchyAndPrependResourceManagers (resourceManagers, definingType);
    }

    private void PrependMixinResourceManagers (ArrayList resourceManagers, Type mixinType)
    {
      Type currentType;
      TAttribute[] resourceAttributes;
      FindFirstResourceDefinitions (mixinType, true, out currentType, out resourceAttributes);
      if (currentType != null)
      {
        resourceManagers.InsertRange (0, GetResourceManagers (currentType.Assembly, resourceAttributes));
        WalkHierarchyAndPrependResourceManagers (resourceManagers, currentType);
      }
    }
  }
}
