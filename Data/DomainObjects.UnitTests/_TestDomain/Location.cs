using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Location : TestDomainBase
  {
    public static Location NewObject ()
    {
      return NewObject<Location> ().With();
    }

    public new static Location GetObject (ObjectID id)
    {
      return GetObject<Location> (id);
    }

    protected Location()
    {
    }

    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
