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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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
  public class ClientTransactionComponentFactoryUtilityTest
  {
    [Test]
    public void CreateApplicationData ()
    {
      var applicationData = ClientTransactionComponentFactoryUtility.CreateApplicationData();

      Assert.That (applicationData, Is.Not.Null);
      Assert.That (applicationData.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateExtensionCollectionFromServiceLocator ()
    {
      var clientTransaction = new ClientTransactionMock ();

      var extensionFactoryMock1 = MockRepository.GenerateStrictMock<IClientTransactionExtensionFactory>();
      var extensionFactoryMock2 = MockRepository.GenerateStrictMock<IClientTransactionExtensionFactory>();
      var extensionStub1 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub1.Stub (stub => stub.Key).Return ("stub1");
      var extensionStub2 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub2.Stub (stub => stub.Key).Return ("stub2");

      var fixedExtensionStub1 = MockRepository.GenerateStub<IClientTransactionExtension> ();
      fixedExtensionStub1.Stub (stub => stub.Key).Return ("fixed1");
      var fixedExtensionStub2 = MockRepository.GenerateStub<IClientTransactionExtension> ();
      fixedExtensionStub2.Stub (stub => stub.Key).Return ("fixed2");

      extensionFactoryMock1.Expect (mock => mock.CreateClientTransactionExtensions (clientTransaction)).Return (new[] { extensionStub1 });
      extensionFactoryMock1.Replay();

      extensionFactoryMock2.Expect (mock => mock.CreateClientTransactionExtensions (clientTransaction)).Return (new[] { extensionStub2 });
      extensionFactoryMock2.Replay();

      var serviceLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator>();
      serviceLocatorMock
          .Expect (mock => mock.GetAllInstances<IClientTransactionExtensionFactory>())
          .Return (new[] { extensionFactoryMock1, extensionFactoryMock2 });
      serviceLocatorMock.Replay();
      
      ClientTransactionExtensionCollection extensions;
      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        extensions = ClientTransactionComponentFactoryUtility.CreateExtensionCollectionFromServiceLocator (
            clientTransaction, 
            fixedExtensionStub1, 
            fixedExtensionStub2);
      }

      serviceLocatorMock.VerifyAllExpectations();
      extensionFactoryMock1.VerifyAllExpectations();
      extensionFactoryMock2.VerifyAllExpectations();

     Assert.That (extensions, Is.EquivalentTo (new[] { fixedExtensionStub1, fixedExtensionStub2, extensionStub1, extensionStub2 }));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var result = ClientTransactionComponentFactoryUtility.CreateObjectLoader (clientTransaction, persistenceStrategy, eventSink);

      Assert.That (result, Is.TypeOf (typeof (ObjectLoader)));
      Assert.That (((ObjectLoader) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((ObjectLoader) result).EagerFetcher, Is.TypeOf<EagerFetcher>());
      Assert.That (((ObjectLoader) result).LoadedObjectDataRegistrationAgent, Is.TypeOf<LoadedObjectDataRegistrationAgent>()
          .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (clientTransaction)
          .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));

      var eagerFetcher = ((EagerFetcher) ((ObjectLoader) result).EagerFetcher);
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent>());
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();

      var result = ClientTransactionComponentFactoryUtility.CreateQueryManager (
          clientTransaction,
          persistenceStrategy,
          objectLoader, 
          dataManager,
          invalidDomainObjectManager,
          eventSink);

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (objectLoader));
      Assert.That (((QueryManager) result).DataContainerLifetimeManager, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).VirtualEndPointProvider, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).AlreadyLoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider>()
          .With.Property ((LoadedObjectDataProvider provider) => provider.LoadedDataContainerProvider).SameAs (dataManager)
          .With.Property ((LoadedObjectDataProvider provider) => provider.InvalidDomainObjectManager).SameAs (invalidDomainObjectManager));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));
    }
  }
}