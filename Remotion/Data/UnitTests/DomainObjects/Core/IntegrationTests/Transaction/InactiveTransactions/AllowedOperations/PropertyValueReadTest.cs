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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class PropertyValueReadTest : InactiveTransactionsTestBase
  {
    private Order _order1;

    public override void SetUp ()
    {
      base.SetUp ();
      _order1 = (Order) LifetimeService.GetObjectReference (ActiveSubTransaction, DomainObjectIDs.Order1);
    }

    [Test]
    public void PropertyValueReadInInactiveRootTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataAvailable (_order1.ID);

      var value = InactiveRootTransaction.Execute (() => _order1.OrderNumber);

      Assert.That (value, Is.EqualTo (1));
    }

    [Test]
    public void PropertyValueReadInInactiveMiddleTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataAvailable (_order1.ID);

      var value = InactiveMiddleTransaction.Execute (() => _order1.OrderNumber);

      Assert.That (value, Is.EqualTo (1));
    }

    [Test]
    [Ignore ("TODO 4992")]
    public void PropertyValueReadInInactiveRootTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      var value = InactiveRootTransaction.Execute (() => _order1.OrderNumber);
      
      Assert.That (value, Is.EqualTo (1));

      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);
    }

    [Test]
    [Ignore ("TODO 4992")]
    public void PropertyValueReadInInactiveMiddleTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      var value = InactiveMiddleTransaction.Execute (() => _order1.OrderNumber);

      Assert.That (value, Is.EqualTo (1));

      CheckDataLoaded (InactiveRootTransaction, _order1);
      CheckDataLoaded (InactiveMiddleTransaction, _order1);
      CheckDataNotLoaded (ActiveSubTransaction, _order1);
    }
  }
}