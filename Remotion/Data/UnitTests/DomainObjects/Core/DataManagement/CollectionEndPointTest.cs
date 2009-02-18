// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private DomainObjectCollection _orders;
    private DomainObjectCollection _freeOrders;
    private CollectionEndPoint _customerEndPoint;
    private Order _order1;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      _orders = new OrderCollection { _order1, _order2 };
      _freeOrders = new OrderCollection { _order1, _order2 };

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
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidRelationEndPointID ()
    {
      CreateCollectionEndPoint (null, _freeOrders);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithNullObjectIDCollection ()
    {
      CreateCollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    public void RemoveFromOppositeDomainObjects ()
    {
      var changeDelegateMock = MockRepository.GenerateMock<ICollectionEndPointChangeDelegate> ();
      var collectionEndPoint = new CollectionEndPoint (ClientTransactionMock, _customerEndPointID, _freeOrders, changeDelegateMock);

      collectionEndPoint.OppositeDomainObjects.Remove (_order1.ID);
      
      changeDelegateMock.AssertWasCalled (mock => mock.PerformRemove (collectionEndPoint, _order1));
    }

    [Test]
    public void AddToOppositeDomainObjects ()
    {
      var changeDelegateMock = MockRepository.GenerateMock<ICollectionEndPointChangeDelegate> ();
      var collectionEndPoint = new CollectionEndPoint (ClientTransactionMock, _customerEndPointID, _freeOrders, changeDelegateMock);
      var newOrder = Order.NewObject ();

      collectionEndPoint.OppositeDomainObjects.Add (newOrder);

      changeDelegateMock.AssertWasCalled (mock => mock.PerformInsert(collectionEndPoint, newOrder, _freeOrders.Count));
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
    public void Touch ()
    {
      Assert.IsFalse (_customerEndPoint.HasBeenTouched);
      _customerEndPoint.Touch ();
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
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

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification = _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Begin ();
      modification.Perform ();
      modification.End ();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    }

    [Test]
    public void PerformWithoutBegin ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification =
          _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Perform();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    }

    [Test]
    public void PerformDelete ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification = _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
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

      var id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
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

      var clone = (CollectionEndPoint) endPoint.Clone (endPoint.ClientTransaction);

      Assert.IsNotNull (clone);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void CloneChanged ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var originalItem1 = order.OrderItems[0];
      var originalItem2 = order.OrderItems[1];

      order.OrderItems.Clear ();

      var item1 = OrderItem.NewObject ();
      var item2 = OrderItem.NewObject ();
      var item3 = OrderItem.NewObject ();

      order.OrderItems.Add (item1);
      order.OrderItems.Add (item2);
      order.OrderItems.Add (item3);

      var id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
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

      var clone = (CollectionEndPoint) endPoint.Clone (endPoint.ClientTransaction);
      Assert.IsNotNull (clone);
      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void Commit ()
    {
      var newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.IsTrue (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
      Assert.IsTrue (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsFalse (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      var collectionBefore = _customerEndPoint.OppositeDomainObjects;
      var originalCollectionBefore = _customerEndPoint.OriginalOppositeDomainObjects;

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
      var newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.IsTrue (_customerEndPoint.HasChanged);
      Assert.IsTrue (_customerEndPoint.HasBeenTouched);
      Assert.IsTrue (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder));
      Assert.IsFalse (_customerEndPoint.OriginalOppositeDomainObjects.ContainsObject (newOrder));

      var collectionBefore = _customerEndPoint.OppositeDomainObjects;
      var originalCollectionBefore = _customerEndPoint.OriginalOppositeDomainObjects;

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
      var orders1 = new DomainObjectCollection (new[] { _order1 }, false);
      var orders2 = new DomainObjectCollection (new[] { _order2 }, false);

      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone());
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      var newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

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
      var orders1 = new DomainObjectCollection (new[] { _order1 }, false);
      var orders2 = new DomainObjectCollection (new[] { _order2 }, false);

      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

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
      var orders1 = new DomainObjectCollection (new[] { _order1 }, false);
      var orders2 = new DomainObjectCollection (new[] { _order2 }, false);

      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone());
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      var newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

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
      var orders1 = new DomainObjectCollection (new[] { _order1 }, false);
      var orders2 = new DomainObjectCollection (new[] { _order2 }, false);

      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders2.Clone ());

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.IsTrue (endPoint1.HasChanged);
      Assert.IsTrue (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

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
      var orders1 = new DomainObjectCollection (new[] {_order1}, false);

      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, orders1.Clone ());

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjects;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.IsFalse (endPoint1.HasChanged);
      Assert.IsFalse (endPoint1.HasBeenTouched);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (orders1));
      Assert.That (endPoint1.OriginalOppositeDomainObjects, Is.EquivalentTo (orders1));

      Assert.AreSame (collectionBefore, endPoint1.OppositeDomainObjects);
      Assert.AreSame (originalCollectionBefore, endPoint1.OriginalOppositeDomainObjects);
    }

    [Test]
    public void ReplaceOppositeCollection_SetsProperty_WithReferenceSemantics ()
    {
      var oldOpposites = new DomainObjectCollection (new[] { _order1 }, false);
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);
      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));

      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
    }

    [Test]
    public void ReplaceOppositeCollection_LeavesOriginalCollection ()
    {
      var oldOpposites = new DomainObjectCollection (new[] { _order1 }, false);
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);
      var originalOpposites = endPoint.OriginalOppositeDomainObjects;

      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (endPoint.OriginalOppositeDomainObjects, Is.SameAs (originalOpposites));
    }

    [Test]
    public void ReplaceOppositeCollection_SetsChangeDelegateOfNewCollection ()
    {
      var oldOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);

      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (newOpposites.ChangeDelegate, Is.SameAs (endPoint));
    }

    [Test]
    public void ReplaceOppositeCollection_ResetsChangeDelegateOfOldCollection ()
    {
      var oldOpposites = new DomainObjectCollection (new[] { _order1 }, false);
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);

      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The new opposite collection is already associated with another " 
        + "relation property.")]
    public void ReplaceOppositeCollection_ThrowsIfCollectionAlreadyHasChangeDelegate ()
    {
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      CreateCollectionEndPoint (_customerEndPointID, newOpposites);
      Assert.That (newOpposites.ChangeDelegate, Is.Not.Null);

      var endPoint = CreateCollectionEndPoint (_customerEndPointID, _freeOrders);
      endPoint.ReplaceOppositeCollection (newOpposites);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = @"The new opposite collection must have the same type as the old "
        + @"collection \(\'Remotion.Data.DomainObjects.ObjectList.*\'\), but its type is \'Remotion.Data.DomainObjects.ObjectList"
        + @".*\'.", MatchType = MessageMatch.Regex)]
    public void ReplaceOppositeCollection_ThrowsIfCollectionOfOtherTypeIsSet ()
    {
      var orders = new ObjectList<Order> ();
      var clients = new ObjectList<Client> ();

      var endPoint = CreateCollectionEndPoint (_customerEndPointID, orders);
      endPoint.ReplaceOppositeCollection (clients);
    }

    [Test]
    public void ReplaceOppositeCollection_WorksIfCollectionHasSameChangeDelegate ()
    {
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, newOpposites);
      Assert.That (newOpposites.ChangeDelegate, Is.Not.Null);
      endPoint.ReplaceOppositeCollection (newOpposites);
    }

    [Test]
    public void ReplaceOppositeCollection_SetsTouchedFlag ()
    {
      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, newOpposites);
      Assert.That (endPoint.HasBeenTouched, Is.False);

      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ReplaceOppositeCollection_SetsChangedFlag_WhenSetToNewCollection ()
    {
      var oldOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);
      Assert.That (endPoint.HasChanged, Is.False);
      endPoint.ReplaceOppositeCollection (oldOpposites);
      Assert.That (endPoint.HasChanged, Is.False);

      var newOpposites = new DomainObjectCollection (new[] { _order2 }, false);
      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (endPoint.HasChanged, Is.True);
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, _freeOrders);
      var newOpposites = new OrderCollection { _order2 };

      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection
      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (_freeOrders));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _order2 }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference_UndowsModifications_LeavesModificationOnDetached ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, _freeOrders);
      var newOpposites = new OrderCollection { _order2 };

      _freeOrders.Clear (); // modify collection
      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection
      newOpposites.Add (_order1);
      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (_freeOrders));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _order2 }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
      Assert.That (newOpposites, Is.EqualTo (new[] { _order2, _order1 } )); // does not undo changes on detached collection
    }

    [Test]
    public void CommitAfterReplace_SavesReference ()
    {
      var oldOpposites = _freeOrders;
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);
      var newOpposites = new OrderCollection { _order2 };

      oldOpposites.Clear (); // modify collection
      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection
      endPoint.Commit ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order2 }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);

      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order2 }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    public void ReplaceOppositeCollection_UpdatesOppositeEndPointViaChangeDelegate ()
    {
      var mockRepository = new MockRepository ();
      var changeDelegateMock = mockRepository.StrictMock<ICollectionEndPointChangeDelegate> ();
      var oldOpposites = new OrderCollection { _order1 };
      var endPoint = new CollectionEndPoint(ClientTransactionMock, _customerEndPointID, oldOpposites, changeDelegateMock);

      changeDelegateMock.PerformRemove (endPoint, _order1); // expectation
      changeDelegateMock.PerformInsert (endPoint, _order2, 1); // expectation

      mockRepository.ReplayAll ();

      var newOpposites = new OrderCollection { _order2 };
      endPoint.ReplaceOppositeCollection (newOpposites);
      
      mockRepository.VerifyAll ();
    }

    [Test]
    public void ReplaceOppositeCollection_LeavesOriginalCollectionUnchanged ()
    {
      var oldOpposites = new OrderCollection { _order1 };
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, oldOpposites);

      var newOpposites = new OrderCollection { _order2 };
      endPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (oldOpposites, Is.EqualTo (new[] { _order1 }));
      Assert.That (newOpposites, Is.EqualTo (new[] { _order2 }));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      var modification = _customerEndPoint.CreateRemoveModification (_order1);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (((CollectionEndPointRemoveModification) modification).ModifiedCollectionData, 
          Is.SameAs (PrivateInvoke.GetNonPublicField (_orders, typeof (DomainObjectCollection), "_data")));
    }

    [Test]
    public void CreateInsertModification ()
    {
      var modification = _customerEndPoint.CreateInsertModification (_order1, 12);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointInsertModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order1));
      Assert.That (((CollectionEndPointInsertModification) modification).Index, Is.EqualTo (12));
      Assert.That (((CollectionEndPointInsertModification) modification).ModifiedCollectionData,
          Is.SameAs (PrivateInvoke.GetNonPublicField (_orders, typeof (DomainObjectCollection), "_data")));
    }

    [Test]
    public void CreateReplaceModification ()
    {
      var modification = _customerEndPoint.CreateReplaceModification(0, _order2);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointReplaceModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order2));
      Assert.That (((CollectionEndPointReplaceModification) modification).ModifiedCollectionData,
          Is.SameAs (PrivateInvoke.GetNonPublicField (_orders, typeof (DomainObjectCollection), "_data")));
    }
  }
}
