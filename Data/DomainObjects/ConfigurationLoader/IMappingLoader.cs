using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader
{
  public interface IMappingLoader
  {
    ClassDefinitionCollection GetClassDefinitions();

    RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions);

    bool ResolveTypes { get; }
  }
}