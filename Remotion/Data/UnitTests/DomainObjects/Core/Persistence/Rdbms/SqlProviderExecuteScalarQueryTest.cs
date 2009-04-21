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
