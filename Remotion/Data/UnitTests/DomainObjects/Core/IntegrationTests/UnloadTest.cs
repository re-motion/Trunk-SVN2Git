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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class UnloadTest : ClientTransactionBaseTest
  {
    [Test]
    public void UnloadCollectionEndPoint_AlreadyUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var endPoint = customer.Orders.AssociatedEndPoint;

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, endPoint.ID, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      Assert.That (endPoint.IsDataAvailable, Is.False);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, endPoint.ID, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void UnloadData_Changed ()
    {
      ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'.")]
    public void UnloadData_ChangedVirtualEndPoint ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);
      domainObject.OrderTicket = OrderTicket.NewObject ();
      DomainObjectUnloader.UnloadData (ClientTransactionMock, domainObject.ID, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
    }

    [Test]
    public void UnloadData_NonLoadedObject_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void UnloadData_ChangedCollection ()
    {
      OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order.OrderItems.Add (OrderItem.NewObject ());
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID].HasChanged, Is.True);

      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.OrderItem1, DomainObjectUnloader.TransactionMode.ThisTransactionOnly);
    }

 
  }
}