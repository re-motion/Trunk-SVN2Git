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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create();
    }

    [Test]
    public void GetParentTransaction ()
    {
      Assert.That (_factory.GetParentTransaction(), Is.Null);
    }

    [Test]
    public void CreateApplicationData ()
    {
      var applicationData = _factory.CreateApplicationData();

      Assert.That (applicationData, Is.Not.Null);
      Assert.That (applicationData.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateExtensions ()
    {
      var extensionCollection = _factory.CreateExtensions();

      Assert.That (extensionCollection, Is.Not.Null);
      Assert.That (((IClientTransactionExtension) extensionCollection).Key, Is.EqualTo ("root"));
      Assert.That (extensionCollection.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var manager = _factory.CreateInvalidDomainObjectManager ();
      Assert.That (manager, Is.TypeOf (typeof (RootInvalidDomainObjectManager)));
      Assert.That (((RootInvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateDataManager ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var dataManager = (DataManager) _factory.CreateDataManager (clientTransaction, invalidDomainObjectManager, objectLoader);
      Assert.That (dataManager.ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (invalidDomainObjectManager));
      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.SameAs (objectLoader));
      
      var manager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (dataManager);
      Assert.That (manager.ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (manager.EndPointFactory, Is.TypeOf<RelationEndPointFactory> ());
      Assert.That (manager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent> ());

      var endPointFactory = ((RelationEndPointFactory) manager.EndPointFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (dataManager));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (dataManager));
      Assert.That (endPointFactory.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));

      var collectionEndPointDataKeeperFactory = ((CollectionEndPointDataKeeperFactory) endPointFactory.CollectionEndPointDataKeeperFactory);
      Assert.That (collectionEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (collectionEndPointDataKeeperFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf<VirtualObjectEndPointDataKeeperFactory>());

      var virtualObjectEndPointDataKeeperFactory = ((VirtualObjectEndPointDataKeeperFactory) endPointFactory.VirtualObjectEndPointDataKeeperFactory);
      Assert.That (virtualObjectEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (clientTransaction));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();

      var result = _factory.CreateQueryManager (clientTransaction, persistenceStrategy, objectLoader, dataManager);

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ObjectLoader, Is.SameAs (objectLoader));
      Assert.That (((QueryManager) result).DataManager, Is.SameAs (dataManager));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (ClientTransactionTestHelper.GetTransactionEventSink (clientTransaction)));
    }

  }
}