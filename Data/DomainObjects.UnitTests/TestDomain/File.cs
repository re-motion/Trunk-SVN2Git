using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class File : FileSystemItem
  {
    protected File()
    {
    }
  }
}
