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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
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

      RelationEndPointID expectedEndPointIDForOrderTicket = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.AreEqual (expectedEndPointIDForOrderTicket,
          _endPoints[expectedEndPointIDForOrderTicket].ID, "RelationEndPointID for OrderTicket");

      Assert.AreEqual (
          DomainObjectIDs.Order1,
          ((ObjectEndPoint) _endPoints[expectedEndPointIDForOrderTicket]).OppositeObjectID,
          "OppositeObjectID for OrderTicket");

      RelationEndPointID expectedEndPointIDForOrder = new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

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
          ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer")]).OppositeObjectID);

      Assert.AreEqual (DomainObjectIDs.Official1,
          ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official")]).OppositeObjectID);
    }
  }
}
