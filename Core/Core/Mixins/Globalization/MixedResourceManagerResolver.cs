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
using System.Collections.Generic;

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

    protected override ResourceDefinition<TAttribute> GetResourceDefinition (Type type, Type currentType)
    {
			ResourceDefinition<TAttribute> resourcesOnType = base.GetResourceDefinition (type, currentType);
			if (type == currentType)
			{
				// only on first call, check mixins
				AddSupplementingAttributesFromMixins (currentType, resourcesOnType);
			}
      return resourcesOnType;
    }

    private void AddSupplementingAttributesFromMixins (Type type, ResourceDefinition<TAttribute> resourcesOnType)
    {
      TargetClassDefinition mixinConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (type);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
          AddSupplementingAttiributesFromMixin (mixinDefinition, resourcesOnType, true);
      }
    }

    private void AddSupplementingAttiributesFromMixin (MixinDefinition mixinDefinition, ResourceDefinition<TAttribute> resourcesOnType, bool isHierarchyIncluded)
    {
      IEnumerable<ResourceDefinition<TAttribute>> resourcesOnMixin = GetResourceDefinitionStream (mixinDefinition.Type, isHierarchyIncluded);
      foreach (ResourceDefinition<TAttribute> resourceOnMixin in resourcesOnMixin)
        resourcesOnType.AddSupplementingAttributes (resourceOnMixin.GetAllAttributePairs());
    }
  }
}
