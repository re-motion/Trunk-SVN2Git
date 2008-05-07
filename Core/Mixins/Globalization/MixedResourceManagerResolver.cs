using System;
using System.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Mixins.Globalization
{
  public class MixedResourceManagerResolver<TAttribute> : ResourceManagerResolver<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    public override IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {
      if (Mixins.TypeUtility.IsGeneratedConcreteMixedType (objectType))
        objectType = Mixins.TypeUtility.GetUnderlyingTargetType (objectType);

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

    protected override void WalkHierarchyAndPrependResourceManagers (System.Collections.ArrayList resourceManagers, Type definingType)
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