using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public enum CustomerType
  {
    Standard = 0,
    Premium = 1
  }

  [ClassID ("TI_Customer")]
  [Instantiable]
  public abstract class Customer: Person
  {
    public new static Customer NewObject ()
    {
      return NewObject<Customer> ().With ();
    }

    public new static Customer GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Customer> (id);
    }

    protected Customer()
    {
    }

    public abstract CustomerType CustomerType { get; set; }

    public abstract DateTime CustomerSince { get; set; }

    [DBBidirectionalRelation ("Customers")]
    public abstract Region Region { get; set; }

    [DBBidirectionalRelation ("Customer")]
    public abstract ObjectList<Order> Orders { get; }
  }
}