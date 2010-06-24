using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionObjectMother
  {
    public static ClientTransaction CreatePartialMock ()
    {
      return CreatePartialMock(new MockRepository ());
    }

    public static ClientTransaction CreatePartialMock (MockRepository mockRepository)
    {
      var componentFactory = new RootClientTransactionComponentFactory ();
      return mockRepository.PartialMock<ClientTransaction> (componentFactory);
    }

    public static ClientTransaction CreateStrictMock ()
    {
      return CreateStrictMock(new MockRepository ());
    }

    public static ClientTransaction CreateStrictMock (MockRepository mockRepository)
    {
      var componentFactory = new RootClientTransactionComponentFactory ();
      return mockRepository.StrictMock<ClientTransaction> (componentFactory);
    }

    public static ClientTransaction CreateTransactionWithPersistenceStrategy (IPersistenceStrategy persistenceStrategy)
    {
      var factory = new TestComponentFactoryWithSpecificPersistenceStrategy (persistenceStrategy);
      return (ClientTransaction) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ClientTransaction), factory);
    }
  }
}