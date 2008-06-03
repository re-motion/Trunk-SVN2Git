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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Queries
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
