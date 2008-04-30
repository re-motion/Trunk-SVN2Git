using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_DomainBase")]
  [TableInheritanceTestDomain]
  public abstract class DomainBase : DomainObject
  {
    protected DomainBase()
    {
        InitializeNew();
    }
  
    private void InitializeNew()
    {
      CreatedBy = "UnitTests";
      CreatedAt = DateTime.Now;
    }

    // Note: This property always returns an empty collection.
    [DBBidirectionalRelation ("DomainBase")]
    public abstract ObjectList<AbstractClassWithoutDerivations> AbstractClassesWithoutDerivations { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string CreatedBy { get; set; }

    public abstract DateTime CreatedAt { get; set; }

    [DBBidirectionalRelation ("AssignedObjects")]
    public abstract Client Client { get; }

    [DBBidirectionalRelation ("Owner", SortExpression = "HistoryDate desc")]
    public abstract ObjectList<HistoryEntry> HistoryEntries { get; }

    public new void Delete ()
    {
      base.Delete ();
    }
  }
}