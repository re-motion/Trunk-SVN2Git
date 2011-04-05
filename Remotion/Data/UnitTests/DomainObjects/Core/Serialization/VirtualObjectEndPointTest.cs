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
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

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
          RelationEndPointID.Create(DomainObjectIDs.Employee3, ReflectionMappingHelper.GetPropertyName (typeof (Employee), "Computer"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
      "Type 'Remotion.Data.DomainObjects.DataManagement.VirtualObjectEndPoint' in Assembly "
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
    public void TouchedChangedContent ()
    {
      _endPoint.CreateSetCommand (Computer.GetObject (DomainObjectIDs.Computer2)).Perform ();
      _endPoint.Touch();

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Computer2, _endPoint.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Computer1, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void Internals ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.That (deserializedEndPoint.LazyLoader, Is.Not.Null);
      Assert.That (deserializedEndPoint.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedEndPoint.DataKeeperFactory, Is.Not.Null);

      var loadState = VirtualObjectEndPointTestHelper.GetLoadState(deserializedEndPoint);
      Assert.That (loadState, Is.Not.Null);
    }
  }
}
