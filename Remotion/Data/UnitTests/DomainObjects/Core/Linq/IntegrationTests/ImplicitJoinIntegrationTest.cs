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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class ImplicitJoinIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithMemberFromClause_WithJoin ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          from oi in ot.Order.OrderItems
          where ot.Order.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }
  }
}