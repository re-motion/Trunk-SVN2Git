using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_FileSystemItem")]
  [TableInheritanceTestDomain]
  public abstract class FileSystemItem : DomainObject
  {
    protected FileSystemItem()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("FileSystemItems")]
    public abstract Folder ParentFolder { get; set; }
  }
}