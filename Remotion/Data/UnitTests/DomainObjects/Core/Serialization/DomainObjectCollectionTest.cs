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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectCollection_IsSerializable ()
    {
      var collection = new DomainObjectCollection ();
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = Serializer.SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
    }

    [Test]
    public void DomainObjectCollection_StandAlone_Contents ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (deserializedCollection.AssociatedEndPoint);
    }

    [Test]
    public void DomainObjectCollection_StandAlone_Data ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      var dataStore = (DomainObjectCollectionData) 
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (collection).GetDataStore();
      long version = dataStore.Version;

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (deserializedCollection, typeof (Order));
      var deserializedDataStore = (DomainObjectCollectionData)
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (deserializedCollection).GetDataStore ();
      Assert.AreEqual (version, deserializedDataStore.Version);
    }

    [Test]
    public void DomainObjectCollection_Associated ()
    {
      var customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      var collection = customer1.Orders;
      var endPointID = new RelationEndPointID (customer1.ID, collection.AssociatedEndPoint.Definition);
      var relatedIDs = collection.Select (obj => obj.ID).ToArray();

      var deserializedCollectionAndTransaction = Serializer.SerializeAndDeserialize (Tuple.Create (collection, ClientTransactionMock));
      var deserializedCollection = deserializedCollectionAndTransaction.Item1;
      var deserializedTransaction = deserializedCollectionAndTransaction.Item2;

      var deserializedEndPoint = deserializedTransaction.DataManager.RelationEndPointMap[endPointID];
      Assert.That (deserializedCollection.AssociatedEndPoint, Is.SameAs (deserializedEndPoint));

      var deserializedData = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (deserializedCollection);
      DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (deserializedData);

      Assert.That (collection.Select (obj => obj.ID).ToArray (), Is.EqualTo (relatedIDs));
    }

    [Test]
    public void DomainObjectCollection_Events_Contents ()
    {
      var collection = new DomainObjectCollection (typeof (Order)) {Order.GetObject (DomainObjectIDs.Order1)};

      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      var deserializedCollectionAndEventReceiver = Serializer.SerializeAndDeserialize (Tuple.Create (collection, eventReceiver));
      var deserializedCollection = deserializedCollectionAndEventReceiver.Item1;
      var deserializedEventReceiver = deserializedCollectionAndEventReceiver.Item2;

      Assert.IsFalse (deserializedEventReceiver.HasAddedEventBeenCalled);
      Assert.IsFalse (deserializedEventReceiver.HasAddingEventBeenCalled);
      Assert.IsFalse (deserializedEventReceiver.HasRemovedEventBeenCalled);
      Assert.IsFalse (deserializedEventReceiver.HasRemovingEventBeenCalled);

      deserializedCollection.Add (Order.NewObject());
      deserializedCollection.RemoveAt (0);

      Assert.IsTrue (deserializedEventReceiver.HasAddedEventBeenCalled);
      Assert.IsTrue (deserializedEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (deserializedEventReceiver.HasRemovedEventBeenCalled);
      Assert.IsTrue (deserializedEventReceiver.HasRemovingEventBeenCalled);
    }

    [Test]
    public void DomainObjectCollection_ReadOnlyContents ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection = collection.Clone (true);

      var deserializedCollection = SerializeAndDeserialize (collection);
      Assert.IsTrue (deserializedCollection.IsReadOnly);
    }

    private DomainObjectCollection SerializeAndDeserialize (DomainObjectCollection source)
    {
      return Serializer.SerializeAndDeserialize (source);
    }
  }
}
