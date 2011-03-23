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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private RealObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Computer.GetObject (DomainObjectIDs.Computer1);
      _endPoint = (RealObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          RelationEndPointID.Create(DomainObjectIDs.Computer1, ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
      "Type 'Remotion.Data.DomainObjects.DataManagement.RealObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RealObjectEndPoint_IsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void RealObjectEndPoint_IsFlattenedSerializable ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void UntouchedContent ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void TouchedContent ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Employee1);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Employee1, _endPoint.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Employee3, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void ForeignKeyProperty ()
    {
      OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var id = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");
      var endPoint = (RealObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      Assert.That (endPoint.ForeignKeyProperty, Is.Not.Null);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (endPoint);

      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.Not.Null);
      Assert.That (deserializedEndPoint.ForeignKeyProperty, Is.Not.Null);
      var expectedForeignKeyProperty = deserializedEndPoint.ForeignKeyDataContainer.PropertyValues[typeof (OrderTicket) + ".Order"];
      Assert.That (deserializedEndPoint.ForeignKeyProperty,  Is.SameAs (expectedForeignKeyProperty));
    }

    [Test]
    public void ForeignKeyProperty_IntegrationWithDataManager ()
    {
      OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var id = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");

      var deserializedDataManager = Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager);

      var deserializedEndPoint = (RealObjectEndPoint) deserializedDataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);

      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.Not.Null);
      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.SameAs (deserializedDataManager.DataContainerMap[DomainObjectIDs.OrderTicket1]));
      
      Assert.That (deserializedEndPoint.ForeignKeyProperty, Is.Not.Null);
      var expectedForeignKeyProperty = deserializedEndPoint.ForeignKeyDataContainer.PropertyValues[typeof (OrderTicket) + ".Order"];
      Assert.That (deserializedEndPoint.ForeignKeyProperty, Is.SameAs (expectedForeignKeyProperty));
    }

    [Test]
    public void SyncState ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      var syncState = ObjectEndPointTestHelper.GetSyncState (deserializedEndPoint);
      Assert.That (syncState, Is.Not.Null);
      Assert.That (syncState.GetType (), Is.SameAs (ObjectEndPointTestHelper.GetSyncState (_endPoint).GetType ()));
    }

    [Test]
    public void LazyLoader ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      var lazyLoader = deserializedEndPoint.LazyLoader;
      Assert.That (lazyLoader, Is.Not.Null);
    }

    [Test]
    public void EndPointProvider ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      var endPointProvider = deserializedEndPoint.EndPointProvider;
      Assert.That (endPointProvider, Is.Not.Null);

    }
  }
}
