using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="PropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a <see cref="PropertyDefinition"/>.
  /// </summary>
  public class PropertyFinder : PropertyFinderBase
  {
    public PropertyFinder (Type type, bool includeBaseProperties, IEnumerable<Type> persistentMixins)
        : base (type, includeBaseProperties, persistentMixins)
    {
    }

    protected override bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (classDefinition, propertyInfo))
        return false;

      if (IsVirtualRelationEndPoint (classDefinition, propertyInfo))
        return false;

      return true;
    }

    private bool IsVirtualRelationEndPoint (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, propertyInfo);
      return relationEndPointReflector.IsVirtualEndRelationEndpoint ();
    }
  }
}