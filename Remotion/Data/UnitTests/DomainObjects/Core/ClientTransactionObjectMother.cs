using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
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

    public static ClientTransaction Create (
        ClientTransaction parentTransaction,
        Dictionary<Enum, object> applicationData,
        Func<ClientTransaction, ClientTransaction> clineFactory,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        ClientTransactionExtensionCollection extensions,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        CompoundClientTransactionListener[] listeners,
        IObjectLoader objectLoader,
        IPersistenceStrategy persistenceStrategy)
    {
      var componentFactoryStub = CreateComponentFactory (
          parentTransaction,
          applicationData,
          clineFactory,
          dataManager,
          enlistedDomainObjectManager,
          extensions,
          invalidDomainObjectManager,
          listeners,
          objectLoader,
          persistenceStrategy);

      return Create<ClientTransaction> (componentFactoryStub);
    }

    public static IClientTransactionComponentFactory CreateComponentFactory (
        ClientTransaction parentTransaction,
        Dictionary<Enum, object> applicationData,
        Func<ClientTransaction, ClientTransaction> clineFactory,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        ClientTransactionExtensionCollection extensions,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        CompoundClientTransactionListener[] listeners,
        IObjectLoader objectLoader,
        IPersistenceStrategy persistenceStrategy)
    {
      var componentFactoryStub = MockRepository.GenerateStub<IClientTransactionComponentFactory>();
      componentFactoryStub.Stub (stub => stub.GetParentTransaction()).Return (parentTransaction);
      componentFactoryStub.Stub (stub => stub.CreateApplicationData ()).Return (applicationData);
      componentFactoryStub.Stub (stub => stub.CreateCloneFactory ()).Return (clineFactory);
      componentFactoryStub
          .Stub (stub => stub.CreateDataManager(
              Arg<ClientTransaction>.Is.Anything, 
              Arg<IInvalidDomainObjectManager>.Is.Anything, 
              Arg<IObjectLoader>.Is.Anything))
          .Return (dataManager);
      componentFactoryStub.Stub (stub => stub.CreateEnlistedObjectManager ()).Return (enlistedDomainObjectManager);
      componentFactoryStub.Stub (stub => stub.CreateExtensions ()).Return (extensions);
      componentFactoryStub.Stub (stub => stub.CreateInvalidDomainObjectManager ()).Return (invalidDomainObjectManager);
      componentFactoryStub.Stub (stub => stub.CreateListeners (Arg<ClientTransaction>.Is.Anything)).Return (listeners);
      componentFactoryStub
          .Stub (stub => stub.CreateObjectLoader(
              Arg<ClientTransaction>.Is.Anything, 
              Arg<IPersistenceStrategy>.Is.Anything, 
              Arg<IClientTransactionListener>.Is.Anything))
          .Return (objectLoader);
      componentFactoryStub.Stub (stub => stub.CreatePersistenceStrategy (Arg<Guid>.Is.Anything)).Return (persistenceStrategy);
      return componentFactoryStub;
    }
  }
}