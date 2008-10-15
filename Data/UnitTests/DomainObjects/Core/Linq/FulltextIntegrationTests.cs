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
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq.ExtensionMethods;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class FulltextIntegrationTests : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      SetDatabaseModifyable ();
      DatabaseAgent.ExecuteBatch ("DataDomainObjects_DropFulltextIndices.sql", false);
      DatabaseAgent.ExecuteBatch ("DataDomainObjects_CreateFulltextIndices.sql", false);
      WaitForIndices ();
    }

    public override void TestFixtureTearDown ()
    {
      DatabaseAgent.ExecuteBatch ("DataDomainObjects_DropFulltextIndices.sql", false);
      base.TestFixtureTearDown ();
    }

    [Test]
    public void Fulltext_Spike ()
    {
      ClassDefinition orderClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      QueryDefinition queryDefinition =
          new QueryDefinition ("bla", orderClassDefinition.StorageProviderID, "SELECT * FROM CeoView WHERE Contains ([CeoView].[Name], 'Fischer')", QueryType.Collection);
      var query = QueryFactory.CreateQuery (queryDefinition);

      var orders = ClientTransactionMock.QueryManager.GetCollection<Ceo> (query).Cast<Ceo> ();
      IntegrationTests.CheckQueryResult (orders, DomainObjectIDs.Ceo4);
    }

    [Test]
    public void QueryWithContainsFullText ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo>()
                 where c.Name.ContainsFulltext ("Fischer")
                 select c;
      IntegrationTests.CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
    }

    private void WaitForIndices ()
    {
      var rowCount = DatabaseAgent.ExecuteScalarCommand ("SELECT COUNT(*) FROM CeoView WHERE Contains ([CeoView].[Name], 'Fischer')");
      if (!rowCount.Equals (1))
      {
        Thread.Sleep (100);
        WaitForIndices();
      }
    }
  }
}