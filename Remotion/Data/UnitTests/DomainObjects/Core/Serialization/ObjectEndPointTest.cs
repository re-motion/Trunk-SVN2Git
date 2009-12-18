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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Computer.GetObject (DomainObjectIDs.Computer1);
      _endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Computer1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.ObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void ObjectEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void ObjectEndPointIsFlattenedSerializable ()
    {
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void ObjectEndPoint_Untouched_Content ()
    {
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void ObjectEndPoint_Touched_Content ()
    {
      _endPoint.OppositeObjectID = DomainObjectIDs.Employee1;
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Employee1, _endPoint.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Employee3, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void ForeignKeyProperty_Null ()
    {
      Order.GetObject (DomainObjectIDs.Order1);
      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order) + ".OrderTicket");
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      Assert.That (endPoint.ForeignKeyProperty, Is.Null);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (endPoint);
    
      Assert.That (deserializedEndPoint.ForeignKeyProperty, Is.Null);
    }

    [Test]
    public void ForeignKeyProperty_NotNull ()
    {
      OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var id = new RelationEndPointID (DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      Assert.That (endPoint.ForeignKeyProperty, Is.Not.Null);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (endPoint);

      Assert.That (deserializedEndPoint.ForeignKeyProperty, Is.Not.Null);
      var expectedForeignKeyProperty = ((ClientTransactionMock) deserializedEndPoint.ClientTransaction).DataManager
          .DataContainerMap[DomainObjectIDs.OrderTicket1].PropertyValues[typeof (OrderTicket) + ".Order"];
      Assert.That (deserializedEndPoint.ForeignKeyProperty,  Is.SameAs (expectedForeignKeyProperty));
    }
  }
}
