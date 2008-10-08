/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryFactoryTest : StandardMappingTest
  {
    [Test]
    public void CreateQueryable_WithSqlGenerator ()
    {
      var sqlGeneratorMock = MockRepository.GenerateMock<ISqlGenerator> ();
      var queryable = QueryFactory.CreateQueryable<Order> (sqlGeneratorMock);
      Assert.That (queryable, Is.Not.Null);
      Assert.That (queryable.GetExecutor (), Is.InstanceOfType (typeof (QueryExecutor<Order>)));
      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.SameAs (sqlGeneratorMock));
    }

    [Test]
    public void CreateQueryable_WithImplicitSqlGenerator ()
    {
      var queryable = QueryFactory.CreateQueryable<Order> ();
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
  }
}