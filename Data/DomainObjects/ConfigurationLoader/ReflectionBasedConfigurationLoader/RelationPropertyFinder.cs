using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="RelationPropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a 
  /// <see cref="RelationEndPointDefinition"/>.
  /// </summary>
  public class RelationPropertyFinder : PropertyFinderBase
  {
    public RelationPropertyFinder (Type type, bool includeBaseProperties, IEnumerable<Type> persistentMixins)
        : base (type, includeBaseProperties, persistentMixins)
    {
    }

    protected override bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (classDefinition, propertyInfo))
        return false;

      return IsRelationEndPoint (propertyInfo);
    }

    private bool IsRelationEndPoint (PropertyInfo propertyInfo)
    {
      return typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType) || ReflectionUtility.IsObjectList (propertyInfo.PropertyType);
    }
  }
}