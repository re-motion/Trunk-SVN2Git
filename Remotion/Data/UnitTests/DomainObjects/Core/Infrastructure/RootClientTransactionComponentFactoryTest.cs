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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;
    private TestableClientTransaction _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create ();
      _fakeConstructedTransaction = new TestableClientTransaction ();
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
    public void CreateEnlistedDomainObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf<DictionaryBasedEnlistedDomainObjectManager>());
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
      Assert.That (extensions[0], Is.TypeOf<CommitValidationClientTransactionExtension> ());
      Assert.That (extensions[1], Is.SameAs (extensionStub));
      
      var validationExtension = (CommitValidationClientTransactionExtension) extensions[0];
      var validator = validationExtension.ValidatorFactory (_fakeConstructedTransaction);
      Assert.That (validator, Is.TypeOf<MandatoryRelationValidator>());
    }

    [Test]
    public void CreateRelationEndPointManager ()
    {
      var lazyLoader = MockRepository.GenerateStub<ILazyLoader> ();
      var endPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider> ();

      var relationEndPointManager =
          (RelationEndPointManager) PrivateInvoke.InvokeNonPublicMethod (
              _factory,
              "CreateRelationEndPointManager",
              _fakeConstructedTransaction,
              endPointProvider,
              lazyLoader);

      Assert.That (relationEndPointManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<RelationEndPointFactory> ());
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent> ());

      var endPointFactory = ((RelationEndPointFactory) relationEndPointManager.EndPointFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (lazyLoader));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (endPointProvider));
      Assert.That (endPointFactory.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));

      var collectionEndPointDataKeeperFactory = ((CollectionEndPointDataKeeperFactory) endPointFactory.CollectionEndPointDataKeeperFactory);
      Assert.That (collectionEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (collectionEndPointDataKeeperFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf<VirtualObjectEndPointDataKeeperFactory> ());

      var virtualObjectEndPointDataKeeperFactory = ((VirtualObjectEndPointDataKeeperFactory) endPointFactory.VirtualObjectEndPointDataKeeperFactory);
      Assert.That (virtualObjectEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var fakeBasicObjectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var factoryPartialMock = MockRepository.GeneratePartialMock<RootClientTransactionComponentFactory> ();
      factoryPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (
              mock,
              "CreateBasicObjectLoader",
              _fakeConstructedTransaction,
              eventSink,
              persistenceStrategy,
              invalidDomainObjectManager,
              dataManager))
          .Return (fakeBasicObjectLoader);
      factoryPartialMock.Replay ();

      var result = PrivateInvoke.InvokeNonPublicMethod (
          factoryPartialMock, 
          "CreateObjectLoader",
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager);

      Assert.That (result, Is.TypeOf (typeof (EagerFetchingObjectLoaderDecorator)));
      var objectLoader = (EagerFetchingObjectLoaderDecorator) result;
      Assert.That (objectLoader.DecoratedObjectLoader, Is.SameAs (fakeBasicObjectLoader));
      Assert.That (objectLoader.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) objectLoader.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) objectLoader.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ()
              .With.Property<FetchedVirtualObjectRelationDataRegistrationAgent> (a => a.LoadedDataContainerProvider).SameAs (dataManager)
              .And.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) objectLoader.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ()
              .With.Property<FetchedVirtualObjectRelationDataRegistrationAgent> (a => a.LoadedDataContainerProvider).SameAs (dataManager)
              .And.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
    }
  }
}