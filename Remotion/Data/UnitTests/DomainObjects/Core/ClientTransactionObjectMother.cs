using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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

    public static T CreateTransactionWithPersistenceStrategy<T> (IPersistenceStrategy persistenceStrategy) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificPersistenceStrategy (persistenceStrategy);
      return Create<T> (componentFactory);
    }

    public static T CreateTransactionWithObjectLoader<T> (
        Func<ClientTransaction, IPersistenceStrategy, IClientTransactionListener, IObjectLoader> factory) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificObjectLoader (factory);
      return Create<T> (componentFactory);
    }

    public static T Create<T> (IClientTransactionComponentFactory componentFactory) where T : ClientTransaction
    {
      return (T) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (T), componentFactory);
    }

  }
}