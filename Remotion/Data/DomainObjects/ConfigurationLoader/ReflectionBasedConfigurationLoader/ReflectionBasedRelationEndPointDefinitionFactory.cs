using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Implements the <see cref="IRelationEndPointDefinitionFactory"/> by delegating to the <see cref="RelationEndPointReflector"/> class.
  /// </summary>
  public class ReflectionBasedRelationEndPointDefinitionFactory : IRelationEndPointDefinitionFactory
  {
    public IRelationEndPointDefinition CreateEndPoint (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      var endPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, propertyInfo, nameResolver);
      return endPointReflector.GetMetadata();
    }
  }
}