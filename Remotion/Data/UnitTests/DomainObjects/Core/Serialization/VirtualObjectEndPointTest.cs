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
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private VirtualObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Computer.GetObject (DomainObjectIDs.Computer1);
      _endPoint = (VirtualObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Employee3, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Employee), "Computer"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.VirtualObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void VirtualObjectEndPoint_IsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void VirtualObjectEndPoint_IsFlattenedSerializable ()
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
      _endPoint.OppositeObjectID = DomainObjectIDs.Computer2;
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Computer2, _endPoint.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Computer1, _endPoint.OriginalOppositeObjectID);
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
  }
}
