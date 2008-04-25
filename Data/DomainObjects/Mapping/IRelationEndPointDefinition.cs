using System;

namespace Remotion.Data.DomainObjects.Mapping
{
public interface IRelationEndPointDefinition : INullObject
{
  RelationDefinition RelationDefinition { get; }
  ClassDefinition ClassDefinition { get; }
  string PropertyName { get; }
  Type PropertyType { get; }
  bool IsPropertyTypeResolved { get; }
  string PropertyTypeName { get; }
  bool IsMandatory { get; }
  CardinalityType Cardinality { get; }
  bool IsVirtual { get; }

  bool CorrespondsTo (string classID, string propertyName);
  void SetRelationDefinition (RelationDefinition relationDefinition);
}
}
