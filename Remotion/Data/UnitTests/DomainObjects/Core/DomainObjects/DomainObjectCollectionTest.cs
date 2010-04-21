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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public partial class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    private DomainObjectCollection _collection;
    private DomainObjectCollection _readOnlyCollection;

    private IDomainObjectCollectionData _dataStrategyMock;
    private DomainObjectCollection _collectionWithDataStrategyMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
      _customer3NotInCollection = Customer.GetObject (DomainObjectIDs.Customer3);

      _collection = CreateCustomerCollection ();
      _readOnlyCollection = DomainObjectCollectionFactory.Instance.CreateReadOnlyCollection (
          typeof (DomainObjectCollection), 
          new[] { _customer1, _customer2 });

      _dataStrategyMock = MockRepository.GenerateMock<IDomainObjectCollectionData> ();
      _collectionWithDataStrategyMock = new DomainObjectCollection (_dataStrategyMock);
    }

    [Test]
    public void CreateDataStrategyForStandAloneCollection ()
    {
      var dataStoreStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      var eventRaiserStub = MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ();
      
      var modificationCheckingDecorator = 
          DomainObjectCollection.CreateDataStrategyForStandAloneCollection (dataStoreStub, typeof (Order), eventRaiserStub);
      Assert.That (modificationCheckingDecorator, Is.InstanceOfType (typeof (ModificationCheckingCollectionDataDecorator)));
      Assert.That (modificationCheckingDecorator.RequiredItemType, Is.SameAs (typeof (Order)));

      var eventRaisingDecorator = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (
          (ModificationCheckingCollectionDataDecorator) modificationCheckingDecorator);
      Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (eventRaiserStub));
      
      var dataStore = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (eventRaisingDecorator);
      Assert.That (dataStore, Is.SameAs (dataStoreStub));
    }

    [Test]
    public void Initialization_Default ()
    {
      var collection = new DomainObjectCollection ();

      Assert.That (collection.IsReadOnly, Is.False);
      Assert.That (collection.AssociatedEndPointID, Is.Null);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (collection, null);
    }

    [Test]
    public void Initialization_WithItemType ()
    {
      var collection = new DomainObjectCollection (typeof (Order));

      Assert.That (collection.IsReadOnly, Is.False);
      Assert.That (collection.AssociatedEndPointID, Is.Null);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (collection, typeof (Order));
    }

    [Test]
    public void Initialization_WithData ()
    {
      var givenData = new DomainObjectCollectionData ();
      var collection = new DomainObjectCollection (givenData);

      Assert.That (collection.IsReadOnly, Is.False);
      Assert.That (collection.AssociatedEndPointID, Is.Null);

      var actualData = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (collection);
      Assert.That (actualData, Is.SameAs (givenData));
    }

    [Test]
    public void Initialization_WithEnumerable ()
    {
      var collection = new DomainObjectCollection (new[] { _customer1, _customer2 }, typeof (Customer));

      Assert.That (collection, Is.EqualTo (new[] { _customer1, _customer2 }));
      Assert.That (collection.RequiredItemType, Is.SameAs (typeof (Customer)));
      Assert.That (collection.IsReadOnly, Is.False);
      Assert.That (collection.AssociatedEndPointID, Is.Null);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (collection, typeof (Customer));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException), ExpectedMessage =
        "Item 0 of argument domainObjects has the type Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer instead of "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.")]
    public void Initialization_WithEnumerable_ChecksItems ()
    {
      new DomainObjectCollection (new[] { _customer1 }, typeof (Order));
    }

    [Test]
    public void IsDataAvailable ()
    {
      _dataStrategyMock.Stub (mock => mock.IsDataAvailable).Return (true);
      Assert.That (_collectionWithDataStrategyMock.IsDataAvailable, Is.True);

      _dataStrategyMock.BackToRecord ();
      _dataStrategyMock.Stub (mock => mock.IsDataAvailable).Return (false);
      Assert.That (_collectionWithDataStrategyMock.IsDataAvailable, Is.False);
    }

    [Test]
    public void EnsureDataAvailable_DelegatesToStrategy ()
    {
      _collectionWithDataStrategyMock.EnsureDataAvailable ();
      _dataStrategyMock.AssertWasCalled (mock => mock.EnsureDataAvailable ());
    }

    [Test]
    public void Count ()
    {
      Assert.That (_collection.Count, Is.EqualTo (2));
    }

    [Test]
    public void AssociatedEndPointID_AssociatedCollection ()
    {
      var endPointStub = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();
      _dataStrategyMock.Stub (stub => stub.AssociatedEndPoint).Return (endPointStub);
      
      Assert.That (_collectionWithDataStrategyMock.AssociatedEndPointID, Is.EqualTo (endPointStub.ID));
    }

    [Test]
    public void AssociatedEndPointID_StandAloneCollection ()
    {
      Assert.That (_collection.AssociatedEndPointID, Is.Null);
    }

    [Test]
    public void IsAssociatedWith_AssociatedCollection ()
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      _dataStrategyMock.Stub (stub => stub.AssociatedEndPoint).Return (endPointStub);

      Assert.That (_collectionWithDataStrategyMock.IsAssociatedWith (endPointStub), Is.True);

      var otherEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      Assert.That (_collectionWithDataStrategyMock.IsAssociatedWith (otherEndPointStub), Is.False);

      Assert.That (_collectionWithDataStrategyMock.IsAssociatedWith (null), Is.False);
    }

    [Test]
    public void IsAssociatedWith_StandAloneCollection ()
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      Assert.That (_collection.IsAssociatedWith (endPointStub), Is.False);

      Assert.That (_collection.IsAssociatedWith (null), Is.True);
    }

    [Test]
    public void GetEnumerator ()
    {
      using (var enumerator = _collection.GetEnumerator ())
      {
        Assert.That (enumerator.MoveNext(), Is.True);
        Assert.That (enumerator.Current, Is.SameAs (_customer1));

        Assert.That (enumerator.MoveNext(), Is.True);
        Assert.That (enumerator.Current, Is.SameAs (_customer2));

        Assert.That (enumerator.MoveNext(), Is.False);
      }
    }

    [Test]
    public void ContainsObject_True()
    {
      Assert.That (_collection.ContainsObject (_customer1), Is.True);
    }

    [Test]
    public void ContainsObject_False_NoID ()
    {
      Assert.That (_collection.ContainsObject (_customer3NotInCollection), Is.False);
    }

    [Test]
    public void ContainsObject_False_SameID_DifferentReference ()
    {
      var customer1InOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Customer> (_collection[0].ID);
      Assert.That (_collection.ContainsObject (customer1InOtherTransaction), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsDomainObject_WithNull ()
    {
      _collection.ContainsObject (null);
    }

    [Test]
    public void Contains_True ()
    {
      Assert.That (_collection.Contains (_customer1.ID), Is.True);
    }

    [Test]
    public void Contains_False ()
    {
      Assert.That (_collection.Contains (_customer3NotInCollection.ID), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Contains_Null ()
    {
      _collection.Contains (null);
    }

    [Test]
    public void IndexOf_Object ()
    {
      Assert.That (_collection.IndexOf (_customer1), Is.EqualTo (0));
    }

    [Test]
    public void IndexOf_Object_Null ()
    {
      Assert.That (_collection.IndexOf ((DomainObject) null), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOf_Object_OtherTransaction ()
    {
      var customer1InOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Customer> (_collection[0].ID);
      Assert.That (_collection.IndexOf (customer1InOtherTransaction), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOf_Object_IDNotContained ()
    {
      Assert.That (_collection.IndexOf (_customer3NotInCollection), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOf_ID ()
    {
      Assert.That (_collection.IndexOf (_customer1.ID), Is.EqualTo (0));
    }

    [Test]
    public void IndexOf_ID_Null ()
    {
      Assert.That (_collection.IndexOf ((ObjectID) null), Is.EqualTo (-1));
    }

    [Test]
    public void Item_Get_ByIndex ()
    {
      _dataStrategyMock.Stub (stub => stub.GetObject (12)).Return (_customer1);

      Assert.That (_collectionWithDataStrategyMock[12], Is.SameAs (_customer1));
    }

    [Test]
    public void Item_Get_ByID ()
    {
      _dataStrategyMock.Stub (stub => stub.GetObject (_customer1.ID)).Return (_customer1);

      Assert.That (_collectionWithDataStrategyMock[_customer1.ID], Is.SameAs (_customer1));
    }

    [Test]
    public void Item_Set ()
    {
      _collectionWithDataStrategyMock[12] = _customer1;
      _dataStrategyMock.AssertWasCalled (mock => mock.Replace (12, _customer1));
    }

    [Test]
    public void Item_Set_Null ()
    {
      _dataStrategyMock.Stub (stub => stub.GetObject (12)).Return (_customer2);
      _collectionWithDataStrategyMock[12] = null;

      _dataStrategyMock.AssertWasCalled (mock => mock.Remove (_customer2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot modify a read-only collection.")]
    public void Item_Set_ReadOnly_Throws ()
    {
      _readOnlyCollection[0] = _customer3NotInCollection;
    }

    [Test]
    public void Add ()
    {
      var result = _collection.Add (_customer3NotInCollection);
      Assert.That (result, Is.EqualTo (2));

      Assert.That (_collection, Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot add an item to a read-only collection.")]
    public void Add_ReadOnly_Throws ()
    {
      _readOnlyCollection.Add (_customer3NotInCollection);
    }

    [Test]
    public void AddRange ()
    {
      var collection = new DomainObjectCollection();
      collection.AddRange (new[] { _customer1, _customer2 });

      Assert.That (collection, Is.EqualTo (new[] { _customer1, _customer2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException), ExpectedMessage = "Item 1 of argument domainObjects is null.")]
    public void AddRange_ChecksItems ()
    {
      var collection = new DomainObjectCollection();
      collection.AddRange (new[] { _customer1, null });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot add items to a read-only collection.")]
    public void AddRange_ReadOnly_Throws ()
    {
      _readOnlyCollection.AddRange (new[] { _customer3NotInCollection });
    }
    
    [Test]
    public void RemoveAt ()
    {
      _dataStrategyMock.Stub (stub => stub.GetObject (12)).Return (_customer2);
      _collectionWithDataStrategyMock.RemoveAt (12);

      _dataStrategyMock.AssertWasCalled (mock => mock.Remove (_customer2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void RemoveAt_ReadOnly_Throws ()
    {
      _readOnlyCollection.RemoveAt (0);
    }

    [Test]
    public void Remove_ID ()
    {
      _collectionWithDataStrategyMock.Remove (_customer1.ID);

      _dataStrategyMock.AssertWasCalled (mock => mock.Remove (_customer1.ID));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void Remove_ID_ReadOnly_Throws ()
    {
      _readOnlyCollection.Remove (_customer1.ID);
    }

    [Test]
    public void Remove_Object ()
    {
      _collectionWithDataStrategyMock.Remove (_customer1);

      _dataStrategyMock.AssertWasCalled (mock => mock.Remove (_customer1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void Remove_Object_ReadOnly_Throws ()
    {
      _readOnlyCollection.Remove (_customer1);
    }

    [Test]
    public void Clear ()
    {
      _collectionWithDataStrategyMock.Clear();

      _dataStrategyMock.AssertWasCalled (mock => mock.Clear());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Clear_ReadOnly_Throws ()
    {
      _readOnlyCollection.Clear ();
    }

    [Test]
    public void Insert_Object ()
    {
      _collectionWithDataStrategyMock.Insert (12, _customer1);

      _dataStrategyMock.AssertWasCalled (mock => mock.Insert (12, _customer1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot insert an item into a read-only collection.")]
    public void Insert_Object_ReadOnly_Throws ()
    {
      _readOnlyCollection.Insert (0, _customer3NotInCollection);
    }

    [Test]
    public void CopyTo ()
    {
      var array = new DomainObject[4];
      _collection.CopyTo (array, 1);

      Assert.That (array, Is.EqualTo (new[] { null, _customer1, _customer2, null }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.")]
    public void CopyTo_ArraySmallerThanCollection ()
    {
      var array = new DomainObject[_collection.Count - 1];

      _collection.CopyTo (array, 0);
    }

    [Test]
    public void CopyTo_EmptyArray_WithEmptyCollection ()
    {
      var emptyCollection = new DomainObjectCollection ();
      var array = new DomainObject[0];

      emptyCollection.CopyTo (array, 0);

      // expectation: no exception
    }

    [Test]
    public void GetNonNotifyingData ()
    {
      var dataStore = new DomainObjectCollectionData ();
      var originalDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      originalDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Customer));
      originalDataStub.Stub (stub => stub.GetDataStore()).Return (dataStore);

      var collection = new DomainObjectCollection (originalDataStub);

      var nonNotifyingData = (IDomainObjectCollectionData) PrivateInvoke.InvokeNonPublicMethod (collection, "GetNonNotifyingData");
      Assert.That (nonNotifyingData, Is.InstanceOfType (typeof (ModificationCheckingCollectionDataDecorator)));
      Assert.That (nonNotifyingData.RequiredItemType, Is.SameAs (typeof (Customer)));

      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (
          (ModificationCheckingCollectionDataDecorator) nonNotifyingData);
      Assert.That (wrappedData, Is.SameAs (dataStore));
    }

    [Test]
    public void Clone ()
    {
      var clonedCollection = _collection.Clone();

      Assert.That (clonedCollection, Is.EqualTo (new[] { _customer1, _customer2 }));
      Assert.That (clonedCollection.IsReadOnly, Is.False);
      Assert.That (clonedCollection.RequiredItemType, Is.EqualTo (_collection.RequiredItemType));

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (clonedCollection, typeof (Customer));
    }

    [Test]
    public void Clone_DecouplesFromOriginalDataStore ()
    {
      var clonedCollection = _collection.Clone ();

      _collection.Remove (_customer1);
      clonedCollection.Add (_customer3NotInCollection);

      Assert.That (_collection, Is.EqualTo (new[] { _customer2 }));
      Assert.That (clonedCollection, Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void Clone_BecomesStandAlone ()
    {
      OrderCollection associatedCollection = CreateAssociatedCollectionWithEndPointStub();
      var clonedCollection = (DomainObjectCollection) associatedCollection.Clone();

      // clone is always stand-alone, even when source is associated with end point
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (clonedCollection, associatedCollection.RequiredItemType);
    }

    [Test]
    public void Clone_IsOfSameType_AsOriginal ()
    {
      var orderCollection = new OrderCollection();

      var clone = (OrderCollection) orderCollection.Clone();

      Assert.That (clone.GetType (), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (clone.RequiredItemType, Is.EqualTo (orderCollection.RequiredItemType));
    }

    [Test]
    public void Clone_ReadOnly ()
    {
      var clonedCollection = _collection.Clone (true);

      Assert.That (clonedCollection, Is.EqualTo (new[] { _customer1, _customer2 }));
      Assert.That (clonedCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void Clone_ReadOnly_DecouplesFromOriginalDataStore ()
    {
      var clonedCollection = _collection.Clone (true);

      _collection.Remove (_customer1);

      Assert.That (_collection, Is.EqualTo (new[] { _customer2 }));
      Assert.That (clonedCollection, Is.EqualTo (new[] { _customer1, _customer2 }));
    }

    [Test]
    public void Clone_ReadOnly_DataStrategy ()
    {
      OrderCollection associatedCollection = CreateAssociatedCollectionWithEndPointStub ();
      var clonedCollection = associatedCollection.Clone (true);

      // clone is always stand-alone, even when source is associated with end point
      DomainObjectCollectionDataTestHelper.CheckReadOnlyCollectionStrategy (clonedCollection);
    }

    [Test]
    public void Clone_ReadOnly_IsOfSameType_AsOriginal ()
    {
      var orderCollection = new OrderCollection ();

      var clone = (OrderCollection) orderCollection.Clone (true);

      Assert.That (clone.GetType (), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (clone.RequiredItemType, Is.Null);
    }

    [Test]
    public void CreateAssociationCommand ()
    {
      CollectionEndPoint endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();

      var newCollection = new OrderCollection ();
      var command = (CollectionEndPointReplaceWholeCollectionCommand) ((IAssociatableDomainObjectCollection) newCollection).CreateAssociationCommand (endPoint);

      Assert.That (command.ModifiedEndPoint, Is.SameAs (endPoint));
      Assert.That (command.NewOppositeCollection, Is.SameAs (newCollection));
      Assert.That (command.NewOppositeCollectionTransformer.Collection, Is.SameAs (newCollection));
      Assert.That (command.OldOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
    }

    [Test]
    public void CreateAssociationCommand_CollectionIsReadOnly ()
    {
      CollectionEndPoint endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();

      var newCollection = new OrderCollection ().Clone (true);
      var result = ((IAssociatableDomainObjectCollection) newCollection).CreateAssociationCommand (endPoint);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This collection ('Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.TestDomain.Order]') is not of the same type "
        + "as the end point's current opposite collection ('Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection').")]
    public void CreateAssociationCommand_DifferentCollectionTypes ()
    {
      CollectionEndPoint endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();

      var newCollection = new ObjectList<Order> ();
      ((IAssociatableDomainObjectCollection) newCollection).CreateAssociationCommand (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This collection has a different item type than the end point's current opposite collection.")]
    public void CreateAssociationCommand_DifferentRequiredItemType ()
    {
      CollectionEndPoint endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();

      var newCollection = new DomainObjectCollection (typeof (Customer));
      ((IAssociatableDomainObjectCollection) newCollection).CreateAssociationCommand (endPoint);
    }

    [Test]
    public void CreateAssociationCommand_SelfReplace ()
    {
      CollectionEndPoint endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders ();

      var command = (CollectionEndPointReplaceWholeCollectionCommand)
                    ((IAssociatableDomainObjectCollection) endPoint.OppositeDomainObjects).CreateAssociationCommand (endPoint);

      Assert.That (command.ModifiedEndPoint, Is.SameAs (endPoint));
      Assert.That (command.NewOppositeCollection, Is.SameAs (endPoint.OppositeDomainObjects));
      Assert.That (command.NewOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
      Assert.That (command.OldOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
    }

    [Test]
    public void Rollback ()
    {
      var sourceCollection = new DomainObjectCollection { _customer3NotInCollection };
      CallRollback (_collection, sourceCollection);

      Assert.That (_collection, Is.EqualTo (sourceCollection));
    }

    [Test]
    public void Rollback_ReadOnly ()
    {
      var readOnlyCollection = _collection.Clone (true);
      var sourceCollection = new DomainObjectCollection { _customer3NotInCollection };
      CallRollback (readOnlyCollection, sourceCollection);

      Assert.That (readOnlyCollection, Is.EqualTo (sourceCollection));
      Assert.That (readOnlyCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit ()
    {
      var sourceCollection = new[] { _customer3NotInCollection };
      CallCommit (_collection, sourceCollection);

      Assert.That (_collection, Is.EqualTo (sourceCollection));
    }

    [Test]
    public void Commit_ReadOnly ()
    {
      var readOnlyCollection = _collection.Clone (true);
      var sourceCollection = new[] { _customer3NotInCollection };
      CallCommit (readOnlyCollection, sourceCollection);

      Assert.That (readOnlyCollection, Is.EqualTo (sourceCollection));
      Assert.That (readOnlyCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void CopyEventHandlersFrom ()
    {
      var source = new DomainObjectCollection ();
      var destination = new DomainObjectCollection ();

      source.Added += delegate { };
      source.Added += delegate { };
      source.Adding += delegate { };
      source.Adding += delegate { };
      source.Removed += delegate { };
      source.Removed += delegate { };
      source.Removing += delegate { };
      source.Removing += delegate { };
      source.Deleted += delegate { };
      source.Deleted += delegate { };
      source.Deleting += delegate { };
      source.Deleting += delegate { };

      CallCopyEventHandlersFrom (source, destination);

      CheckSameEventHandlers (source, destination, "Adding");
      CheckSameEventHandlers (source, destination, "Added");
      CheckSameEventHandlers (source, destination, "Removing");
      CheckSameEventHandlers (source, destination, "Removed");
      CheckSameEventHandlers (source, destination, "Deleting");
      CheckSameEventHandlers (source, destination, "Deleted");
    }

    [Test]
    public void OnDeleting ()
    {
      object eventSender = null;
      EventArgs eventArgs = null;
      _collection.Deleting += (sender, args) => { eventSender = sender; eventArgs = args; };

      PrivateInvoke.InvokeNonPublicMethod (_collection, "OnDeleting");

      Assert.That (eventSender, Is.SameAs (_collection));
      Assert.That (eventArgs, Is.EqualTo (EventArgs.Empty));
    }

    [Test]
    public void OnDeleted ()
    {
      object eventSender = null;
      EventArgs eventArgs = null;
      _collection.Deleted += (sender, args) => { eventSender = sender; eventArgs = args; };

      PrivateInvoke.InvokeNonPublicMethod (_collection, "OnDeleted");

      Assert.That (eventSender, Is.SameAs (_collection));
      Assert.That (eventArgs, Is.EqualTo (EventArgs.Empty));
    }

    private void CallRollback (DomainObjectCollection collection, DomainObjectCollection sourceCollection)
    {
      PrivateInvoke.InvokeNonPublicMethod (collection, "Rollback", sourceCollection);
    }

    private void CallCommit (DomainObjectCollection collection, IEnumerable<DomainObject> sourceCollection)
    {
      PrivateInvoke.InvokeNonPublicMethod (collection, "Commit", sourceCollection);
    }

    private void CallCopyEventHandlersFrom (DomainObjectCollection source, DomainObjectCollection destination)
    {
      PrivateInvoke.InvokeNonPublicMethod (destination, "CopyEventHandlersFrom", source);
    }

    private void CheckSameEventHandlers (DomainObjectCollection source, DomainObjectCollection destination, string eventName)
    {
      var sourceEvent = ((Delegate) PrivateInvoke.GetNonPublicField (source, eventName));
      Delegate[] sourceInvocationList = sourceEvent.GetInvocationList ();

      var destinationEvent = ((Delegate) PrivateInvoke.GetNonPublicField (destination, eventName));
      Assert.That (destinationEvent, Is.Not.Null, eventName + " event handlers not copied");
      Delegate[] destinationInvocationList = destinationEvent.GetInvocationList ();

      Assert.That (sourceInvocationList, Is.EqualTo (destinationInvocationList), eventName + " event handlers not copied");
    }

    private DomainObjectCollection CreateCustomerCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (_customer1);
      collection.Add (_customer2);

      return collection;
    }

    private OrderCollection CreateAssociatedCollectionWithEndPointStub ()
    {
      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointDataStub = new LazyLoadableCollectionEndPointData (
          ClientTransactionMock, 
          endPointID, 
          new DomainObject[0]);

      var delegatingStrategy = new EndPointDelegatingCollectionData (collectionEndPointStub, endPointDataStub);
      var associatedCollection = new OrderCollection (new ModificationCheckingCollectionDataDecorator (typeof (Order), delegatingStrategy));
      Assert.That (DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (associatedCollection), Is.SameAs (collectionEndPointStub));
      return associatedCollection;
    }
  }
}