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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

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
    public void GetListenersFromServiceLocator ()
    {
      var clientTransaction = new ClientTransactionMock ();

      var listenerFactoryMock1 = MockRepository.GenerateStrictMock<IClientTransactionListenerFactory>();
      var listenerFactoryMock2 = MockRepository.GenerateStrictMock<IClientTransactionListenerFactory>();
      var listenerStub1 = MockRepository.GenerateStub<IClientTransactionListener>();
      var listenerStub2 = MockRepository.GenerateStub<IClientTransactionListener>();

      listenerFactoryMock1.Expect (mock => mock.CreateClientTransactionListener (clientTransaction)).Return (listenerStub1);
      listenerFactoryMock1.Replay();

      listenerFactoryMock2.Expect (mock => mock.CreateClientTransactionListener (clientTransaction)).Return (listenerStub2);
      listenerFactoryMock2.Replay();

      var serviceLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator>();
      serviceLocatorMock
          .Expect (mock => mock.GetAllInstances<IClientTransactionListenerFactory>())
          .Return (new[] { listenerFactoryMock1, listenerFactoryMock2 });
      serviceLocatorMock.Replay();
      
      IEnumerable<IClientTransactionListener> listeners;
      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        listeners = ClientTransactionComponentFactoryUtility.GetListenersFromServiceLocator (clientTransaction).ToArray();
      }

      serviceLocatorMock.VerifyAllExpectations();
      listenerFactoryMock1.VerifyAllExpectations();
      listenerFactoryMock2.VerifyAllExpectations();

     Assert.That (listeners, Is.EquivalentTo (new[] { listenerStub1, listenerStub2 }));
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
        extensions = ClientTransactionComponentFactoryUtility.CreateExtensionCollectionFromServiceLocator (clientTransaction);
      }

      serviceLocatorMock.VerifyAllExpectations();
      extensionFactoryMock1.VerifyAllExpectations();
      extensionFactoryMock2.VerifyAllExpectations();

     Assert.That (extensions, Is.EquivalentTo (new[] { extensionStub1, extensionStub2 }));
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
      Assert.That (((ObjectLoader) result).ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (((ObjectLoader) result).TransactionEventSink, Is.SameAs (eventSink));
      Assert.That (((ObjectLoader) result).EagerFetcher, Is.TypeOf<EagerFetcher>());

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

      var result = ClientTransactionComponentFactoryUtility.CreateQueryManager (clientTransaction, persistenceStrategy, objectLoader, dataManager, eventSink);

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (objectLoader));
      Assert.That (((QueryManager) result).DataManager, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));
    }
  }
}