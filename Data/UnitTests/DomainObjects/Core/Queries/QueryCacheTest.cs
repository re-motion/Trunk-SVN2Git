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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixtureAttribute]
  public class QueryCacheTest
  {
    private QueryCache _cache;

    [SetUp]
    public void SetUp()
    {
      _cache = new QueryCache();
    }

    [Test]
    public void GetOrCreateQuery_Uncached()
    {
      IQuery query = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      Assert.That (query.Statement, Is.EqualTo ("SELECT [o].* FROM [OrderView] [o] WHERE ([o].[OrderNo] > @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.ID, Is.EqualTo ("id"));
    }

    [Test]
    public void GetOrCreateQuery_Cached ()
    {
      IQuery query1 = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);
      IQuery query2 = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      Assert.That (query1, Is.SameAs (query2));
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      IQuery query = _cache.GetQuery<Order> ("id", orders => from o in orders where o.OrderNumber > 1 select o);

      var clientTransactionStub = new ClientTransactionMock ();
      var queryManagerMock = MockRepository.GenerateMock<IQueryManager> ();
      clientTransactionStub.SetQueryManager (queryManagerMock);

      var expectedResult = new ObjectList<Order> ();
      queryManagerMock.Expect (mock => mock.GetCollection<Order> (query)).Return (expectedResult);
      queryManagerMock.Replay ();

      var result = _cache.ExecuteCollectionQuery<Order> (clientTransactionStub, "id", orders => from o in orders where o.OrderNumber > 1 select o);

      queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expectedResult));
    }
  }
}