/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private DomainObjectCollection _orders;
    private CollectionEndPoint _customerEndPoint;
    private DomainObject _order1;
    private DomainObject _order2;

    public CollectionEndPointTest ()
    {
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      _orders = new OrderCollection ();
      _orders.Add (_order1);
      _orders.Add (_order2);

      _customerEndPoint = CreateCollectionEndPoint (_customerEndPointID, _orders);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (_customerEndPointID, _customerEndPoint.ID);

      Assert.AreEqual (_orders.Count, _customerEndPoint.OriginalOppositeDomainObjects.Count);
      Assert.IsNotNull (_customerEndPoint.OriginalOppositeDomainObjects[_order1.ID]);
      Assert.IsNotNull (_customerEndPoint.OriginalOppositeDomainObjects[_order2.ID]);

      Assert.AreSame (_orders, _customerEndPoint.OppositeDomainObjects);
      Assert.AreNotSame (_customerEndPoint.OppositeDomainObjects, _customerEndPoint.OriginalOppositeDomainObjects);
    }

    [Test]
    public void GetDataContainerUsesStoredTransaction ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        Assert.AreSame (ClientTransactionMock, _customerEndPoint.GetDataContainer ().ClientTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidRelationEndPointID ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPoint (null, _orders);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithNullObjectIDCollection ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), ExpectedMessage = "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void RemoveFromOppositeDomainObjects ()
    {
      CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock, _customerEndPointID, _orders);

      collectionEndPoint.OppositeDomainObjects.Remove (_order1.ID);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), ExpectedMessage = "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void AddToOppositeDomainObjects ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);

      CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock, _customerEndPointID, _orders);

      collectionEndPoint.OppositeDomainObjects.Add (newOrder);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ChangeOriginalOppositeDomainObjects ()
    {
      _customerEndPoint.OriginalOppositeDomainObjects.Remove (_order1.ID);
    }

    [Test]
    public void HasChangedFalse ()
    {
      Assert.IsFalse (_customerEndPoint.HasChanged);
    }

    [Test]
    public void HasChangedFalseWhenSameElements ()
    {
      Assert.IsFalse (_customerEndPoint.HasChanged);
      _customerEndPoint.OppositeDomainObjects.Add (Order.NewObject ());
      Assert.IsTrue (_customerEndPoint.HasChanged);
      _customerEndPoint.OppositeDomainObjects.RemoveAt (_customerEndPoint.OppositeDomainObjects.Count - 1);
      Assert.IsFalse (_customerEndPoint.HasChanged);
    }

    [Test]
    public void HasBeenTouchedAddAndRemove_LeavingSameElements ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Add (Order.NewObject ());
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.RemoveAt (_customerEndPoint.OppositeDomainObjects.Count - 1);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedInsert ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Insert (0, Order.NewObject ());
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRemove ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Remove (_customerEndPoint.OppositeDomainObjects[0]);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRemoveNonExisting ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Remove (Order.NewObject());
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedClear ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Clear ();
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedClearEmpty ()
    {
      _customerEndPoint.OppositeDomainObjects.Clear ();
      _customerEndPoint.Commit ();

      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects.Clear ();
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedReplace ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects[0] = Order.NewObject();
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedReplaceWithSame ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedPerformDelete ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.PerformDelete ();
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void OriginalOppositeDomainObjectsType ()
    {
      Assert.AreEqual (typeof (OrderCollection), _customerEndPoint.OriginalOppositeDomainObjects.GetType ());
      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.IsReadOnly);

      Assert.AreEqual (
          _customerEndPoint.OppositeDomainObjects.RequiredItemType,
          _customerEndPoint.OriginalOppositeDomainObjects.RequiredItemType);
    }

    [Test]
    public void ChangeOppositeDomainObjects ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      RelationEndPointModification modification = _customerEndPoint.CreateModification (CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID));
      modification.Begin ();
      modification.Perform ();
      modification.End ();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    }

    [Test]
    public void PerformWithoutBegin ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      RelationEndPointModification modification =
          _customerEndPoint.CreateModification (
              CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID));
      modification.Perform();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    }

    [Test]
    public void PerformDelete ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      RelationEndPointModification modification = _customerEndPoint.CreateModification (CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID));
      modification.Begin ();
      _customerEndPoint.PerformDelete ();
      modification.End ();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
      Assert.AreEqual (0, _customerEndPoint.OppositeDomainObjects.Count);
    }

    private void CheckIfRelationEndPointsAreEqual (CollectionEndPoint expected, CollectionEndPoint actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.AreNotSame (expected, actual);

      Assert.AreSame (expected.ChangeDelegate, actual.ChangeDelegate);
      Assert.AreSame (expected.ClientTransaction, actual.ClientTransaction);
      Assert.AreSame (expected.Definition, actual.Definition);
      Assert.AreSame (expected.GetDomainObject(), actual.GetDomainObject ());
      Assert.AreEqual (expected.HasChanged, actual.HasChanged);
      Assert.AreEqual (expected.HasBeenTouched, actual.HasBeenTouched);
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.ObjectID, actual.ObjectID);

      Assert.IsNotNull (actual.OppositeDomainObjects);
      Assert.AreNotSame (expected.OppositeDomainObjects, actual.OppositeDomainObjects);

      Assert.AreEqual (expected.OppositeDomainObjects.Count, actual.OppositeDomainObjects.Count);
      for (int i = 0; i < expected.OppositeDomainObjects.Count; ++i)
        Assert.AreSame (expected.OppositeDomainObjects[i], actual.OppositeDomainObjects[i]);

      Assert.IsNotNull (actual.OriginalOppositeDomainObjects);
      Assert.AreNotSame (expected.OriginalOppositeDomainObjects, actual.OriginalOppositeDomainObjects);
      Assert.AreEqual (expected.OriginalOppositeDomainObjects.IsReadOnly, actual.OriginalOppositeDomainObjects.IsReadOnly);

      Assert.AreEqual (expected.OriginalOppositeDomainObjects.Count, actual.OriginalOppositeDomainObjects.Count);
      for (int i = 0; i < expected.OriginalOppositeDomainObjects.Count; ++i)
        Assert.AreSame (expected.OriginalOppositeDomainObjects[i], actual.OriginalOppositeDomainObjects[i]);
    }

    [Test]
    public void CloneUnchanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem item1 = order.OrderItems[0];
      OrderItem item2 = order.OrderItems[1];

      RelationEndPointID id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      CollectionEndPoint endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint.ChangeDelegate);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.AreSame (order, endPoint.GetDomainObject());
      Assert.IsFalse (endPoint.HasChanged);
      Assert.IsFalse (endPoint.HasBeenTouched);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (order.ID, endPoint.ObjectID);
      Assert.IsNotNull (endPoint.OppositeDomainObjects);
      
      Assert.AreEqual (2, endPoint.OppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OppositeDomainObjects[1]);
      
      Assert.AreNotSame (endPoint.OppositeDomainObjects, endPoint.OriginalOppositeDomainObjects);

      Assert.AreEqual (2, endPoint.OriginalOppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OriginalOppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OriginalOppositeDomainObjects[1]);

      CollectionEndPoint clone = (CollectionEndPoint) endPoint.Clone ();

      Assert.IsNotNull (clone);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void CloneChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem originalItem1 = order.OrderItems[0];
      OrderItem originalItem2 = order.OrderItems[1];

      order.OrderItems.Clear ();

      OrderItem item1 = OrderItem.NewObject ();
      OrderItem item2 = OrderItem.NewObject ();
      OrderItem item3 = OrderItem.NewObject ();

      order.OrderItems.Add (item1);
      order.OrderItems.Add (item2);
      order.OrderItems.Add (item3);

      RelationEndPointID id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      CollectionEndPoint endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint.ChangeDelegate);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.AreSame (order, endPoint.GetDomainObject ());
      Assert.IsTrue (endPoint.HasChanged);
      Assert.IsTrue (endPoint.HasBeenTouched);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (order.ID, endPoint.ObjectID);
      Assert.IsNotNull (endPoint.OppositeDomainObjects);

      Assert.AreEqual (3, endPoint.OppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OppositeDomainObjects[1]);
      Assert.AreSame (item3, endPoint.OppositeDomainObjects[2]);

      Assert.AreNotSame (endPoint.OppositeDomainObjects, endPoint.OriginalOppositeDomainObjects);

      Assert.AreEqual (2, endPoint.OriginalOppositeDomainObjects.Count);
      Assert.AreSame (originalItem1, endPoint.OriginalOppositeDomainObjects[0]);
      Assert.AreSame (originalItem2, endPoint.OriginalOppositeDomainObjects[1]);

      CollectionEndPoint clone = (CollectionEndPoint) endPoint.Clone ();

      Assert.IsNotNull (clone);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void Commit ()
    {
      Order newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.IsTrue (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
      Assert.IsTrue (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsFalse (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      DomainObjectCollection collectionBefore = _customerEndPoint.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = _customerEndPoint.OriginalOppositeDomainObjects;

      _customerEndPoint.Commit ();

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      Assert.IsTrue (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      Assert.AreSame (collectionBefore, _customerEndPoint.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, _customerEndPoint.OriginalOppositeDomainObjects);
    }

    [Test]
    public void CommitTouchedUnchanged ()
    {
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);

      _customerEndPoint.Commit ();

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void Rollback ()
    {
      Order newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.IsTrue (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
      Assert.IsTrue (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsFalse (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      DomainObjectCollection collectionBefore = _customerEndPoint.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = _customerEndPoint.OriginalOppositeDomainObjects;

      _customerEndPoint.Rollback ();

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      Assert.IsFalse (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsFalse (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      Assert.AreSame (collectionBefore, _customerEndPoint.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, _customerEndPoint.OriginalOppositeDomainObjects);
    }

    [Test]
    public void RollbackTouchedUnchanged ()
    {
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);

      _customerEndPoint.Rollback();

      Assert.IsFalse (_customerEndPoint.HasChanged);
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoUnchanged ()
    {
      DomainObjectCollection orders1 = new DomainObjectCollection (new DomainObject[] { _order1 }, false);
      DomainObjectCollection orders2 = new DomainObjectCollection (new DomainObject[] { _order2 }, false);

      CollectionEndPoint endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone());
      CollectionEndPoint endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      Order newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      DomainObjectCollection collectionBefore = endPoint1.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (endPoint2.OppositeDomainObjects));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoUnchanged ()
    {
      DomainObjectCollection orders1 = new DomainObjectCollection (new DomainObject[] { _order1 }, false);
      DomainObjectCollection orders2 = new DomainObjectCollection (new DomainObject[] { _order2 }, false);

      CollectionEndPoint endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      CollectionEndPoint endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      DomainObjectCollection collectionBefore = endPoint1.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders2));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoChanged ()
    {
      DomainObjectCollection orders1 = new DomainObjectCollection (new DomainObject[] { _order1 }, false);
      DomainObjectCollection orders2 = new DomainObjectCollection (new DomainObject[] { _order2 }, false);

      CollectionEndPoint endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone());
      CollectionEndPoint endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      Order newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      DomainObjectCollection collectionBefore = endPoint1.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (endPoint2.OppositeDomainObjects));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoChanged ()
    {
      DomainObjectCollection orders1 = new DomainObjectCollection (new DomainObject[] { _order1 }, false);
      DomainObjectCollection orders2 = new DomainObjectCollection (new DomainObject[] { _order2 }, false);

      CollectionEndPoint endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      CollectionEndPoint endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      DomainObjectCollection collectionBefore = endPoint1.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders2));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoEqual ()
    {
      DomainObjectCollection orders1 = new DomainObjectCollection (new DomainObject[] {_order1}, false);

      CollectionEndPoint endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      CollectionEndPoint endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      DomainObjectCollection collectionBefore = endPoint1.OppositeDomainObjects;
      DomainObjectCollection originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }
  }
}
