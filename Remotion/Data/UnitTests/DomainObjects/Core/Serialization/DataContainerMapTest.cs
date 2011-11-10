// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.DataContainerMap' in Assembly "
       + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void DataContainerMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager.DataContainers);
    }

    [Test]
    public void DataContainerMapIsFlattenedSerializable ()
    {
      DataContainerMap map = DataManagerTestHelper.GetDataContainerMap (ClientTransactionMock.DataManager);

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.IsNotNull (deserializedMap);
    }

    [Test]
    public void DataContainerMap_Content ()
    {
      DataContainerMap map = DataManagerTestHelper.GetDataContainerMap (ClientTransactionMock.DataManager);
      Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, map.Count);

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"));
      Assert.AreEqual (1, deserializedMap.Count);
    }
  }
}
