// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
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
      Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager.DataContainerMap);
    }

    [Test]
    public void DataContainerMapIsFlattenedSerializable ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.IsNotNull (deserializedMap);
    }

    [Test]
    public void DataContainerMap_Content ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;
      Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, map.Count);

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.AreEqual (ClientTransactionMock, PrivateInvoke.GetNonPublicField (deserializedMap, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"));
      Assert.AreEqual (1, deserializedMap.Count);
    }
  }
}
