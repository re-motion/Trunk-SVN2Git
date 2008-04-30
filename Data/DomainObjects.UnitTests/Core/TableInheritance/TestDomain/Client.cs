using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Client")]
  [DBTable ("TableInheritance_Client")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class Client : DomainObject
  {
    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    public static Client GetObject (ObjectID id)
    {
      return GetObject<Client> (id);
    }

    protected Client ()
    {
    }

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public abstract ObjectList<DomainBase> AssignedObjects { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}