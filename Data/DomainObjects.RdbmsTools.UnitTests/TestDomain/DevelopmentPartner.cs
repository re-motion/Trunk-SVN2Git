using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [Instantiable]
  public abstract class DevelopmentPartner : Partner
  {
    public new static DevelopmentPartner NewObject()
    {
      return NewObject<DevelopmentPartner>().With();
    }

    protected DevelopmentPartner()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Competences { get; set; }
  }
}