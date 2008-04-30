using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Person : TestDomainBase
  {
    public static Person NewObject ()
    {
      return NewObject<Person>().With();
    }

    public new static Person GetObject (ObjectID id)
    {
      return GetObject<Person> (id);
    }

    protected Person()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ContactPerson")]
    public abstract Partner AssociatedPartnerCompany { get; set; }
  }
}
