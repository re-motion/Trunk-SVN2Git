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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq.ExtensionMethods;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryFactoryTest : StandardMappingTest
  {
    [Test]
    public void CreateLinqQuery_WithSqlGenerator ()
    {
      var sqlGeneratorMock = MockRepository.GenerateMock<ISqlGenerator> ();
      var queryable = QueryFactory.CreateLinqQuery<Order> (sqlGeneratorMock);
      Assert.That (queryable, Is.Not.Null);
      Assert.That (queryable.GetExecutor (), Is.InstanceOfType (typeof (QueryExecutor<Order>)));
      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.SameAs (sqlGeneratorMock));
    }

    [Test]
    public void CreateLinqQuery_WithImplicitSqlGenerator ()
    {
      var queryable = QueryFactory.CreateLinqQuery<Order> ();
      Assert.That (queryable, Is.Not.Null);
      Assert.That (queryable.GetExecutor (), Is.InstanceOfType (typeof (QueryExecutor<Order>)));
      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.Not.Null);

      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.SameAs (QueryFactory.GetDefaultSqlGenerator (typeof (Order))));
    }

    [Test]
    public void GetDefaultSqlGenerator ()
    {
      var providerID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID;
      var providerDefinition = DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[providerID];
      Assert.That (QueryFactory.GetDefaultSqlGenerator (typeof (Order)), Is.SameAs (providerDefinition.LinqSqlGenerator));
    }

    [Test]
    public void CreateQuery_FromDefinition ()
    {
      var definition = new QueryDefinition ("Test", "x", "y", QueryType.Collection, typeof (OrderCollection));
      
      IQuery query = QueryFactory.CreateQuery (definition);
      Assert.That (query.CollectionType, Is.EqualTo (definition.CollectionType));
      Assert.That (query.ID, Is.EqualTo (definition.ID));
      Assert.That (query.Parameters, Is.Empty);
      Assert.That (query.QueryType, Is.EqualTo (definition.QueryType));
      Assert.That (query.Statement, Is.EqualTo (definition.Statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (definition.StorageProviderID));
    }

    [Test]
    public void CreateQuery_FromDefinition_WithParameterCollection ()
    {
      var definition = new QueryDefinition ("Test", "x", "y", QueryType.Collection, typeof (OrderCollection));
      var parameterCollection = new QueryParameterCollection ();
      
      IQuery query = QueryFactory.CreateQuery (definition, parameterCollection);
      Assert.That (query.CollectionType, Is.EqualTo (definition.CollectionType));
      Assert.That (query.ID, Is.EqualTo (definition.ID));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (definition.QueryType));
      Assert.That (query.Statement, Is.EqualTo (definition.Statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (definition.StorageProviderID));
    }

    [Test]
    public void CreateQueryFromConfiguration_FromID ()
    {
      var definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions[0];

      IQuery query = QueryFactory.CreateQueryFromConfiguration (definition.ID);
      Assert.That (query.CollectionType, Is.EqualTo (definition.CollectionType));
      Assert.That (query.ID, Is.EqualTo (definition.ID));
      Assert.That (query.Parameters, Is.Empty);
      Assert.That (query.QueryType, Is.EqualTo (definition.QueryType));
      Assert.That (query.Statement, Is.EqualTo (definition.Statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (definition.StorageProviderID));
    }

    [Test]
    public void CreateQueryFromConfiguration_FromID_WithParameterCollection ()
    {
      var definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions[0];
      var parameterCollection = new QueryParameterCollection ();

      IQuery query = QueryFactory.CreateQueryFromConfiguration (definition.ID, parameterCollection);
      Assert.That (query.CollectionType, Is.EqualTo (definition.CollectionType));
      Assert.That (query.ID, Is.EqualTo (definition.ID));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (definition.QueryType));
      Assert.That (query.Statement, Is.EqualTo (definition.Statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (definition.StorageProviderID));
    }

    [Test]
    public void CreateScalarQuery()
    {
      var id = "id";
      var storageProviderID = "spID";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection ();

      IQuery query = QueryFactory.CreateScalarQuery (id, storageProviderID, statement, parameterCollection);
      Assert.That (query.CollectionType, Is.Null);
      Assert.That (query.ID, Is.EqualTo (id));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
      Assert.That (query.Statement, Is.EqualTo (statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (storageProviderID));
    }

    [Test]
    public void CreateCollectionQuery()
    {
      var id = "id";
      var storageProviderID = "spID";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection ();
      var collectionType = typeof (OrderCollection);

      IQuery query = QueryFactory.CreateCollectionQuery (id, storageProviderID, statement, parameterCollection, collectionType);
      Assert.That (query.ID, Is.EqualTo (id));
      Assert.That (query.CollectionType, Is.SameAs (collectionType));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
      Assert.That (query.Statement, Is.EqualTo (statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (storageProviderID));
    }

    [Test]
    public void CreateQuery_FromLinqQuery()
    {
      var queryable = from o in QueryFactory.CreateLinqQuery<Order> ()
                      where o.OrderNumber > 1
                      select o;

      IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);
      Assert.That (query.Statement, Is.EqualTo ("SELECT [o].* FROM [OrderView] [o] WHERE ([o].[OrderNo] > @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.ID, Is.EqualTo ("<dynamico queryo>"));
    }

    [Test]
    public void CreateQuery_FromLinqQuery_WithEagerFetching ()
    {
      var queryable = (from o in QueryFactory.CreateLinqQuery<Order> ()
                      where o.OrderNumber > 1
                      select o).FetchMany (o => o.OrderItems);

      IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);
      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      Assert.That (query.EagerFetchQueries.Single().Key.PropertyName, Is.EqualTo (typeof (Order).FullName + ".OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given queryable must stem from an instance of DomainObjectQueryable. Instead, "
        + "it is of type 'EnumerableQuery`1', with a query provider of type 'EnumerableQuery`1'. Be sure to use QueryFactory.CreateLinqQuery to "
        + "create the queryable instance, and only use standard query methods on it.\r\nParameter name: queryable")]
    public void CreateQuery_FromLinqQuery_InvalidQueryable ()
    {
      var queryable = new int[0].AsQueryable ();
      QueryFactory.CreateQuery ("<dynamic query>", queryable);
    }
  }
}
