using System;

using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  public abstract class ClientBoundBaseClass : DomainObject
  {
    // types

    // static members and constants

    public static ClientBoundBaseClass GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClientBoundBaseClass> (id);
    }

    public static ClientBoundBaseClass GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.GetObject<ClientBoundBaseClass> (id);
      }
    }

    // member fields

    // construction and disposing

    protected ClientBoundBaseClass ()
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClientBoundBaseClasses")]
    [Mandatory]
    public abstract Client Client { get; set;}
  }
}
