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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointCollectionTest : ClientTransactionBaseTest
  {
    private RelationEndPoint _orderTicketEndPoint;
    private RelationEndPointCollection _endPoints;

    public override void SetUp ()
    {
      base.SetUp ();

      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      
      _orderTicketEndPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, DomainObjectIDs.Order1);

      _endPoints = new RelationEndPointCollection (ClientTransactionMock);
    }

    [Test]
    public void Add ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreEqual (1, _endPoints.Count);
    }

    [Test]
    public void EndPointIDIndexer ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreSame (_orderTicketEndPoint, _endPoints[_orderTicketEndPoint.ID]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreSame (_orderTicketEndPoint, _endPoints[0]);
    }

    [Test]
    public void ContainsRelationEndPointIDTrue ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.IsTrue (_endPoints.Contains (_orderTicketEndPoint.ID));
    }

    [Test]
    public void ContainsRelationEndPointIDFalse ()
    {
      Assert.IsFalse (_endPoints.Contains (_orderTicketEndPoint.ID));
    }

    [Test]
    public void ContainsRelationEndPointTrue ()
    {
      _endPoints.Add (_orderTicketEndPoint);

      Assert.IsTrue (_endPoints.Contains (_orderTicketEndPoint));
    }

    [Test]
    public void ContainsRelationEndPointFalse ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      ObjectEndPoint copy = RelationEndPointObjectMother.CreateRealObjectEndPoint (_orderTicketEndPoint.ID);

      Assert.IsFalse (_endPoints.Contains (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsRelationEndPointNull ()
    {
      _endPoints.Contains ((RelationEndPoint) null);
    }

    [Test]
    public void CopyConstructor ()
    {
      _endPoints.Add (_orderTicketEndPoint);

      var copiedCollection = new RelationEndPointCollection (_endPoints, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_orderTicketEndPoint, copiedCollection[0]);
    }

    [Test]
    public void RemoveByEndPointID ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreEqual (1, _endPoints.Count);

      _endPoints.Remove (_orderTicketEndPoint.ID);
      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    public void RemoveByRelationEndPoint ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreEqual (1, _endPoints.Count);

      _endPoints.Remove (_orderTicketEndPoint);
      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    public void RemoveByIndex ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreEqual (1, _endPoints.Count);

      _endPoints.Remove (0);
      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RemoveNullEndPoint ()
    {
      _endPoints.Remove ((RelationEndPoint) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RemoveNullEndPointID ()
    {
      _endPoints.Remove ((RelationEndPointID) null);
    }

    [Test]
    public void Clear ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      Assert.AreEqual (1, _endPoints.Count);

      _endPoints.Clear ();
      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    public void MergeForItemAlreadyInCollection ()
    {
      _endPoints.Add (_orderTicketEndPoint);
      var secondEndPoints = new RelationEndPointCollection (_endPoints, false);

      _endPoints.Combine (secondEndPoints);

      Assert.AreEqual (1, _endPoints.Count);
    }
  }
}
