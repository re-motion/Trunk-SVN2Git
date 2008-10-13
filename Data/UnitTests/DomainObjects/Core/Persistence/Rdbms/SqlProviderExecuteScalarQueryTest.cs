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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
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
      Assert.AreEqual (42, Provider.ExecuteScalarQuery (QueryFactory.CreateQueryFromConfiguration ("QueryWithoutParameter")));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void InvalidScalarQuery ()
    {
      QueryDefinition definition = new QueryDefinition ("InvalidQuery", c_testDomainProviderID, "This is not T-SQL", QueryType.Scalar);

      Provider.ExecuteScalarQuery (QueryFactory.CreateQuery (definition));
    }

    [Test]
    public void ScalarQueryWithParameter ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery");
      query.Parameters.Add ("@customerName", "Kunde 1");

      Assert.AreEqual (3, Provider.ExecuteScalarQuery (query));
    }

    [Test]
    public void ParameterWithTextReplacement ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumForMultipleCustomers");
      query.Parameters.Add ("{companyNames}", "'Kunde 1', 'Kunde 3'", QueryParameterType.Text);

      Assert.AreEqual (6, Provider.ExecuteScalarQuery (query));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected query type is 'Scalar', but was 'Collection'.\r\nParameter name: query")]
    public void CollectionQuery ()
    {
      Provider.ExecuteScalarQuery (QueryFactory.CreateQueryFromConfiguration ("OrderQuery"));
    }

    [Test]
    public void BulkUpdateQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("BulkUpdateQuery");
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

      Provider.ExecuteScalarQuery (QueryFactory.CreateQuery (definition));
    }
  }
}
