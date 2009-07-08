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
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq.Backend;
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
      var queryDefinition =
          new QueryDefinition ("bla", orderClassDefinition.StorageProviderID, "SELECT * FROM CeoView WHERE Contains ([CeoView].[Name], 'Fischer')", QueryType.Collection);
      var query = QueryFactory.CreateQuery (queryDefinition);

      var orders = ClientTransactionMock.QueryManager.GetCollection<Ceo> (query).AsEnumerable();
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
