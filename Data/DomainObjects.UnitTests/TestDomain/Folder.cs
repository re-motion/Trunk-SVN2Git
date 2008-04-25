using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Folder : FileSystemItem
  {
    public new static Folder NewObject ()
    {
      return NewObject<Folder> ().With();
    }

    protected Folder ()
    {
    }

    [DBBidirectionalRelation ("ParentFolder")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

  }
}
