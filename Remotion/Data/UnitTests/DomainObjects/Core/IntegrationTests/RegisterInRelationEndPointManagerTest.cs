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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RegisterInRelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (ClientTransactionMock.DataManager);
    }

    [Test]
    public void DataContainerWithNoRelation ()
    {
      var container = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      _relationEndPointManager.RegisterEndPointsForDataContainer (container);

      Assert.AreEqual (0, _relationEndPointManager.RelationEndPoints.Count);
    }

    [Test]
    public void OrderTicket ()
    {
      var orderTicketContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      _relationEndPointManager.RegisterEndPointsForDataContainer (orderTicketContainer);

      Assert.AreEqual (2, _relationEndPointManager.RelationEndPoints.Count, "Count");

      var expectedEndPointIDForOrderTicket = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, 
                                                    "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

      Assert.AreEqual (expectedEndPointIDForOrderTicket,
                       _relationEndPointManager.GetRelationEndPointWithoutLoading (expectedEndPointIDForOrderTicket).ID, "RelationEndPointID for OrderTicket");

      Assert.AreEqual (
          DomainObjectIDs.Order1,
          ((IObjectEndPoint) _relationEndPointManager.GetRelationEndPointWithoutLoading (expectedEndPointIDForOrderTicket)).OppositeObjectID,
          "OppositeObjectID for OrderTicket");

      var expectedEndPointIDForOrder = RelationEndPointID.Create(DomainObjectIDs.Order1, 
                                              "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      Assert.AreEqual (expectedEndPointIDForOrder,
                       _relationEndPointManager.GetRelationEndPointWithoutLoading (expectedEndPointIDForOrder).ID, "RelationEndPointID for Order");

      Assert.AreEqual (
          DomainObjectIDs.OrderTicket1,
          ((IObjectEndPoint) _relationEndPointManager.GetRelationEndPointWithoutLoading (expectedEndPointIDForOrder)).OppositeObjectID,
          "OppositeObjectID for Order");
    }

    [Test]
    public void VirtualEndPoint ()
    {
      DataContainer container = TestDataContainerFactory.CreateClassWithGuidKeyDataContainer ();
      _relationEndPointManager.RegisterEndPointsForDataContainer (container);

      Assert.AreEqual (0, _relationEndPointManager.RelationEndPoints.Count);
    }

    [Test]
    public void DerivedDataContainer ()
    {
      DataContainer distributorContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();
      _relationEndPointManager.RegisterEndPointsForDataContainer (distributorContainer);

      Assert.AreEqual (4, _relationEndPointManager.RelationEndPoints.Count);
    }

    [Test]
    public void DataContainerWithOneToManyRelation ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();

      _relationEndPointManager.RegisterEndPointsForDataContainer (orderContainer);

      Assert.AreEqual (4, _relationEndPointManager.RelationEndPoints.Count, "Count");

      var customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");
      var customerEndPoint = (IObjectEndPoint) _relationEndPointManager.GetRelationEndPointWithoutLoading (customerEndPointID);
      Assert.AreEqual (DomainObjectIDs.Customer1, customerEndPoint.OppositeObjectID);

      var customerOrdersEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Customer1, 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      var customerOrdersEndPoint = _relationEndPointManager.GetRelationEndPointWithoutLoading (customerOrdersEndPointID);
      Assert.IsNotNull (customerOrdersEndPoint);

      var officialEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official");
      var officialEndPoint = (IObjectEndPoint) _relationEndPointManager.GetRelationEndPointWithoutLoading (officialEndPointID);
      Assert.AreEqual (DomainObjectIDs.Official1, officialEndPoint.OppositeObjectID);
      var officialOrdersEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Official1, 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
      var officialOrdersEndPoint = _relationEndPointManager.GetRelationEndPointWithoutLoading (officialOrdersEndPointID);
      Assert.IsNotNull (officialOrdersEndPoint);
    }
  }
}