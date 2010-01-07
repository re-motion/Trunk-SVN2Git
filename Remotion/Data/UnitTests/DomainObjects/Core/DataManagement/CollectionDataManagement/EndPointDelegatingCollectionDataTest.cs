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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class EndPointDelegatingCollectionDataTest : ClientTransactionBaseTest
  {
    private Order _owningOrder;

    private ICollectionEndPoint _collectionEndPointMock;
    private IDomainObjectCollectionData _actualDataStub;
    private CompositeRelationModification _compositeModificationMock;
    private IRelationEndPointModification _modificationStub;

    private EndPointDelegatingCollectionData _data;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;

    public override void SetUp ()
    {
      base.SetUp();

      _owningOrder = Order.GetObject (DomainObjectIDs.Order1);

      _collectionEndPointMock = MockRepository.GenerateMock<ICollectionEndPoint>();
      StubCollectionEndPoint(_collectionEndPointMock, ClientTransactionMock, _owningOrder);

      _actualDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();

      _modificationStub = MockRepository.GenerateStub<IRelationEndPointModification> ();
      _compositeModificationMock = MockRepository.GenerateMock<CompositeRelationModification> (new[] { new IRelationEndPointModification[0] });

      _data = new EndPointDelegatingCollectionData (_collectionEndPointMock, _actualDataStub);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);

      ClientTransactionScope.EnterNullScope (); // no active transaction
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave ();
      base.TearDown ();
    }

    [Test]
    public void Count ()
    {
      PrepareActualDataContents (_orderItem1);

      Assert.That (_data.Count, Is.EqualTo (1));
    }

    [Test]
    public void RequiredItemType ()
    {
      _actualDataStub.Stub (mock => mock.RequiredItemType).Return (typeof (Computer));

      Assert.That (_data.RequiredItemType, Is.SameAs (typeof (Computer)));
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (_data.AssociatedEndPoint, Is.SameAs (_collectionEndPointMock));
    }

    [Test]
    public void GetUndecoratedDataStore ()
    {
      var dataStoreStub = new DomainObjectCollectionData ();
      _actualDataStub.Stub (mock => mock.GetUndecoratedDataStore ()).Return (dataStoreStub);

      Assert.That (_data.GetUndecoratedDataStore (), Is.SameAs (dataStoreStub));
    }

    [Test]
    public void ContainsObjectID ()
    {
      PrepareActualDataContents (_orderItem1);

      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.OrderItem2), Is.False);
    }

    [Test]
    public void GetObject_Index ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.GetObject (1), Is.SameAs (_orderItem2));
    }

    [Test]
    public void GetObject_ID ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.GetObject (DomainObjectIDs.OrderItem1), Is.SameAs (_orderItem1));
    }

    [Test]
    public void IndexOf ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      Assert.That (_data.IndexOf (DomainObjectIDs.OrderItem2), Is.EqualTo (1));
    }

    [Test]
    public void GetEnumerator ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      IEnumerable<DomainObject> dataAsEnumerable = _data;
      Assert.That (dataAsEnumerable.ToArray(), Is.EqualTo (new[] { _orderItem1, _orderItem2, _orderItem3 }));
    }

    [Test]
    public void Clear ()
    {
      PrepareActualDataContents (_orderItem1, _orderItem2, _orderItem3);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem3)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem2)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);

      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      _modificationStub.Stub (stub => stub.CreateRelationModification()).Return (_compositeModificationMock);

      _compositeModificationMock.Expect (mock => mock.ExecuteAllSteps ()).Repeat.Times (3);
      _compositeModificationMock.Replay ();

      _data.Clear();

      _collectionEndPointMock.VerifyAllExpectations ();
      _compositeModificationMock.VerifyAllExpectations ();
    }

    [Test]
    public void Clear_WithoutItems ()
    {
      PrepareActualDataContents ();

      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();

      _data.Clear ();

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateInsertModification (_orderItem1, 17)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateRelationModification ()).Return (_compositeModificationMock);

      _data.Insert (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _compositeModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Insert_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Insert (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Insert (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Insert (17, relatedObject));
    }

    [Test]
    public void Remove ()
    {
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateRelationModification ()).Return (_compositeModificationMock);

      var result = _data.Remove (_orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _compositeModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ObjectNotContained ()
    {
      bool result = _data.Remove (_orderItem1);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveModification (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch ());

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Remove (relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Remove (deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject));
    }

    [Test]
    public void Remove_ID ()
    {
      PrepareActualDataContents (_orderItem1);

      _collectionEndPointMock.Expect (mock => mock.CreateRemoveModification (_orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateRelationModification ()).Return (_compositeModificationMock);

      var result = _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.VerifyAllExpectations ();
      _compositeModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_ObjectNotContained ()
    {
      var result = _data.Remove (_orderItem1.ID);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.CreateRemoveModification (Arg<DomainObject>.Is.Anything));
      _collectionEndPointMock.AssertWasCalled (mock => mock.Touch ());

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID_ChecksErrorConditions ()
    {
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Remove (relatedObject.ID));
    }

    [Test]
    public void Replace ()
    {
      _collectionEndPointMock.Expect (mock => mock.CreateReplaceModification (17, _orderItem1)).Return (_modificationStub);
      _collectionEndPointMock.Expect (mock => mock.Touch ());
      _collectionEndPointMock.Replay ();
      _modificationStub.Stub (stub => stub.CreateRelationModification ()).Return (_compositeModificationMock);

      _data.Replace (17, _orderItem1);

      _collectionEndPointMock.VerifyAllExpectations ();
      _compositeModificationMock.AssertWasCalled (mock => mock.ExecuteAllSteps ());
    }

    [Test]
    public void Replace_ChecksErrorConditions ()
    {
      CheckClientTransactionDiffersException ((data, relatedObjectInOtherTransaction) => data.Replace (17, relatedObjectInOtherTransaction));
      CheckObjectDeletedException ((data, deletedRelatedObject) => data.Replace (17, deletedRelatedObject));
      CheckOwningObjectDeletedException ((data, relatedObject) => data.Replace (17, relatedObject));
    }

    private T CreateDomainObjectInTransaction<T> (ClientTransaction transaction) where T : DomainObject
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T), ParamList.Empty);
      }
    }

    private ICollectionEndPoint CreateCollectionEndPointStub (ClientTransaction clientTransaction, Order owningOrder)
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      StubCollectionEndPoint (endPointStub, clientTransaction, owningOrder);
      return endPointStub;
    }

    private void StubCollectionEndPoint (ICollectionEndPoint endPointStub, ClientTransaction clientTransaction, Order owningOrder)
    {
      endPointStub.Stub (stub => stub.ClientTransaction).Return (clientTransaction);
      var relationEndPointDefinition = owningOrder.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      endPointStub.Stub (mock => mock.ObjectID).Return (owningOrder.ID);
      endPointStub.Stub (mock => mock.Definition).Return (relationEndPointDefinition);
    }

    private void CheckClientTransactionDiffersException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      var orderItemInOtherTransaction = CreateDomainObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());
      try
      {
        action (_data, orderItemInOtherTransaction);
        Assert.Fail ("Expected ClientTransactionsDifferException");
      }
      catch (ClientTransactionsDifferException)
      {
        // ok
      }
    }

    private void CheckObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      OrderItem deletedObject;
      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedObject = OrderItem.GetObject (DomainObjectIDs.OrderItem5);
        deletedObject.Delete();
      }

      try
      {
        action (_data, deletedObject);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void CheckOwningObjectDeletedException (Action<EndPointDelegatingCollectionData, DomainObject> action)
    {
      Order deletedOwningObject;
      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedOwningObject = Order.GetObject (DomainObjectIDs.Order4);
      }

      var endPointStub = CreateCollectionEndPointStub (ClientTransactionMock, deletedOwningObject);
      var data = new EndPointDelegatingCollectionData (endPointStub, _actualDataStub);

      using (_data.AssociatedEndPoint.ClientTransaction.EnterNonDiscardingScope ())
      {
        deletedOwningObject.Delete ();
      }

      try
      {
        action (data, _orderItem1);
        Assert.Fail ("Expected ObjectDeletedException");
      }
      catch (ObjectDeletedException)
      {
        // ok
      }
    }

    private void PrepareActualDataContents (params DomainObject[] contents)
    {
      _actualDataStub.Stub (stub => stub.Count).Return (contents.Length);
      _actualDataStub.Stub (stub => stub.GetEnumerator ()).Return (((IEnumerable<DomainObject>) contents).GetEnumerator ());

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _actualDataStub.Stub (stub => stub.ContainsObjectID (contents[currentIndex].ID)).Return (true);
        _actualDataStub.Stub (stub => stub.GetObject (contents[currentIndex].ID)).Return (contents[currentIndex]);
        _actualDataStub.Stub (stub => stub.GetObject (currentIndex)).Return (contents[currentIndex]);
        _actualDataStub.Stub (stub => stub.IndexOf (contents[currentIndex].ID)).Return (currentIndex);
      }
    }
  }
}