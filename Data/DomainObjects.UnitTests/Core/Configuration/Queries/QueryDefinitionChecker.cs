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