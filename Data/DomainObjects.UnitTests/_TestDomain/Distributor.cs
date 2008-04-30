using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Distributor : Partner
  {
    public static new Distributor NewObject ()
    {
      return NewObject<Distributor> ().With();
    }

    public new static Distributor GetObject (ObjectID id)
    {
      return GetObject<Distributor> (id);
    }

    protected Distributor ()
    {
    }

    public abstract int NumberOfShops { get; set; }

    [DBBidirectionalRelation ("Distributor")]
    private ClassWithoutRelatedClassIDColumn ClassWithoutRelatedClassIDColumn
    {
      get { return (ClassWithoutRelatedClassIDColumn) GetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn"); }
      set { SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn", value); }
    }
  }
}
