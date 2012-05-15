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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Queries
{
  [TestFixture]
  public class LinqQueryTest : QueryTestBase
  {
    [Test]
    public void LinqQuery_CallsFilterQueryResult ()
    {
      var extensionKey = "LinqQuery_CallsFilterQueryResult_Key";
      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      extensionMock.Stub (stub => stub.Key).Return (extensionKey);
      extensionMock
          .Expect (mock => mock.FilterQueryResult (Arg.Is (TestableClientTransaction), Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (TestQueryFactory.CreateTestQueryResult<DomainObject>());
      
      TestableClientTransaction.Extensions.Add (extensionMock);
      try
      {
        var query = from o in QueryFactory.CreateLinqQuery<Order>() where o.Customer.ID == DomainObjectIDs.Customer1 select o;
        query.ToArray();

        extensionMock.VerifyAllExpectations();
      }
      finally
      {
        TestableClientTransaction.Extensions.Remove (extensionKey);
      }
    }

    [Ignore ("TODO RM-4855: enable")]
    [Test]
    public void LinqCustomQuery_CallsFilterQueryResult ()
    {
      var extensionKey = "LinqCustomQuery_CallsFilterQueryResult_Key";
      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension> ();
      extensionMock.Stub (stub => stub.Key).Return (extensionKey);
      extensionMock
          .Expect (mock => mock.FilterCustomQueryResult (Arg.Is (TestableClientTransaction), Arg<IQuery>.Is.Anything, Arg<IEnumerable<int>>.Is.Anything))
          .Return (new[]{1, 2, 3});

      TestableClientTransaction.Extensions.Add (extensionMock);
      try
      {
        var query = from o in QueryFactory.CreateLinqQuery<Order> () select o.OrderNumber;
        query.ToArray ();

        extensionMock.VerifyAllExpectations ();
      }
      finally
      {
        TestableClientTransaction.Extensions.Remove (extensionKey);
      }
    }

  }
}