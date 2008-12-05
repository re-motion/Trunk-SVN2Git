// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
