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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
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
      long version = (long) PrivateInvoke.GetNonPublicField (collection, "_version");

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (PrivateInvoke.GetNonPublicField (deserializedCollection, "_changeDelegate"));
      Assert.AreEqual (version, PrivateInvoke.GetNonPublicField (collection, "_version"));
    }

    [Test]
    public void DomainObjectCollection_Events_Contents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection.Add (Order.GetObject (DomainObjectIDs.Order1));
      long version = (long) PrivateInvoke.GetNonPublicField (collection, "_version");

      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.AreEqual (1, deserializedCollection.Count);
      Assert.IsTrue (deserializedCollection.Contains (DomainObjectIDs.Order1));
      Assert.AreEqual (DomainObjectIDs.Order1, deserializedCollection[0].ID);
      Assert.AreEqual (typeof (Order), deserializedCollection.RequiredItemType);
      Assert.IsFalse (deserializedCollection.IsReadOnly);
      Assert.IsNull (PrivateInvoke.GetNonPublicField (deserializedCollection, "_changeDelegate"));
      Assert.AreEqual (version, PrivateInvoke.GetNonPublicField (collection, "_version"));

      Assert.IsFalse (eventReceiver.HasAddedEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasAddingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);

      deserializedCollection.Add (Order.NewObject());
      deserializedCollection.RemoveAt (0);

      Assert.IsTrue (eventReceiver.HasAddedEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRemovedEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRemovingEventBeenCalled);
    }

    [Test]
    public void DomainObjectCollection_ReadOnlyContents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      collection = new DomainObjectCollection (collection, true);

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.IsTrue (deserializedCollection.IsReadOnly);
    }

    private DomainObjectCollection SerializeAndDeserialize (DomainObjectCollection source)
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo();
      source.SerializeIntoFlatStructure (serializationInfo);
      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData());
      DomainObjectCollection target = new DomainObjectCollection();
      target.DeserializeFromFlatStructure (deserializationInfo);
      return target;
    }
  }
}
