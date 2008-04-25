using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Order")]
  [DBTable ("TableInheritance_Order")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class Order: DomainObject
  {
    public static Order NewObject()
    {
      return NewObject<Order>().With();
    }

    public static Order GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Order> (id);
    }

    protected Order()
    {
    }

    public abstract int Number { get; set; }

    public abstract DateTime OrderDate { get; set; }

    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }
  }
}