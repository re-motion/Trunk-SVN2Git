// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Collections;
using Remotion.Globalization;
using System.Collections.Generic;

namespace Remotion.Mixins.Globalization
{
  /// <summary>
  /// Extends <see cref="ResourceManagerResolver{TAttribute}"/> to detect resources added via mixins.
  /// </summary>
  /// <typeparam name="TAttribute">The type of the resource attribute to be resolved by this class.</typeparam>
  public class MixedResourceManagerResolver<TAttribute> : ResourceManagerResolver<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    public override IEnumerable<ResourceDefinition<TAttribute>> GetResourceDefinitionStream (Type type, bool includeHierarchy)
    {
      type = MixinTypeUtility.GetUnderlyingTargetType (type); // this adjusts type if it is a mixin engine-generated type, otherwise ignores it

      return base.GetResourceDefinitionStream (type, includeHierarchy);
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

    protected override object GetResourceManagerSetCacheKey (Type definingType, bool includeHierarchy)
    {
      return Tuple.Create (
          base.GetResourceManagerSetCacheKey (definingType, includeHierarchy),
          MixinConfiguration.ActiveConfiguration.GetContext (definingType));
    }

    private void AddSupplementingAttributesFromMixins (Type type, ResourceDefinition<TAttribute> resourcesOnType)
    {
      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (type);
      if (classContext != null)
      {
        foreach (var mixinType in MixinTypeUtility.GetMixinTypesExact (type))
          AddSupplementingAttributesFromMixin (mixinType, resourcesOnType);
      }
    }

    private void AddSupplementingAttributesFromMixin (Type mixinType, ResourceDefinition<TAttribute> resourcesOnType)
    {
      IEnumerable<ResourceDefinition<TAttribute>> resourcesOnMixin = GetResourceDefinitionStream (mixinType, true);
      foreach (ResourceDefinition<TAttribute> resourceOnMixin in resourcesOnMixin)
        resourcesOnType.AddSupplementingAttributes (resourceOnMixin.GetAllAttributePairs());
    }
  }
}
