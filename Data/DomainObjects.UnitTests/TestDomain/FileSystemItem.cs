using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class FileSystemItem : TestDomainBase
  {
    public static FileSystemItem NewObject ()
    {
      return NewObject<FileSystemItem> ().With ();
    }

    protected FileSystemItem()
    {
    }

    [DBBidirectionalRelation ("FileSystemItems")]
    public abstract Folder ParentFolder { get; set; }

  }
}
