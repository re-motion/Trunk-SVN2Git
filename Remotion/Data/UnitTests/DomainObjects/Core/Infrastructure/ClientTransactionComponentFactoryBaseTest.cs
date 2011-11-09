// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ClientTransactionComponentFactoryBaseTest
  {
    private TestableClientTransactionComponentFactoryBase _factory;
    private ClientTransaction _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = new TestableClientTransactionComponentFactoryBase ();
      _fakeConstructedTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void CreateListeners ()
    {
      IEnumerable<IClientTransactionListener> listeners = _factory.CreateListeners (_fakeConstructedTransaction).ToArray ();
      Assert.That (listeners, Has.Length.EqualTo (1).And.Some.TypeOf<LoggingClientTransactionListener> ());
    }

    [Test]
    public void CreateDataManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy>();

      var fakeEndPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      var fakeLazyLoader = MockRepository.GenerateStub<ILazyLoader> ();
      var fakeRelationEndPointManager = MockRepository.GenerateStub<IRelationEndPointManager> ();
      var fakeObjectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      DataManager endPointProviderDataManager = null;
      DataManager lazyLoaderDataManager = null;
      DelegatingDataManager objectLoaderDataManager = null;

      var factoryPartialMock = MockRepository.GeneratePartialMock<TestableClientTransactionComponentFactoryBase>();
      factoryPartialMock
          .Expect (mock => mock.CallGetEndPointProvider (Arg<DataManager>.Is.NotNull))
          .Return (fakeEndPointProvider)
          .WhenCalled (mi => endPointProviderDataManager = (DataManager) mi.Arguments[0]);
      factoryPartialMock
          .Expect (mock => mock.CallGetLazyLoader (Arg<DataManager>.Is.NotNull))
          .Return (fakeLazyLoader)
          .WhenCalled (mi => lazyLoaderDataManager = (DataManager) mi.Arguments[0]);
      factoryPartialMock
          .Expect (
              mock => mock.CallCreateRelationEndPointManager (_fakeConstructedTransaction, fakeEndPointProvider, fakeLazyLoader))
          .Return (fakeRelationEndPointManager);
      factoryPartialMock
          .Expect (
              mock => mock.CallCreateObjectLoader (
                  Arg.Is (_fakeConstructedTransaction),
                  Arg.Is (eventSink),
                  Arg.Is (persistenceStrategy),
                  Arg.Is (invalidDomainObjectManager),
                  Arg<DelegatingDataManager>.Is.TypeOf))
          .Return (fakeObjectLoader)
          .WhenCalled (mi => objectLoaderDataManager = (DelegatingDataManager) mi.Arguments[4]);
      factoryPartialMock.Replay();

      var dataManager = (DataManager) factoryPartialMock.CreateDataManager (_fakeConstructedTransaction, eventSink, invalidDomainObjectManager, persistenceStrategy);

      factoryPartialMock.VerifyAllExpectations();
      Assert.That (endPointProviderDataManager, Is.SameAs (dataManager));
      Assert.That (lazyLoaderDataManager, Is.SameAs (dataManager));
      Assert.That (objectLoaderDataManager.InnerDataManager, Is.SameAs (dataManager));

      Assert.That (dataManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (invalidDomainObjectManager));
      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.SameAs (fakeObjectLoader));
      Assert.That (DataManagerTestHelper.GetRelationEndPointManager (dataManager), Is.SameAs (fakeRelationEndPointManager));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var fakeObjectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var factoryPartialMock = MockRepository.GeneratePartialMock<TestableClientTransactionComponentFactoryBase> ();
      factoryPartialMock
          .Expect (
              mock => mock.CallCreateObjectLoader (_fakeConstructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager))
          .Return (fakeObjectLoader);
      factoryPartialMock.Replay ();

      var result = factoryPartialMock.CreateQueryManager (_fakeConstructedTransaction, eventSink, invalidDomainObjectManager, persistenceStrategy, dataManager);

      factoryPartialMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (fakeObjectLoader));
    }

    [Test]
    public void CreateExtensionCollection ()
    {
      var extensionFactoryMock = MockRepository.GenerateStrictMock<IClientTransactionExtensionFactory> ();
      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension> ();
      extensionStub.Stub (stub => stub.Key).Return ("stub1");

      extensionFactoryMock.Expect (mock => mock.CreateClientTransactionExtensions (_fakeConstructedTransaction)).Return (new[] { extensionStub });
      extensionFactoryMock.Replay ();

      var serviceLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator> ();
      serviceLocatorMock
          .Expect (mock => mock.GetAllInstances<IClientTransactionExtensionFactory> ())
          .Return (new[] { extensionFactoryMock });
      serviceLocatorMock.Replay ();

      ClientTransactionExtensionCollection extensions;
      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        extensions = _factory.CreateExtensionCollection (_fakeConstructedTransaction);
      }

      serviceLocatorMock.VerifyAllExpectations ();
      extensionFactoryMock.VerifyAllExpectations ();

      Assert.That (extensions.Count, Is.EqualTo (1));
      Assert.That (extensions[0], Is.SameAs (extensionStub));
    }

    [Test]
    public void GetLazyLoader ()
    {
      var dataManager = ClientTransactionTestHelper.GetDataManager (_fakeConstructedTransaction);

      var result = _factory.CallGetLazyLoader (dataManager);

      Assert.That (result, Is.SameAs (result));
    }

    [Test]
    public void GetEndPointProvider ()
    {
      var dataManager = MockRepository.GenerateStub<IDataManager>();

      var result = _factory.CallGetEndPointProvider (dataManager);

      Assert.That (result, Is.SameAs (dataManager));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var result = _factory.CallCreateObjectLoader (
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager);

      Assert.That (result, Is.TypeOf (typeof (ObjectLoader)));
      var objectLoader = (ObjectLoader) result;
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher> ());
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent> ()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));
      Assert.That (objectLoader.DataContainerLifetimeManager, Is.SameAs (dataManager));

      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider> ());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      Assert.That (loadedObjectDataProvider.LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));

      var eagerFetcher = ((EagerFetcher) objectLoader.EagerFetcher);
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ()
              .With.Property<FetchedVirtualObjectRelationDataRegistrationAgent> (a => a.LoadedDataContainerProvider).SameAs (dataManager)
              .And.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ()
              .With.Property<FetchedVirtualObjectRelationDataRegistrationAgent> (a => a.LoadedDataContainerProvider).SameAs (dataManager)
              .And.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
    }
  }
}