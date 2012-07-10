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
    public void CreateEventBroker ()
    {
      var fakeListener = MockRepository.GenerateStub<IClientTransactionListener>();

      var factoryPartialMock = MockRepository.GeneratePartialMock<TestableClientTransactionComponentFactoryBase> ();
      factoryPartialMock.Stub (stub => stub.CallCreateListeners (_fakeConstructedTransaction)).Return (new[] { fakeListener });

      var result = factoryPartialMock.CreateEventBroker (_fakeConstructedTransaction);

      Assert.That (
          result,
          Is.TypeOf<ClientTransactionEventBroker>()
              .With.Property<ClientTransactionEventBroker> (m => m.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .And.Property<ClientTransactionEventBroker> (m => m.EventDistributor).TypeOf<ClientTransactionEventDistributor>()
              .And.Property<ClientTransactionEventBroker> (m => m.Listeners).EqualTo (new[] { fakeListener }));
    }

    [Test]
    public void CreateListeners ()
    {
      IEnumerable<IClientTransactionListener> listeners = _factory.CallCreateListeners (_fakeConstructedTransaction).ToArray ();
      Assert.That (
          listeners,
          Has
              .Length.EqualTo (3)
              .And.Some.TypeOf<ReadOnlyClientTransactionListener> ()
              .And.Some.TypeOf<LoggingClientTransactionListener> ()
              .And.Some.TypeOf<NewObjectHierarchyInvalidationClientTransactionListener> ());
    }

    [Test]
    public void CreateDataManager ()
    {
      var fakeEventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      var fakeInvalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var fakePersistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy>();

      var fakeDataContainerEventListener = MockRepository.GenerateStub<IDataContainerEventListener> ();
      var fakeEndPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      var fakeLazyLoader = MockRepository.GenerateStub<ILazyLoader> ();
      var fakeRelationEndPointManager = MockRepository.GenerateStub<IRelationEndPointManager> ();
      var fakeObjectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      DelegatingDataManager endPointProviderDataManager = null;
      DelegatingDataManager lazyLoaderDataManager = null;
      DelegatingDataManager objectLoaderDataManager = null;

      var factoryPartialMock = MockRepository.GeneratePartialMock<TestableClientTransactionComponentFactoryBase>();
      factoryPartialMock
          .Expect (mock => mock.CallCreateDataContainerEventListener (fakeEventSink))
          .Return (fakeDataContainerEventListener);
      factoryPartialMock
          .Expect (mock => mock.CallGetEndPointProvider (Arg<DelegatingDataManager>.Is.TypeOf))
          .Return (fakeEndPointProvider)
          .WhenCalled (mi => endPointProviderDataManager = (DelegatingDataManager) mi.Arguments[0]);
      factoryPartialMock
          .Expect (mock => mock.CallGetLazyLoader (Arg<DelegatingDataManager>.Is.TypeOf))
          .Return (fakeLazyLoader)
          .WhenCalled (mi => lazyLoaderDataManager = (DelegatingDataManager) mi.Arguments[0]);
      factoryPartialMock
          .Expect (
              mock => mock.CallCreateRelationEndPointManager (_fakeConstructedTransaction, fakeEndPointProvider, fakeLazyLoader, fakeEventSink))
          .Return (fakeRelationEndPointManager);
      factoryPartialMock
          .Expect (
              mock => mock.CallCreateObjectLoader (
                  Arg.Is (_fakeConstructedTransaction),
                  Arg.Is (fakeEventSink),
                  Arg.Is (fakePersistenceStrategy),
                  Arg.Is (fakeInvalidDomainObjectManager),
                  Arg<DelegatingDataManager>.Is.TypeOf))
          .Return (fakeObjectLoader)
          .WhenCalled (mi => objectLoaderDataManager = (DelegatingDataManager) mi.Arguments[4]);
      factoryPartialMock.Replay();

      var dataManager = (DataManager) factoryPartialMock.CreateDataManager (
          _fakeConstructedTransaction, 
          fakeEventSink, 
          fakeInvalidDomainObjectManager, 
          fakePersistenceStrategy);

      factoryPartialMock.VerifyAllExpectations();
      Assert.That (endPointProviderDataManager.InnerDataManager, Is.SameAs (dataManager));
      Assert.That (lazyLoaderDataManager.InnerDataManager, Is.SameAs (dataManager));
      Assert.That (objectLoaderDataManager.InnerDataManager, Is.SameAs (dataManager));

      Assert.That (dataManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (dataManager.TransactionEventSink, Is.SameAs (fakeEventSink));
      Assert.That (dataManager.DataContainerEventListener, Is.SameAs (fakeDataContainerEventListener));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (fakeInvalidDomainObjectManager));
      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.SameAs (fakeObjectLoader));
      Assert.That (DataManagerTestHelper.GetRelationEndPointManager (dataManager), Is.SameAs (fakeRelationEndPointManager));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

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
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (fakeObjectLoader));
    }

    [Test]
    public void CreateCommitRollbackAgent ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      
      var result = _factory.CreateCommitRollbackAgent (_fakeConstructedTransaction, eventSink, persistenceStrategy, dataManager);

      Assert.That (result, Is.TypeOf<CommitRollbackAgent>());
      Assert.That (((CommitRollbackAgent) result).ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((CommitRollbackAgent) result).EventSink, Is.SameAs (eventSink));
      Assert.That (((CommitRollbackAgent) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((CommitRollbackAgent) result).DataManager, Is.SameAs (dataManager));
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
  }
}