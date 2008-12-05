// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

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
  }
}
