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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class CollectionQueryTest : InactiveTransactionsTestBase
  {
    [Test]
    public void QueryInInactiveRootTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      var resultSet = InactiveRootTransaction.Execute (() => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.OrderNumber == 1).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void QueryInInactiveMiddleTransaction_WithoutLoading_IsAllowed ()
    {
      ActiveSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      var resultSet = InactiveMiddleTransaction.Execute (() => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).ToList ());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    [Ignore ("TODO 4992")]
    public void QueryInInactiveRootTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      var resultSet = InactiveRootTransaction.Execute (() => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).ToList ());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (InactiveRootTransaction, resultSet[0]);
      CheckDataNotLoaded (InactiveMiddleTransaction, resultSet[0]);
      CheckDataNotLoaded (ActiveSubTransaction, resultSet[0]);
    }

    [Test]
    [Ignore ("TODO 4992")]
    public void QueryInInactiveMiddleTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (InactiveRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (InactiveMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ActiveSubTransaction, DomainObjectIDs.Order1);

      var resultSet = InactiveMiddleTransaction.Execute (() => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).ToList ());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (InactiveRootTransaction, resultSet[0]);
      CheckDataLoaded (InactiveMiddleTransaction, resultSet[0]);
      CheckDataNotLoaded (ActiveSubTransaction, resultSet[0]);
    }
  }
}