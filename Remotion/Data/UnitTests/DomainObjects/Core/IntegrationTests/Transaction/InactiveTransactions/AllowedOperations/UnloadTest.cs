// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class UnloadTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _endPointID = ActiveSubTransaction.Execute (() => _order1.OrderItems.AssociatedEndPointID);
    }

    [Test]
    public void UnloadDataInInactiveRootTransaction_IsAllowed ()
    {
      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataLoaded (InactiveMiddleTransaction, _order1);
      CheckDataLoaded (ActiveSubTransaction, _order1);

      UnloadService.UnloadData (InactiveRootTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (InactiveRootTransaction, _order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);
    }

    [Test]
    public void UnloadDataInInactiveMiddleTransaction_IsAllowed ()
    {
      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataLoaded (InactiveMiddleTransaction, _order1);
      CheckDataLoaded (ActiveSubTransaction, _order1);

      UnloadService.UnloadData (InactiveMiddleTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (InactiveRootTransaction, _order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);
    }

    [Test]
    public void UnloadVirtualEndPointInInactiveRootTransaction_IsAllowed ()
    {
      CheckEndPointComplete (InactiveRootTransaction, _endPointID);
      CheckEndPointComplete (InactiveMiddleTransaction, _endPointID);
      CheckEndPointComplete (ActiveSubTransaction, _endPointID);

      UnloadService.UnloadVirtualEndPoint (InactiveRootTransaction, _endPointID);

      CheckEndPointUnloaded (InactiveRootTransaction, _endPointID);
      CheckEndPointUnloaded (InactiveMiddleTransaction, _endPointID);
      CheckEndPointUnloaded (ActiveSubTransaction, _endPointID);
    }

    [Test]
    public void UnloadVirtualRelationEndPointInInactiveMiddleTransaction_IsAllowed ()
    {
      CheckEndPointComplete (InactiveRootTransaction, _endPointID);
      CheckEndPointComplete (InactiveMiddleTransaction, _endPointID);
      CheckEndPointComplete (ActiveSubTransaction, _endPointID);

      UnloadService.UnloadVirtualEndPoint (InactiveMiddleTransaction, _endPointID);

      CheckEndPointUnloaded (InactiveRootTransaction, _endPointID);
      CheckEndPointUnloaded (InactiveMiddleTransaction, _endPointID);
      CheckEndPointUnloaded (ActiveSubTransaction, _endPointID);
    }

    protected void CheckEndPointUnloaded (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      var relationEndPoint = ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID];
      Assert.That (relationEndPoint, Is.Not.Null);
      Assert.That (relationEndPoint.IsDataComplete, Is.False);
    }
  }
}