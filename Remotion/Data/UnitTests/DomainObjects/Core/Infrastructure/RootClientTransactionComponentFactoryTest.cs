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
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;
    private ClientTransactionMock _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create ();
      _fakeConstructedTransaction = new ClientTransactionMock ();
    }

    [Test]
    public void GetParentTransaction ()
    {
      Assert.That (_factory.GetParentTransaction (_fakeConstructedTransaction), Is.Null);
    }

    [Test]
    public void CreateApplicationData ()
    {
      var applicationData = _factory.CreateApplicationData (_fakeConstructedTransaction);

      Assert.That (applicationData, Is.Not.Null);
      Assert.That (applicationData.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateListeners ()
    {
     IEnumerable<IClientTransactionListener> listeners = _factory.CreateListeners (_fakeConstructedTransaction).ToArray();
     Assert.That (listeners, Has.Length.EqualTo (1).And.Some.TypeOf<LoggingClientTransactionListener>());
    }
    
    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf (typeof (RootInvalidDomainObjectManager)));
      Assert.That (((RootInvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);

      Assert.That (result, Is.TypeOf<RootPersistenceStrategy> ());
      Assert.That (((RootPersistenceStrategy) result).TransactionID, Is.EqualTo (_fakeConstructedTransaction.ID));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<RootPersistenceStrategy> ().AddMixin<NullMixin> ().EnterScope ())
      {
        var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);
        Assert.That (Mixin.Get<NullMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var result = _factory.CreateObjectLoader (_fakeConstructedTransaction, persistenceStrategy, eventSink);

      Assert.That (result, Is.TypeOf (typeof (ObjectLoader)));
      Assert.That (((ObjectLoader) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((ObjectLoader) result).ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((ObjectLoader) result).TransactionEventSink, Is.SameAs (eventSink));
      Assert.That (((ObjectLoader) result).EagerFetcher, Is.TypeOf<EagerFetcher> ());

      var eagerFetcher = ((EagerFetcher) ((ObjectLoader) result).EagerFetcher);
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ());
    }

    [Test]
    public void CreateDataManager ()
    {
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var dataManager = (DataManager) _factory.CreateDataManager (_fakeConstructedTransaction, invalidDomainObjectManager, objectLoader);
      Assert.That (dataManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (invalidDomainObjectManager));
      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.SameAs (objectLoader));
      
      var manager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (dataManager);
      Assert.That (manager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (manager.EndPointFactory, Is.TypeOf<RelationEndPointFactory> ());
      Assert.That (manager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent> ());

      var endPointFactory = ((RelationEndPointFactory) manager.EndPointFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (dataManager));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (dataManager));
      Assert.That (endPointFactory.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));

      var collectionEndPointDataKeeperFactory = ((CollectionEndPointDataKeeperFactory) endPointFactory.CollectionEndPointDataKeeperFactory);
      Assert.That (collectionEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (collectionEndPointDataKeeperFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf<VirtualObjectEndPointDataKeeperFactory>());

      var virtualObjectEndPointDataKeeperFactory = ((VirtualObjectEndPointDataKeeperFactory) endPointFactory.VirtualObjectEndPointDataKeeperFactory);
      Assert.That (virtualObjectEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var result = _factory.CreateQueryManager (_fakeConstructedTransaction, persistenceStrategy, objectLoader, dataManager, eventSink);

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (objectLoader));
      Assert.That (((QueryManager) result).DataManager, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));
    }

    [Test]
    [Ignore ("TODO 4352")]
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

      Assert.That (extensions.Count, Is.EqualTo (2));
      Assert.That (extensions[1], Is.SameAs (extensionStub));
      Assert.That (extensions[0], Is.TypeOf<CommitValidationClientTransactionExtension> ());
      
      var validationExtension = (CommitValidationClientTransactionExtension) extensions[0];
      var validator = validationExtension.ValidatorFactory(_fakeConstructedTransaction);
      Assert.That (validator, Is.TypeOf<MandatoryRelationValidator>());
      Assert.That (((MandatoryRelationValidator) validator).DataManager, Is.SameAs (_fakeConstructedTransaction.DataManager));
    }
  }
}