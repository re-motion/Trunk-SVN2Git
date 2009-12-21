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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectEndPointExtensionsTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);
    }

    [Test]
    public void GetOppositeObject ()
    {
      var oppositeObject = _endPoint.GetOppositeObject (true);
      Assert.That (Order.GetObject (_endPoint.OppositeObjectID), Is.SameAs (oppositeObject));
    }


    [Test]
    public void GetOppositeObject_Null ()
    {
      _endPoint.OppositeObjectID = null;
      var oppositeObject = _endPoint.GetOppositeObject (false);
      Assert.That (oppositeObject, Is.Null);
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      _endPoint.OppositeObjectID = DomainObjectIDs.Order1;

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (order1));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetOppositeObject_Deleted_NoDeleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      _endPoint.OppositeObjectID = DomainObjectIDs.Order1;

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Discarded ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void GetOppositeObject_Discarded_NoDeleted ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      var originalOppositeObject = _endPoint.GetOppositeObject (true);
      _endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      var originalOppositeObject = (Order) _endPoint.GetOppositeObject (true);
      _endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      originalOppositeObject.Delete ();
      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }
  }
}