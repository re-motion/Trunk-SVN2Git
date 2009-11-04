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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    [Test]
    public void DataManagerIsSerializable ()
    {
      DataManager dataManager = new DataManager (ClientTransactionMock);
      DataManager dataManager2 = Serializer.SerializeAndDeserialize (dataManager);
      Assert.IsNotNull (dataManager2);
      Assert.AreNotSame (dataManager2, dataManager);
    }

    [Test]
    public void DataManager_Content ()
    {
      DataManager dataManager = ClientTransactionMock.DataManager;
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = order.OrderItems[0];

      Order discardedOrder = Order.NewObject ();
      DataContainer discardedContainer = discardedOrder.InternalDataContainer;
      discardedOrder.Delete ();

      Assert.AreNotEqual (0, dataManager.DataContainerMap.Count);
      Assert.AreNotEqual (0, dataManager.RelationEndPointMap.Count);
      Assert.AreEqual (1, dataManager.DiscardedObjectCount);
      Assert.IsTrue (dataManager.IsDiscarded (discardedContainer.ID));
      Assert.AreSame (discardedContainer, dataManager.GetDiscardedDataContainer (discardedContainer.ID));

      Tuple<ClientTransaction, DataManager> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, dataManager));

      Assert.AreNotEqual (0, deserializedData.B.DataContainerMap.Count);
      Assert.AreNotEqual (0, deserializedData.B.RelationEndPointMap.Count);
      Assert.AreEqual (1, deserializedData.B.DiscardedObjectCount);
      Assert.IsTrue (deserializedData.B.IsDiscarded (discardedContainer.ID));
      Assert.IsNotNull (deserializedData.B.GetDiscardedDataContainer (discardedContainer.ID));

      Assert.AreSame (deserializedData.A, PrivateInvoke.GetNonPublicField (deserializedData.B, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedData.B, "_transactionEventSink"));
    }

    public void DumpSerializedDataManager ()
    {
      DataManager dataManager = ClientTransactionMock.DataManager;
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = order.OrderItems[0];

      Order discardedOrder = Order.NewObject ();
      discardedOrder.Delete ();

      for (int i = 0; i < 500; ++i)
      {
        Order newOrder = Order.NewObject();
        newOrder.OrderTicket = OrderTicket.NewObject();
      }

      SerializationInfo info = new SerializationInfo (typeof (DataManager), new FormatterConverter ());
      ((ISerializable) dataManager).GetObjectData (info, new StreamingContext ());
      object[] data = (object[]) info.GetValue ("doInfo.GetData", typeof (object[]));
      Console.WriteLine ("Object stream:");
      Dump ((object[])data[0]);
      Console.WriteLine ("Int stream:");
      Dump ((int[]) data[1]);
      Console.WriteLine ("Bool stream:");
      Dump ((bool[]) data[2]);
    }

    private void Dump<T> (T[] data)
    {
      Console.WriteLine ("The data array contains {0} elements.", data.Length);
      Dictionary<Type, int> types = new Dictionary<Type, int> ();
      foreach (T o in data)
      {
        Type type = o != null ? o.GetType() : typeof (void);
        if (!types.ContainsKey (type))
          types.Add (type, 0);
        ++types[type];
      }
      List<KeyValuePair<Type, int>> typeList = new List<KeyValuePair<Type, int>> (types);
      typeList.Sort (delegate (KeyValuePair<Type, int> one, KeyValuePair<Type, int> two) { return one.Value.CompareTo (two.Value); });
      foreach (KeyValuePair<Type, int> entry in typeList)
        Console.WriteLine ("{0}: {1}", entry.Key != typeof (void) ? entry.Key.ToString() : "<null>", entry.Value);
    }
  }
}
