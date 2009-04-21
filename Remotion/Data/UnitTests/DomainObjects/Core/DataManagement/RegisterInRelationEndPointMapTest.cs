// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RegisterInRelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _endPoints;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPoints = ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    public void DataContainerWithNoRelation ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      DataContainer container = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      _endPoints.RegisterExistingDataContainer (container);

      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    public void OrderTicket ()
    {
      DataContainer orderTicketContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      _endPoints.RegisterExistingDataContainer (orderTicketContainer);

      Assert.AreEqual (2, _endPoints.Count, "Count");

      RelationEndPointID expectedEndPointIDForOrderTicket = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.AreEqual (expectedEndPointIDForOrderTicket,
          _endPoints[expectedEndPointIDForOrderTicket].ID, "RelationEndPointID for OrderTicket");

      Assert.AreEqual (
          DomainObjectIDs.Order1,
          ((ObjectEndPoint) _endPoints[expectedEndPointIDForOrderTicket]).OppositeObjectID,
          "OppositeObjectID for OrderTicket");

      RelationEndPointID expectedEndPointIDForOrder = new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      Assert.AreEqual (expectedEndPointIDForOrder,
          _endPoints[expectedEndPointIDForOrder].ID, "RelationEndPointID for Order");

      Assert.AreEqual (
          DomainObjectIDs.OrderTicket1,
          ((ObjectEndPoint) _endPoints[expectedEndPointIDForOrder]).OppositeObjectID,
          "OppositeObjectID for Order");
    }

    [Test]
    public void VirtualEndPoint ()
    {
      DataContainer container = TestDataContainerFactory.CreateClassWithGuidKeyDataContainer ();
      _endPoints.RegisterExistingDataContainer (container);

      Assert.AreEqual (0, _endPoints.Count);
    }

    [Test]
    public void DerivedDataContainer ()
    {
      DataContainer distributorContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();
      _endPoints.RegisterExistingDataContainer (distributorContainer);

      Assert.AreEqual (3, _endPoints.Count);
    }

    [Test]
    public void DataContainerWithOneToManyRelation ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      _endPoints.RegisterExistingDataContainer (orderContainer);

      Assert.AreEqual (2, _endPoints.Count, "Count");

      Assert.AreEqual (
          DomainObjectIDs.Customer1,
          ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer")]).OppositeObjectID);

      Assert.AreEqual (DomainObjectIDs.Official1,
          ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official")]).OppositeObjectID);
    }
  }
}
