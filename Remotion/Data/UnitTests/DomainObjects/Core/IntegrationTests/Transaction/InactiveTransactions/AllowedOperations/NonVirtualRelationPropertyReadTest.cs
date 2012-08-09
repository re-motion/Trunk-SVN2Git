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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class NonVirtualRelationPropertyReadTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private RelationEndPointID _relationEndPointID;
    private RelationEndPointID _oppositeRelationEndPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = (Order) LifetimeService.GetObjectReference (ActiveSubTransaction, DomainObjectIDs.Order1);
      _relationEndPointID = RelationEndPointID.Resolve (_order1, o => o.Customer);
      _oppositeRelationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, _relationEndPointID.Definition.GetOppositeEndPointDefinition ());
    }

    [Test]
    public void RelationReadInInactiveRootTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataComplete (_relationEndPointID);
      ActiveSubTransaction.EnsureDataAvailable (DomainObjectIDs.Customer1);

      var customer = InactiveRootTransaction.Execute (() => _order1.Customer);

      Assert.That (customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
    }

    [Test]
    public void RelationReadInInactiveMiddleTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataComplete (_relationEndPointID);
      ActiveSubTransaction.EnsureDataAvailable (DomainObjectIDs.Customer1);

      var customer = InactiveMiddleTransaction.Execute (() => _order1.Customer);

      Assert.That (customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
    }

    [Test]
    public void RelationReadInInactiveRootTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Customer1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Customer1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Customer1);

      CheckEndPointNull (InactiveRootTransaction, _relationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _relationEndPointID);

      CheckEndPointNull (InactiveRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _oppositeRelationEndPointID);

      var customer = InactiveRootTransaction.Execute (() => _order1.Customer);
      
      Assert.That (customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));

      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);

      CheckDataLoaded (InactiveRootTransaction, customer);
      CheckDataNotLoaded (InactiveMiddleTransaction, customer);
      CheckDataNotLoaded (ActiveSubTransaction, customer);

      CheckEndPointComplete (InactiveRootTransaction, _relationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _relationEndPointID);

      CheckEndPointIncomplete (InactiveRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _oppositeRelationEndPointID);
    }

    [Test]
    public void RelationReadInInactiveMiddleTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Customer1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Customer1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Customer1);

      CheckEndPointNull (InactiveRootTransaction, _relationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _relationEndPointID);

      CheckEndPointNull (InactiveRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (InactiveMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _oppositeRelationEndPointID);

      var customer = InactiveMiddleTransaction.Execute (() => _order1.Customer);

      Assert.That (customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));

      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);

      CheckDataLoaded (InactiveRootTransaction, customer);
      CheckDataLoaded (InactiveMiddleTransaction, customer);
      CheckDataNotLoaded (ActiveSubTransaction, customer);

      CheckEndPointComplete (InactiveRootTransaction, _relationEndPointID);
      CheckEndPointComplete (InactiveMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _relationEndPointID);

      CheckEndPointIncomplete (InactiveRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointIncomplete (InactiveMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ActiveSubTransaction, _oppositeRelationEndPointID);
    }
  }
}