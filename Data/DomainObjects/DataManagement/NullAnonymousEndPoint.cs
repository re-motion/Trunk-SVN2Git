using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class NullAnonymousEndPoint : AnonymousEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullAnonymousEndPoint (RelationDefinition relationDefinition) : base (relationDefinition)
  {
  }

  // methods and properties

  public override ClientTransaction ClientTransaction
  {
    get { return null; }
  }

  public override DomainObject GetDomainObject ()
  {
    return null; 
  }

  public override DataContainer GetDataContainer ()
  {
    return null;
  }

  public override ObjectID ObjectID
  {
    get { return null; }
  }


  public override bool IsNull
  {
    get { return true; }
  }
}
}
