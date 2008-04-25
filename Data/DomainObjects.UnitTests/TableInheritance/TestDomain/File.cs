using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_File")]
  [DBTable ("TableInheritance_File")]
  [Instantiable]
  public abstract class File: FileSystemItem
  {
    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    public static File GetObject (ObjectID id)
    {
      return DomainObject.GetObject<File> (id);
    }

    protected File ()
    {
    }

    public abstract int Size { get; set; }

    [DBColumn ("FileCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}