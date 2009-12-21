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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private VirtualObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a virtual end point.\r\nParameter name: id")]
    public void Initialize_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      new VirtualObjectEndPoint (ClientTransactionMock, id, null);
    }

    [Test]
    public void InitializeWithNullObjectID ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);

      Assert.That (endPoint.OriginalOppositeObjectID, Is.Null);
      Assert.That (endPoint.OppositeObjectID, Is.Null);
    }


    [Test]
    public void OppositeObjectID_Get ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void OppositeObjectID_Set ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      _endPoint.OppositeObjectID = DomainObjectIDs.OrderTicket2;
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void OppositeObjectID_Set_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _endPoint.OppositeObjectID = _endPoint.OppositeObjectID;
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.HasChanged, Is.False);

      _endPoint.OppositeObjectID = DomainObjectIDs.OrderTicket2;

      Assert.That (_endPoint.HasChanged, Is.True);
    }


    [Test]
    public void HasChanged_WithOriginalAndCurrentNull ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);

      Assert.That (endPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChanged_WithOriginalNull_CurrentNotNull ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);
      endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());

      Assert.That (endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasChangedWith_OriginalNonNull_CurrentNotNull ()
    {
      _endPoint.OppositeObjectID = null;

      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.Touch ();
      
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      _endPoint.OppositeObjectID = DomainObjectIDs.OrderTicket2;

      _endPoint.Commit ();

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void Commit_ClearsTouchedFlag ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Commit ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      _endPoint.OppositeObjectID = DomainObjectIDs.OrderTicket2;
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket1));

      _endPoint.Rollback ();

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void Rollback_ClearsTouchedFlag ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

  }
}