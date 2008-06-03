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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private CollectionEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      _endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Order1, ReflectionUtility.GetPropertyName (typeof (Order), "OrderItems"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.CollectionEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void CollectionEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void CollectionEndPointIsFlattenedSerializable ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void CollectionEndPoint_Touched_Content ()
    {
      _endPoint.OppositeDomainObjects.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem5));
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);

      Assert.AreEqual (3, deserializedEndPoint.OppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem5));
      Assert.IsFalse (deserializedEndPoint.OppositeDomainObjects.IsReadOnly);
      Assert.AreSame (deserializedEndPoint, PrivateInvoke.GetNonPublicField (deserializedEndPoint.OppositeDomainObjects, "_changeDelegate"));

      Assert.AreEqual (2, deserializedEndPoint.OriginalOppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.IsReadOnly);
      Assert.AreSame (deserializedEndPoint, PrivateInvoke.GetNonPublicField (deserializedEndPoint.OriginalOppositeDomainObjects, "_changeDelegate"));

      Assert.IsNull (deserializedEndPoint.ChangeDelegate);
    }

    [Test]
    public void CollectionEndPoint_Untouched_Content ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }
  }
}
