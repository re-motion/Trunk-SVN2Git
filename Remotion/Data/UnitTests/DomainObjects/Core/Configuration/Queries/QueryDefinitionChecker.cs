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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Queries
{
  public class QueryDefinitionChecker
  {
    public void Check (QueryDefinitionCollection expectedQueries, QueryDefinitionCollection actualQueries)
    {
      Assert.AreEqual (expectedQueries.Count, actualQueries.Count, "Number of queries does not match.");

      foreach (QueryDefinition expectedQuery in expectedQueries)
      {
        QueryDefinition actualQuery = actualQueries[expectedQuery.ID];
        CheckQuery (expectedQuery, actualQuery);
      }
    }

    private void CheckQuery (QueryDefinition expectedQuery, QueryDefinition actualQuery)
    {
      Assert.AreEqual (
          expectedQuery.StorageProviderID,
          actualQuery.StorageProviderID,
          "ProviderID of query definition {0} does not match.",
          expectedQuery.ID);

      Assert.AreEqual (expectedQuery.Statement, actualQuery.Statement, "Statement of query definition {0} does not match.", expectedQuery.ID);

      Assert.AreEqual (expectedQuery.QueryType, actualQuery.QueryType, "QueryType of query definition {0} does not match.", expectedQuery.ID);

      Assert.AreEqual (
          expectedQuery.CollectionType,
          actualQuery.CollectionType,
          "CollectionType of query definition {0} does not match.",
          expectedQuery.ID);
    }
  }
}
