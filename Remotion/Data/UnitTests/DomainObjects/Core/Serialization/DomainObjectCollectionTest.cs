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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectCollectionIsSerializable ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = Serializer.SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
    }

    [Test]
    public void DomainObjectCollectionIsFlattenedSerializable ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreNotSame (collection, deserializedCollection);
      Assert.IsNotNull (deserializedCollection);
    }

    [Test]
    public void DomainObjectCollection_Contents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));
      var data = (DomainObjectCollectionData) PrivateInvoke.GetNonPublicField (collection, "_data");
      long version = data.Version;

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (PrivateInvoke.GetNonPublicField (deserializedCollection, "_changeDelegate"));
      Assert.AreEqual (version, data.Version);
    }

    [Test]
    public void DomainObjectCollection_Events_Contents ()
    {
      var collection = new DomainObjectCollection (typeof (Order)) {Order.GetObject (DomainObjectIDs.Order1)};
      var data = (DomainObjectCollectionData) PrivateInvoke.GetNonPublicField (collection, "_data");
      long version = data.Version;

      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      var tuple = Tuple.NewTuple (collection, eventReceiver);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      var deserializedCollection = deserializedTuple.A;
      var deserializedEventReceiver = deserializedTuple.B;

      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (deserializedCollection.ChangeDelegate);
      Assert.AreEqual (version, data.Version);

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
      collection = new DomainObjectCollection (collection, true);

      var deserializedCollection = SerializeAndDeserialize (collection);
      Assert.IsTrue (deserializedCollection.IsReadOnly);
    }

    private DomainObjectCollection SerializeAndDeserialize (DomainObjectCollection source)
    {
      return Serializer.SerializeAndDeserialize (source);
    }
  }
}
