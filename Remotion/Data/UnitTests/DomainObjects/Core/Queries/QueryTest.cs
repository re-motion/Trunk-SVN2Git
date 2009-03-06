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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryTest : StandardMappingTest
  {
    [Test]
    public void InitializeWithQueryID ()
    {
      var parameters = new QueryParameterCollection ();
      var query = (Query) QueryFactory.CreateQueryFromConfiguration ("OrderQuery", parameters);

      QueryDefinition definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions["OrderQuery"];
      Assert.AreSame (definition, query.Definition);
      Assert.AreEqual (definition.ID, query.ID);
      Assert.AreEqual (definition.CollectionType, query.CollectionType);
      Assert.AreEqual (definition.QueryType, query.QueryType);
      Assert.AreEqual (definition.Statement, query.Statement);
      Assert.AreEqual (definition.StorageProviderID, query.StorageProviderID);
      Assert.AreSame (parameters, query.Parameters);
    }

    [Test]
    public void InitializeWithQueryDefinition ()
    {
      var parameters = new QueryParameterCollection ();

      QueryDefinition definition = TestQueryFactory.CreateOrderQueryWithCustomCollectionType ();
      var query = new Query (definition, parameters);

      Assert.AreSame (definition, query.Definition);
      Assert.AreEqual (definition.ID, query.ID);
      Assert.AreSame (parameters, query.Parameters);
    }

    [Test]
    public void EagerFetchQueries ()
    {
      QueryDefinition definition = TestQueryFactory.CreateOrderQueryWithCustomCollectionType ();
      var query1 = new Query (definition, new QueryParameterCollection ());

      Assert.That (query1.EagerFetchQueries, Is.Not.Null);
      Assert.That (query1.EagerFetchQueries.Count, Is.EqualTo (0));

      var query2 = new Query (definition, new QueryParameterCollection ());
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      query1.EagerFetchQueries.Add (endPointDefinition, query2);
      Assert.That (query1.EagerFetchQueries.Count, Is.EqualTo (1));
    }

    [Test]
    public void EagerFetchQueries_Recursive ()
    {
      QueryDefinition definition = TestQueryFactory.CreateOrderQueryWithCustomCollectionType ();
      var query1 = new Query (definition, new QueryParameterCollection ());

      Assert.That (query1.EagerFetchQueries, Is.Not.Null);
      Assert.That (query1.EagerFetchQueries.Count, Is.EqualTo (0));

      var query2 = new Query (definition, new QueryParameterCollection ());
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      query1.EagerFetchQueries.Add (endPointDefinition, query2);
      Assert.That (query1.EagerFetchQueries.Count, Is.EqualTo (1));

      var query3 = new Query (definition, new QueryParameterCollection ());
      query2.EagerFetchQueries.Add (endPointDefinition, query3);
      Assert.That (query2.EagerFetchQueries.Count, Is.EqualTo (1));

    }
  }
}
