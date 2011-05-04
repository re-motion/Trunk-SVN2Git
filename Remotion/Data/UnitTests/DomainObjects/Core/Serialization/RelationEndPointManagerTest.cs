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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class RelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp ();
      _relationEndPointManager = (RelationEndPointManager) ClientTransactionMock.DataManager.RelationEndPointManager;
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RelationEndPointManager' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_relationEndPointManager);
    }

    [Test]
    public void RelationEndPointMapIsFlattenedSerializable ()
    {
      RelationEndPointManager deserializedManager = FlattenedSerializer.SerializeAndDeserialize (_relationEndPointManager);
      Assert.That (deserializedManager, Is.Not.Null);
      Assert.That (deserializedManager, Is.Not.SameAs (_relationEndPointManager));
    }

    [Test]
    public void RelationEndPointMap_Content ()
    {
      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      Assert.That (_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo (7));

      var deserializedMap = (RelationEndPointManager) Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager).RelationEndPointManager;

      Assert.That (deserializedMap.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedMap.ClientTransaction, Is.InstanceOf (typeof (ClientTransactionMock)));
      Assert.That (deserializedMap.ClientTransaction, Is.Not.SameAs (ClientTransactionMock));

      var deserializedDataManager = ClientTransactionTestHelper.GetDataManager (deserializedMap.ClientTransaction);

      Assert.That (deserializedMap.LazyLoader, Is.SameAs (deserializedDataManager));
      Assert.That (deserializedMap.EndPointProvider, Is.SameAs (deserializedDataManager));
      Assert.That (deserializedMap.CollectionEndPointDataKeeperFactory, Is.TypeOf (_relationEndPointManager.CollectionEndPointDataKeeperFactory.GetType ()));
      Assert.That (deserializedMap.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf (_relationEndPointManager.VirtualObjectEndPointDataKeeperFactory.GetType ()));
      Assert.That (deserializedMap.RegistrationAgent, Is.TypeOf (_relationEndPointManager.RegistrationAgent.GetType()));

      Assert.That (deserializedMap.RelationEndPoints.Count, Is.EqualTo (7));

      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      var endPoint = (CollectionEndPoint) deserializedMap.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (endPoint.ClientTransaction, Is.SameAs (deserializedMap.ClientTransaction));
    }
  }
}
