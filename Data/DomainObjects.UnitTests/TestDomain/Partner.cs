using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Partner : Company
  {
    public static new Partner NewObject ()
    {
      return NewObject<Partner>().With();
    }

    public new static Partner GetObject (ObjectID id)
    {
      return GetObject<Partner> (id);
    }

    protected Partner ()
    {
    }

    [DBBidirectionalRelation ("AssociatedPartnerCompany", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Person ContactPerson { get; set; }
  }
}
