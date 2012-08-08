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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class CustomQueryTest : InactiveTransactionsTestBase
  {
    [Test]
    public void QueryInInactiveRootTransaction_IsAllowed ()
    {
      var resultSet = InactiveRootTransaction.Execute (
          () => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).Select (o => new { o.OrderNumber }).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void QueryInInactiveMiddleTransaction_IsAllowed ()
    {
      var resultSet = InactiveMiddleTransaction.Execute (
          () => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).Select (o => new { o.OrderNumber }).ToList ());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].OrderNumber, Is.EqualTo (1));
    }
  }
}