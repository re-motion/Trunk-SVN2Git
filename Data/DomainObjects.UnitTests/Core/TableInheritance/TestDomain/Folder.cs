using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Folder")]
  [DBTable ("TableInheritance_Folder")]
  [Instantiable]
  public abstract class Folder: FileSystemItem
  {
    public static Folder NewObject()
    {
      return NewObject<Folder>().With();
    }

    public static Folder GetObject (ObjectID id)
    {
      return GetObject<Folder> (id);
    }

    protected Folder()
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

    [DBColumn ("FolderCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}