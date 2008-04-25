using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  public abstract class Partner : Company
  {
    public new static Partner NewObject()
    {
      return NewObject<Partner>().With();
    }

    protected Partner()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Description { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn("PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }
  }
}