using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Supplier : Partner
  {
    public new static Supplier NewObject ()
    {
      return NewObject<Supplier> ().With();
    }

    public new static Supplier GetObject (ObjectID id)
    {
      return GetObject<Supplier> (id);
    }

    protected Supplier()
    {
    }

    public abstract int SupplierQuality { get; set; }
  }
}
