using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_HistoryEntry")]
  [DBTable ("TableInheritance_HistoryEntry")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class HistoryEntry: DomainObject
  {
    public static HistoryEntry NewObject()
    {
      return NewObject<HistoryEntry>().With();
    }

    public static HistoryEntry GetObject (ObjectID id)
    {
      return DomainObject.GetObject<HistoryEntry> (id);
    }

    protected HistoryEntry()
    {
    }

    public abstract DateTime HistoryDate { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 250)]
    public abstract string Text { get; set; }

    [DBBidirectionalRelation ("HistoryEntries")]
    [Mandatory]
    public abstract DomainBase Owner { get; set; }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}