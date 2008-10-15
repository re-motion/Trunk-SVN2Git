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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class QueryProviderTest
  {
    [Test]
    public void CreateQuery()
    {
      var sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
      var executor = new QueryExecutor<Supplier> (sqlGenerator);
      var provider = new QueryProvider (executor);
      IQueryable<Supplier> query = from supplier in QueryFactory.CreateLinqQuery<Supplier>() select supplier;

      IQueryable<Supplier> queryCreatedByProvider = provider.CreateQuery<Supplier> (query.Expression);
      Assert.IsNotNull (queryCreatedByProvider);
      Assert.IsInstanceOfType (typeof (DomainObjectQueryable<Supplier>), queryCreatedByProvider);
    }
  }
}