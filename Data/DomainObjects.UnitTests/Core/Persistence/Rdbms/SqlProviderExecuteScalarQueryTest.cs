using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderExecuteScalarQueryTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void ScalarQueryWithoutParameter ()
    {
      Assert.AreEqual (42, Provider.ExecuteScalarQuery (new Query ("QueryWithoutParameter")));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void InvalidScalarQuery ()
    {
      QueryDefinition definition = new QueryDefinition ("InvalidQuery", c_testDomainProviderID, "This is not T-SQL", QueryType.Scalar);

      Provider.ExecuteScalarQuery (new Query (definition));
    }

    [Test]
    public void ScalarQueryWithParameter ()
    {
      Query query = new Query ("OrderNoSumByCustomerNameQuery");
      query.Parameters.Add ("@customerName", "Kunde 1");

      Assert.AreEqual (3, Provider.ExecuteScalarQuery (query));
    }

    [Test]
    public void ParameterWithTextReplacement ()
    {
      Query query = new Query ("OrderNoSumForMultipleCustomers");
      query.Parameters.Add ("{companyNames}", "'Kunde 1', 'Kunde 3'", QueryParameterType.Text);

      Assert.AreEqual (6, Provider.ExecuteScalarQuery (query));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected query type is 'Scalar', but was 'Collection'.\r\nParameter name: query")]
    public void CollectionQuery ()
    {
      Provider.ExecuteScalarQuery (new Query ("OrderQuery"));
    }

    [Test]
    public void BulkUpdateQuery ()
    {
      Query query = new Query ("BulkUpdateQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

      Assert.AreEqual (2, Provider.ExecuteScalarQuery (query));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void DifferentStorageProviderID ()
    {
      QueryDefinition definition = new QueryDefinition (
          "QueryWithDifferentStorageProviderID",
          "DifferentStorageProviderID",
          "select 42",
          QueryType.Scalar);

      Provider.ExecuteScalarQuery (new Query (definition));
    }
  }
}
