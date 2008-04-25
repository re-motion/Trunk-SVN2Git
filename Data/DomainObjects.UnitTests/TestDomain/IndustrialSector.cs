using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class IndustrialSector : TestDomainBase
  {
    public static IndustrialSector NewObject ()
    {
      return NewObject<IndustrialSector> ().With ();
    }

    public new static IndustrialSector GetObject (ObjectID id)
    {
      return DomainObject.GetObject<IndustrialSector> (id);
    }

    protected IndustrialSector ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public virtual string Name
    {
      get
      {
        return CurrentProperty.GetValue<string> ();
      }
      set
      {
        CurrentProperty.SetValue (value);
      }
    }

    [DBBidirectionalRelationAttribute ("IndustrialSector")]
    [Mandatory]
    public virtual ObjectList<Company> Companies
    {
      get
      {
        return CurrentProperty.GetValue<ObjectList<Company>> ();
      }
    }
  }
}
