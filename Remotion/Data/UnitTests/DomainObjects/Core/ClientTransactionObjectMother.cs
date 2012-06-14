// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionObjectMother
  {
    public static ClientTransaction CreateStrictMock ()
    {
      return CreateStrictMock(new MockRepository ());
    }

    public static ClientTransaction CreateStrictMock (MockRepository mockRepository)
    {
      var componentFactory = RootClientTransactionComponentFactory.Create();
      return mockRepository.StrictMock<ClientTransaction> (componentFactory);
    }

    public static T CreateTransactionWithPersistenceStrategy<T> (IPersistenceStrategy persistenceStrategy) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificPersistenceStrategy (persistenceStrategy);
      return Create<T> (componentFactory);
    }

    public static T CreateTransactionWithQueryManager<T> (IQueryManager queryManager) where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithSpecificQueryManager (queryManager);
      return Create<T> (componentFactory);
    }

    public static T CreateTransactionWithObjectLoaderDecorator<T> (TestComponentFactoryWithObjectLoaderDecorator.DecoratorFactory factory) 
        where T : ClientTransaction
    {
      var componentFactory = new TestComponentFactoryWithObjectLoaderDecorator (factory);
      return Create<T> (componentFactory);
    }

    public static ClientTransaction Create ()
    {
      return ClientTransaction.CreateRootTransaction();
    }

    public static T Create<T> (IClientTransactionComponentFactory componentFactory) where T : ClientTransaction
    {
      return (T) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (T), componentFactory);
    }

    public static T Create<T> (
        ClientTransaction parentTransaction,
        Dictionary<Enum, object> applicationData,
        Func<ClientTransaction, ClientTransaction> cloneFactory,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        ClientTransactionExtensionCollection extensions,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IClientTransactionListenerManager listenerManager,
        IPersistenceStrategy persistenceStrategy,
        IQueryManager queryManager)
      where T : ClientTransaction
    {
      var componentFactoryStub = CreateComponentFactory (
          parentTransaction,
          applicationData,
          cloneFactory,
          dataManager,
          enlistedDomainObjectManager,
          extensions,
          invalidDomainObjectManager,
          listenerManager,
          persistenceStrategy,
          queryManager);

      return Create<T> (componentFactoryStub);
    }

    public static IClientTransactionComponentFactory CreateComponentFactory (
        ClientTransaction parentTransaction,
        Dictionary<Enum, object> applicationData,
        Func<ClientTransaction, ClientTransaction> cloneFactory,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        ClientTransactionExtensionCollection extensions,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IClientTransactionListenerManager listenerManager,
        IPersistenceStrategy persistenceStrategy,
        IQueryManager queryManager)
    {
      var componentFactoryStub = MockRepository.GenerateStub<IClientTransactionComponentFactory>();
      componentFactoryStub.Stub (stub => stub.GetParentTransaction (Arg<ClientTransaction>.Is.Anything)).Return (parentTransaction);
      componentFactoryStub.Stub (stub => stub.CreateApplicationData (Arg<ClientTransaction>.Is.Anything)).Return (applicationData);
      componentFactoryStub.Stub (stub => stub.CreateCloneFactory ()).Return (cloneFactory);
      componentFactoryStub
          .Stub (stub => stub.CreateDataManager(
              Arg<ClientTransaction>.Is.Anything,
              Arg<IClientTransactionEventSink>.Is.Anything, 
              Arg<IInvalidDomainObjectManager>.Is.Anything, 
              Arg<IPersistenceStrategy>.Is.Anything))
          .Return (dataManager);
      componentFactoryStub.Stub (stub => stub.CreateEnlistedObjectManager (Arg<ClientTransaction>.Is.Anything)).Return (enlistedDomainObjectManager);
      componentFactoryStub.Stub (stub => stub.CreateExtensionCollection (Arg<ClientTransaction>.Is.Anything)).Return (extensions);
      componentFactoryStub
          .Stub (stub => stub.CreateInvalidDomainObjectManager (Arg<ClientTransaction>.Is.Anything, Arg<IClientTransactionEventSink>.Is.Anything))
          .Return (invalidDomainObjectManager);
      componentFactoryStub.Stub (stub => stub.CreateListenerManager (Arg<ClientTransaction>.Is.Anything)).Return (listenerManager);
      componentFactoryStub.Stub (stub => stub.CreatePersistenceStrategy (Arg<ClientTransaction>.Is.Anything)).Return (persistenceStrategy);
      componentFactoryStub
          .Stub (stub => stub.CreateQueryManager (
              Arg<ClientTransaction>.Is.Anything,
              Arg<IClientTransactionEventSink>.Is.Anything,
              Arg<IInvalidDomainObjectManager>.Is.Anything,
              Arg<IPersistenceStrategy>.Is.Anything,
              Arg<IDataManager>.Is.Anything))
          .Return (queryManager);
      return componentFactoryStub;
    }

    public static ClientTransaction CreateWithCustomListeners (params IClientTransactionListener[] listeners)
    {
      var componentFactoryPartialMock = MockRepository.GeneratePartialMock<RootClientTransactionComponentFactory>();
      componentFactoryPartialMock
          .Stub (stub => PrivateInvoke.InvokeNonPublicMethod (stub, "CreateListeners", Arg<ClientTransaction>.Is.Anything))
          .Return (listeners);
      componentFactoryPartialMock.Replay ();

      return Create<ClientTransaction> (componentFactoryPartialMock);
    }
  }
}