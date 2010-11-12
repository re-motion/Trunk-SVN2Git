using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Defines a factory interface for classes creating <see cref="IRelationEndPointDefinition"/> instances. There is only one implementation
  /// (<see cref="ReflectionBasedRelationEndPointDefinitionFactory"/>), this class exists primarily for testing purposes.
  /// </summary>
  public interface IRelationEndPointDefinitionFactory
  {
    IRelationEndPointDefinition CreateEndPoint (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver);
  }
}