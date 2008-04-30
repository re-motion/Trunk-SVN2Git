using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Client : TestDomainBase
  {
    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    public new static Client GetObject (ObjectID id)
    {
      return GetObject<Client> (id);
    }

    protected Client ()
    {
    }

    public abstract Client ParentClient { get; set; }
  }
}
