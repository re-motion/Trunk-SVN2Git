using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Company : TestDomainBase
  {
    public static Company NewObject ()
    {
      return NewObject<Company> ().With();
    }

    public new static Company GetObject (ObjectID id)
    {
      return GetObject<Company> (id);
    }

    protected Company ()
    {
    }

    [StorageClassNone]
    internal int NamePropertyOfInvalidType
    {
      set { Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"].SetValue (value); }
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Companies")]
    public virtual IndustrialSector IndustrialSector
    {
      get { return CurrentProperty.GetValue<IndustrialSector> (); }
      set { CurrentProperty.SetValue<IndustrialSector> (value); }
    }

    [DBBidirectionalRelation ("Company")]
    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get { return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation"); }
      set { SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation", value); }
    }
  }
}