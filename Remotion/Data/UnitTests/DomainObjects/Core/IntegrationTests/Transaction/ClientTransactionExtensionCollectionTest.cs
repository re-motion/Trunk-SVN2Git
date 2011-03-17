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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionCollectionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ClientTransactionExtensionCollection _collection;
    private ClientTransactionExtensionCollection _collectionWithExtensions;
    private IClientTransactionExtension _extension1;
    private IClientTransactionExtension _extension2;

    private Order _order;
    private DataContainer _dataContainer;
    private PropertyValue _propertyValue;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _collection = new ClientTransactionExtensionCollection ();
      _extension1 = _mockRepository.StrictMock<IClientTransactionExtension> ();
      _extension2 = _mockRepository.StrictMock<IClientTransactionExtension> ();

      _collectionWithExtensions = new ClientTransactionExtensionCollection ();
      _collectionWithExtensions.Add ("Name1", _extension1);
      _collectionWithExtensions.Add ("Name2", _extension2);

      _order = Order.NewObject ();
			_dataContainer = _order.InternalDataContainer;
      _propertyValue = _dataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
    }

    [Test]
    public void Add ()
    {
      Assert.AreEqual (0, _collection.Count);
      _collection.Add ("Name", _extension1);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void Insert ()
    {
      _collection.Add ("Name1", _extension1);
      Assert.AreEqual (1, _collection.Count);
      Assert.AreSame (_extension1, _collection[0]);

      _collection.Insert (0, "Name2", _extension2);
      Assert.AreEqual (2, _collection.Count);
      Assert.AreSame (_extension2, _collection[0]);
      Assert.AreSame (_extension1, _collection[1]);
    }

    [Test]
    public void Remove ()
    {
      _collection.Add ("Name", _extension1);
      Assert.AreEqual (1, _collection.Count);
      _collection.Remove ("Name");
      Assert.AreEqual (0, _collection.Count);
      _collection.Remove ("Name");
      //expectation: no exception
    }

    [Test]
    public void Indexer ()
    {
      _collection.Add ("Name1", _extension1);
      _collection.Add ("Name2", _extension2);
      Assert.AreSame (_extension1, _collection[0]);
      Assert.AreSame (_extension2, _collection[1]);
    }

    [Test]
    public void IndexerWithName ()
    {
      _collection.Add ("Name1", _extension1);
      _collection.Add ("Name2", _extension2);
      Assert.AreSame (_extension1, _collection["Name1"]);
      Assert.AreSame (_extension2, _collection["Name2"]);
    }

    [Test]
    public void IndexOf ()
    {
      _collection.Add ("Name", _extension1);

      Assert.AreEqual (0, _collection.IndexOf ("Name"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "An extension with name 'Name' is already part of the collection.\r\nParameter name: extensionName")]
    public void AddWithDuplicateName ()
    {
      _collection.Add ("Name", _extension1);
      _collection.Add ("Name", _extension2);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "An extension with name 'Name' is already part of the collection.\r\nParameter name: extensionName")]
    public void InsertWithDuplicateName ()
    {
      _collection.Insert (0, "Name", _extension1);
      _collection.Insert (0, "Name", _extension2);
    }

    [Test]
    public void PropertyChanging ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueChanging (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
        _extension2.PropertyValueChanging (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanging (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChanged ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueChanged (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
        _extension2.PropertyValueChanged (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanged (ClientTransactionMock, _dataContainer, _propertyValue, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyReading ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueReading (ClientTransactionMock, _dataContainer, _propertyValue, ValueAccess.Original);
        _extension2.PropertyValueReading (ClientTransactionMock, _dataContainer, _propertyValue, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueReading (ClientTransactionMock, _dataContainer, _propertyValue, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [Explicit ("Performance test")]
    public void PropertyReading_Perf ()
    {
      var coll = new ClientTransactionExtensionCollection ();

      Stopwatch sw = Stopwatch.StartNew ();
      for (int i = 0; i < 100000; ++i)
        coll.PropertyValueReading (ClientTransactionMock, _dataContainer, _propertyValue, ValueAccess.Original);
      sw.Stop ();
      Console.WriteLine (sw.Elapsed);
      Console.WriteLine (sw.ElapsedMilliseconds / 100000.0);
    }

    [Test]
    public void PropertyRead ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueRead (ClientTransactionMock, _dataContainer, _propertyValue, 0, ValueAccess.Original);
        _extension2.PropertyValueRead (ClientTransactionMock, _dataContainer, _propertyValue, 0, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueRead (ClientTransactionMock, _dataContainer, _propertyValue, 0, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanging ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.NewObject ();

      _mockRepository.BackToRecord (_extension1);
      _mockRepository.BackToRecord (_extension2);

      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();

      using (_mockRepository.Ordered ())
      {
        _extension1.RelationChanging (ClientTransactionMock, _order, relationEndPointDefinition, orderTicket, newOrderTicket);
        _extension2.RelationChanging (ClientTransactionMock, _order, relationEndPointDefinition, orderTicket, newOrderTicket);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanging (ClientTransactionMock, _order, relationEndPointDefinition, orderTicket, newOrderTicket);
      
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanged ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationChanged (ClientTransactionMock, _order, relationEndPointDefinition);
        _extension2.RelationChanged (ClientTransactionMock, _order, relationEndPointDefinition);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanged (ClientTransactionMock, _order, relationEndPointDefinition);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.NewObjectCreating (ClientTransactionMock, typeof(Order));
        _extension2.NewObjectCreating (ClientTransactionMock, typeof (Order));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.NewObjectCreating (ClientTransactionMock, typeof (Order));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleting ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectDeleting (ClientTransactionMock, _order);
        _extension2.ObjectDeleting (ClientTransactionMock, _order);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleting (ClientTransactionMock, _order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleted ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectDeleted (ClientTransactionMock, _order);
        _extension2.ObjectDeleted (ClientTransactionMock, _order);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleted (ClientTransactionMock, _order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committing ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Committing (ClientTransactionMock, data);
        _extension2.Committing (ClientTransactionMock, data);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committing (ClientTransactionMock, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committed ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Committed (ClientTransactionMock, data);
        _extension2.Committed (ClientTransactionMock, data);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committed (ClientTransactionMock, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RollingBack ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.RollingBack (ClientTransactionMock, data);
        _extension2.RollingBack (ClientTransactionMock, data);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RollingBack (ClientTransactionMock, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RolledBack ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.RolledBack (ClientTransactionMock, data);
        _extension2.RolledBack (ClientTransactionMock, data);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RolledBack (ClientTransactionMock, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var loadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectsLoaded (ClientTransactionMock, loadedDomainObjects);
        _extension2.ObjectsLoaded (ClientTransactionMock, loadedDomainObjects);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsLoaded (ClientTransactionMock, loadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoading ()
    {
      var objectIDs = new List<ObjectID> { _order.ID }.AsReadOnly ();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsLoading (ClientTransactionMock, objectIDs));
        _extension2.Expect (mock => mock.ObjectsLoading (ClientTransactionMock, objectIDs));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsLoading (ClientTransactionMock, objectIDs);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsUnloaded (ClientTransactionMock, unloadedDomainObjects));
        _extension2.Expect (mock => mock.ObjectsUnloaded (ClientTransactionMock, unloadedDomainObjects));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsUnloaded (ClientTransactionMock, unloadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsUnloading ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsUnloading (ClientTransactionMock, unloadedDomainObjects));
        _extension2.Expect (mock => mock.ObjectsUnloading (ClientTransactionMock, unloadedDomainObjects));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsUnloading (ClientTransactionMock, unloadedDomainObjects);

      _mockRepository.VerifyAll ();
    }


    [Test]
    public void FilterQueryResult ()
    {
      var originalResult = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new Order[0]);
      var newResult1 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order1) });
      var newResult2 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order2) });

      _extension1.Expect (mock => mock.FilterQueryResult (ClientTransactionMock, originalResult)).Return (newResult1);
      _extension2.Expect (mock => mock.FilterQueryResult (ClientTransactionMock, newResult1)).Return (newResult2);

      _extension1.Replay ();
      _extension2.Replay ();

      var finalResult = _collectionWithExtensions.FilterQueryResult (ClientTransactionMock, originalResult);
      Assert.That (finalResult, Is.SameAs (newResult2));

      _extension1.VerifyAllExpectations ();
      _extension2.VerifyAllExpectations ();
    }

    [Test]
    public void RelationReading ()
    {
      IRelationEndPointDefinition endPointDefinition = _order.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition;
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationReading (ClientTransactionMock, _order, endPointDefinition, ValueAccess.Current);
        _extension2.RelationReading (ClientTransactionMock, _order, endPointDefinition, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationReading (ClientTransactionMock, _order, endPointDefinition, ValueAccess.Current);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToOneRelation ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      IRelationEndPointDefinition endPointDefinition = _order.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition;
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderTicket, ValueAccess.Original);
        _extension2.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderTicket, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToManyRelation ()
    {
      var orderItems = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (_order.OrderItems);
      IRelationEndPointDefinition endPointDefinition = _order.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition;
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderItems, ValueAccess.Original);
        _extension2.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderItems, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (ClientTransactionMock, _order, endPointDefinition, orderItems, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Subtransactions ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();

      using (_mockRepository.Ordered ())
      {
        _extension1.SubTransactionCreating (ClientTransactionMock);
        _extension2.SubTransactionCreating (ClientTransactionMock);
        _extension1.SubTransactionCreated (ClientTransactionMock, subTransaction);
        _extension2.SubTransactionCreated (ClientTransactionMock, subTransaction);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.SubTransactionCreating (ClientTransactionMock);
      _collectionWithExtensions.SubTransactionCreated (ClientTransactionMock, subTransaction);

      _mockRepository.VerifyAll ();
    }

  }
}
