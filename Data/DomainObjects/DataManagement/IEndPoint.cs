using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement
{
public interface IEndPoint : INullObject
{
  ClientTransaction ClientTransaction { get; }
  DomainObject GetDomainObject ();
  DataContainer GetDataContainer ();
  ObjectID ObjectID { get; }

  RelationDefinition RelationDefinition { get; }
  IRelationEndPointDefinition Definition { get; }
}
}
