using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Ceo : TestDomainBase
  {
    public static Ceo NewObject ()
    {
      return NewObject<Ceo> ().With();
    }

    public new static Ceo GetObject (ObjectID id)
    {
      return GetObject<Ceo> (id);
    }

    protected Ceo ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Ceo", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Company Company { get; set; }
  }
}
