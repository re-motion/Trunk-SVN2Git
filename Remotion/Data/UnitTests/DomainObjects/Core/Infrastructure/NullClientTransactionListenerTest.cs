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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class NullClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private IClientTransactionListener _listener;
    private DataContainer _dataContainer;
    private Client _domainObject;
    private PropertyValue _propertyValue;
    private RelationEndPoint _relationEndPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _listener = NullClientTransactionListener.Instance;
      _domainObject = DomainObjectMother.CreateObjectInTransaction<Client> (ClientTransactionMock);
      _dataContainer = _domainObject.GetInternalDataContainerForTransaction (ClientTransactionMock);
      _propertyValue = _dataContainer.PropertyValues[0];
      _relationEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap[_dataContainer.AssociatedRelationEndPointIDs[0]];
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_listener.IsNull, Is.True);
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      Assert.That (
          Serializer.SerializeAndDeserialize (NullClientTransactionListener.Instance), 
          Is.SameAs (NullClientTransactionListener.Instance));
    }

    [Test]
    public void RootTransactionCreating ()
    {
      _listener.TransactionInitializing (ClientTransactionMock);
    }

    [Test]
    public void TransactionDiscarding ()
    {
      _listener.TransactionDiscarding (ClientTransactionMock);
    }

    [Test]
    public void SubTransactionCreating ()
    {
      _listener.SubTransactionCreating (ClientTransactionMock);
    }

    [Test]
    public void SubTransactionCreated ()
    {
      _listener.SubTransactionCreated (ClientTransactionMock, ClientTransactionMock);
    }

    [Test]
    public void NewObjectCreating ()
    {
      _listener.NewObjectCreating (ClientTransactionMock, _domainObject.GetPublicDomainObjectType(), _domainObject);
    }

    [Test]
    public void ObjectsLoading ()
    {
      _listener.ObjectsLoading (ClientTransactionMock, new ReadOnlyCollection<ObjectID> (new ObjectID[0]));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _listener.ObjectsLoaded (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void ObjectsUnloading ()
    {
      _listener.ObjectsUnloading (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      _listener.ObjectsUnloaded (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void ObjectDeleting ()
    {
      _listener.ObjectDeleting (ClientTransactionMock, _domainObject);
    }

    [Test]
    public void ObjectDeleted ()
    {
      _listener.ObjectDeleted (ClientTransactionMock, _domainObject);
    }

    [Test]
    public void PropertyValueReading ()
    {
      _listener.PropertyValueReading (ClientTransactionMock, _dataContainer, _propertyValue, ValueAccess.Current);
    }

    [Test]
    public void PropertyValueRead ()
    {
      _listener.PropertyValueRead (ClientTransactionMock, _dataContainer, _propertyValue, 0, ValueAccess.Current);
    }

    [Test]
    public void PropertyValueChanging ()
    {
      _listener.PropertyValueChanging (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
    }

    [Test]
    public void PropertyValueChanged ()
    {
      _listener.PropertyValueChanged (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
    }

    [Test]
    public void RelationReading ()
    {
      _listener.RelationReading (ClientTransactionMock, _domainObject, _relationEndPoint.Definition, ValueAccess.Current);
    }

    [Test]
    public void RelationRead_SingleValue ()
    {
      _listener.RelationRead (ClientTransactionMock, _domainObject, "Relation", _domainObject, ValueAccess.Current);
    }

    [Test]
    public void RelationRead_MultiValue ()
    {
      _listener.RelationRead (ClientTransactionMock, _domainObject, "Relation", new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection()), ValueAccess.Current);
    }

    [Test]
    public void RelationChanging ()
    {
      _listener.RelationChanging (ClientTransactionMock, _domainObject, _relationEndPoint.Definition, _domainObject, _domainObject);
    }

    [Test]
    public void RelationChanged ()
    {
      _listener.RelationChanged (ClientTransactionMock, _domainObject, _relationEndPoint.Definition);
    }

    [Test]
    public void FilterQueryResult ()
    {
      var querResult = new QueryResult<DomainObject> (MockRepository.GenerateStub<IQuery>(), new DomainObject[0]);
      Assert.That (_listener.FilterQueryResult (ClientTransactionMock, querResult), Is.SameAs (querResult));
    }

    [Test]
    public void TransactionCommitting ()
    {
      _listener.TransactionCommitting (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void TransactionCommitted ()
    {
      _listener.TransactionCommitted (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void TransactionRollingBack ()
    {
      _listener.TransactionRollingBack (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void TransactionRolledBack ()
    {
      _listener.TransactionRolledBack (ClientTransactionMock, new ReadOnlyCollection<DomainObject> (new DomainObject[0]));
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      _listener.RelationEndPointMapRegistering (ClientTransactionMock, _relationEndPoint);
    }

    [Test]
    public void RelationEndPointMapUnregistering ()
    {
      _listener.RelationEndPointMapUnregistering (ClientTransactionMock, _relationEndPoint.ID);
    }

    [Test]
    public void RelationEndPointUnloading ()
    {
      _listener.RelationEndPointUnloading (ClientTransactionMock, _relationEndPoint);
    }

    [Test]
    public void DataManagerMarkingObjectDiscarded ()
    {
      _listener.DataManagerMarkingObjectInvalid (ClientTransactionMock, _domainObject.ID);
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      _listener.DataContainerMapRegistering (ClientTransactionMock, _dataContainer);
    }

    [Test]
    public void DataContainerMapUnregistering ()
    {
      _listener.DataContainerMapUnregistering (ClientTransactionMock, _dataContainer);
    }
  }
}