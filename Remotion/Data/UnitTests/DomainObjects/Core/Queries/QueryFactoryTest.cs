// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryFactoryTest : StandardMappingTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;

    public override void SetUp ()
    {
      base.SetUp ();
      _serviceLocatorProviderBackup = (ServiceLocatorProvider) PrivateInvoke.GetNonPublicStaticField (typeof (ServiceLocator), "currentProvider");
    }

    public override void TearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ServiceLocator), "currentProvider", _serviceLocatorProviderBackup);
      base.TearDown ();
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
      var parameterCollection = new QueryParameterCollection();

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
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateQueryFromConfiguration (definition.ID, parameterCollection);
      Assert.That (query.CollectionType, Is.EqualTo (definition.CollectionType));
      Assert.That (query.ID, Is.EqualTo (definition.ID));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (definition.QueryType));
      Assert.That (query.Statement, Is.EqualTo (definition.Statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (definition.StorageProviderID));
    }

    [Test]
    public void CreateScalarQuery ()
    {
      var id = "id";
      var storageProviderID = "spID";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateScalarQuery (id, storageProviderID, statement, parameterCollection);
      Assert.That (query.CollectionType, Is.Null);
      Assert.That (query.ID, Is.EqualTo (id));
      Assert.That (query.Parameters, Is.SameAs (parameterCollection));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
      Assert.That (query.Statement, Is.EqualTo (statement));
      Assert.That (query.StorageProviderID, Is.EqualTo (storageProviderID));
    }

    [Test]
    public void CreateCollectionQuery ()
    {
      var id = "id";
      var storageProviderID = "spID";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection();
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
    public void CreateQuery_FromLinqQuery ()
    {
      var queryable = from o in QueryFactory.CreateLinqQuery<Order>()
                      where o.OrderNumber > 1
                      select o;

      IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);
      Assert.That (query.Statement, Is.EqualTo (
        "SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[OrderNo],[t0].[DeliveryDate],[t0].[OfficialID],[t0].[CustomerID],[t0].[CustomerIDClassID] "
        +"FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] > @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.ID, Is.EqualTo ("<dynamico queryo>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    public void CreateLinqQuery_WithMixedStages ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<DefaultSqlPreparationStage>().AddMixin<TestSqlPreparationStageMixin>()
          .ForClass<DefaultMappingResolutionStage>().AddMixin<TestMappingResolutionStageMixin>()
          .ForClass<DefaultSqlGenerationStage>().AddMixin<TestSqlGenerationStageMixin>()
          .EnterScope())
      {
        var queryable = from o in QueryFactory.CreateLinqQuery<Order>()
                        select o;
        IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);

        Assert.That (query.Statement, Is.EqualTo ("Value added by generation mixin"));
      }
    }

    [Test]
    public void CreateLinqQuery_WithStages ()
    {
      var preparationStageMock = MockRepository.GenerateMock<ISqlPreparationStage>();
      var resolutionStageMock = MockRepository.GenerateMock<IMappingResolutionStage>();
      var generationStageMock = MockRepository.GenerateMock<ISqlGenerationStage>();
      var nodeTypeRegistry = MethodCallExpressionNodeTypeRegistry.CreateDefault();

      var queryable = QueryFactory.CreateLinqQuery<Order> (preparationStageMock, resolutionStageMock, generationStageMock, nodeTypeRegistry);
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry, Is.SameAs (nodeTypeRegistry));

      var sqlStatementBuilder = new SqlStatementBuilder();
      sqlStatementBuilder.DataInfo = new StreamedScalarValueInfo (typeof (string));
      sqlStatementBuilder.SelectProjection = new SqlEntityDefinitionExpression (
          typeof (int), "c", "CookTable", new SqlColumnDefinitionExpression (typeof (int), "c", "ID", false));
      var sqlStatement = sqlStatementBuilder.GetSqlStatement();

      preparationStageMock
          .Expect (mock => mock.PrepareSqlStatement (Arg<QueryModel>.Is.Anything, Arg<ISqlPreparationContext>.Is.Anything))
          .Return (sqlStatement);
      resolutionStageMock
          .Expect (mock => mock.ResolveSqlStatement (Arg<SqlStatement>.Matches (s => s == sqlStatement), Arg<IMappingResolutionContext>.Is.Anything))
          .Return (sqlStatement);
      generationStageMock
          .Expect (
              mock => mock.GenerateTextForOuterSqlStatement (
                  Arg<SqlCommandBuilder>.Is.Anything,
                  Arg<SqlStatement>.Is.Anything))
          .WhenCalled (mi =>
          {
            var commandBuilder = ((SqlCommandBuilder) mi.Arguments[0]);
            commandBuilder.Append ("test");
            commandBuilder.SetInMemoryProjectionBody (Expression.Constant ("test"));
          });

      preparationStageMock.Replay();

      IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);

      Assert.That (query.Statement, Is.EqualTo ("test"));

      preparationStageMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateQuery_FromLinqQuery_WithEagerFetching ()
    {
      var queryable = (from o in QueryFactory.CreateLinqQuery<Order>()
                       where o.OrderNumber > 1
                       select o).FetchMany (o => o.OrderItems);

      IQuery query = QueryFactory.CreateQuery ("<dynamico queryo>", queryable);
      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      Assert.That (query.EagerFetchQueries.Single().Key.PropertyName, Is.EqualTo (typeof (Order).FullName + ".OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given queryable must stem from an instance of DomainObjectQueryable. Instead, "
                          +
                          "it is of type 'EnumerableQuery`1', with a query provider of type 'EnumerableQuery`1'. Be sure to use QueryFactory.CreateLinqQuery to "
                          + "create the queryable instance, and only use standard query methods on it.\r\nParameter name: queryable")]
    public void CreateQuery_FromLinqQuery_InvalidQueryable ()
    {
      var queryable = new int[0].AsQueryable();
      QueryFactory.CreateQuery ("<dynamic query>", queryable);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given queryable must stem from an instance of DomainObjectQueryable. Instead, "
                          +
                          "it is of type 'TestQueryable`1', with a query provider of type 'DefaultQueryProvider'. Be sure to use QueryFactory.CreateLinqQuery to "
                          + "create the queryable instance, and only use standard query methods on it.\r\nParameter name: queryable")]
    public void CreateQuery_FromLinqQuery_InvalidQueryExecutor ()
    {
      var queryable = new TestQueryable<int> (MockRepository.GenerateMock<IQueryExecutor>()).AsQueryable();
      QueryFactory.CreateQuery ("<dynamic query>", queryable);
    }

    [Test]
    public void CreateMethodCallExpressionNodeTypeRegistry_RegistersDefaultNodes ()
    {
      var selectMethod = SelectExpressionNode.SupportedMethods[0];
      var nodeTypeRegistry = CallCreateNodeTypeRegistry ();

      Assert.That (nodeTypeRegistry.GetNodeType (selectMethod), Is.SameAs (typeof (SelectExpressionNode)));
    }

    [Test]
    public void CreateMethodCallExpressionNodeTypeRegistry_RegistersContainsObject ()
    {
      var containsObjectMethod = typeof (DomainObjectCollection).GetMethod ("ContainsObject");
      var nodeTypeRegistry = CallCreateNodeTypeRegistry ();
      
      Assert.That (nodeTypeRegistry.GetNodeType (containsObjectMethod), Is.SameAs (typeof (ContainsObjectExpressionNode)));
    }

    [Test]
    public void CreateMethodCallExpressionNodeTypeRegistry_RegistersFetchObject ()
    {
      var fetchOneMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("FetchOne");
      var fetchManyMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("FetchMany");
      var thenFetchOneMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchOne");
      var thenFetchManyMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchMany");

      var nodeTypeRegistry = CallCreateNodeTypeRegistry ();

      Assert.That (nodeTypeRegistry.GetNodeType (fetchOneMethod), Is.SameAs (typeof (FetchOneExpressionNode)));
      Assert.That (nodeTypeRegistry.GetNodeType (fetchManyMethod), Is.SameAs (typeof (FetchManyExpressionNode)));
      Assert.That (nodeTypeRegistry.GetNodeType (thenFetchOneMethod), Is.SameAs (typeof (ThenFetchOneExpressionNode)));
      Assert.That (nodeTypeRegistry.GetNodeType (thenFetchManyMethod), Is.SameAs (typeof (ThenFetchManyExpressionNode)));
    }

    [Test]
    public void CreateMethodCallExpressionNodeTypeRegistry_UsesCustomizers ()
    {
      var customMethod1 = typeof (object).GetMethod ("ToString");
      var customMethod2 = typeof (object).GetMethod ("GetHashCode");

      var customizer1 = CreateCustomizerStub (
          stub => stub.GetCustomNodeTypes (), 
          new[] { Tuple.Create<IEnumerable<MethodInfo>, Type> (new[] { customMethod1 }, typeof (SelectExpressionNode)) });
      var customizer2 = CreateCustomizerStub (
          stub => stub.GetCustomNodeTypes (), 
          new[] { Tuple.Create<IEnumerable<MethodInfo>, Type> (new[] { customMethod2 }, typeof (WhereExpressionNode)) });

      PrepareServiceLocatorWithParserCustomizers (customizer1, customizer2);

      var nodeTypeRegistry = CallCreateNodeTypeRegistry ();

      Assert.That (nodeTypeRegistry.GetNodeType (customMethod1), Is.SameAs (typeof (SelectExpressionNode)));
      Assert.That (nodeTypeRegistry.GetNodeType (customMethod2), Is.SameAs (typeof (WhereExpressionNode)));
    }

    [Test]
    public void CreateMethodCallExpressionNodeTypeRegistry_CustomizersCanOverrideDefaults ()
    {
      var exstingMethod = SelectExpressionNode.SupportedMethods[0];
      var defaultNodeTypeRegistry = CallCreateNodeTypeRegistry ();

      Assert.That (defaultNodeTypeRegistry.GetNodeType (exstingMethod), Is.SameAs (typeof (SelectExpressionNode)));

      var customizer = CreateCustomizerStub (
          stub => stub.GetCustomNodeTypes (),
          new[] { Tuple.Create<IEnumerable<MethodInfo>, Type> (new[] { exstingMethod }, typeof (WhereExpressionNode)) });

      PrepareServiceLocatorWithParserCustomizers (customizer);

      var nodeTypeRegistryWithOverrides = CallCreateNodeTypeRegistry ();

      Assert.That (nodeTypeRegistryWithOverrides.GetNodeType (exstingMethod), Is.SameAs (typeof (WhereExpressionNode)));
    }

    [Test]
    public void CreateMethodCallTransformerRegistry_RegistersDefaultTransformers ()
    {
      var toStringMethod = ToStringMethodCallTransformer.SupportedMethods[0];
      var provider = CallCreateMethodCallTransformerProvider ();

      Assert.That (provider.Providers.Length, Is.EqualTo (3));

      Assert.That (provider.Providers[0], Is.TypeOf (typeof (MethodInfoBasedMethodCallTransformerRegistry)));
      Assert.That (provider.Providers[1], Is.TypeOf (typeof (AttributeBasedMethodCallTransformerProvider)));
      Assert.That (provider.Providers[2], Is.TypeOf (typeof (NameBasedMethodCallTransformerRegistry)));

      Assert.That (((MethodInfoBasedMethodCallTransformerRegistry) provider.Providers[0]).GetItem (toStringMethod), 
          Is.TypeOf (typeof (ToStringMethodCallTransformer)));
    }

    [Test]
    public void CreateMethodCallTransformerRegistry_UsesCustomizers ()
    {
      var customMethod1 = typeof (object).GetMethod ("ToString");
      var customTransformer1 = MockRepository.GenerateStub<IMethodCallTransformer> ();
      var customMethod2 = typeof (object).GetMethod ("GetHashCode");
      var customTransformer2 = MockRepository.GenerateStub<IMethodCallTransformer> ();

      var customizer1 = CreateCustomizerStub (
          stub => stub.GetCustomMethodCallTransformers (), 
          new[] { Tuple.Create<IEnumerable<MethodInfo>, IMethodCallTransformer> (new[] { customMethod1 }, customTransformer1 )});
      var customizer2 = CreateCustomizerStub (
          stub => stub.GetCustomMethodCallTransformers (),
          new[] { Tuple.Create<IEnumerable<MethodInfo>, IMethodCallTransformer> (new[] { customMethod2 }, customTransformer2) });

      PrepareServiceLocatorWithParserCustomizers (customizer1, customizer2);

      var nodeTypeRegistry = CallCreateMethodCallTransformerProvider ();

      Assert.That (((MethodInfoBasedMethodCallTransformerRegistry) nodeTypeRegistry.Providers[0]).GetItem (customMethod1), Is.SameAs (customTransformer1));
      Assert.That (((MethodInfoBasedMethodCallTransformerRegistry) nodeTypeRegistry.Providers[0]).GetItem (customMethod2), Is.SameAs (customTransformer2));
    }

    [Test]
    public void CreateMethodCallTransformerRegistry_CustomizersCanOverrideDefaults ()
    {
      var existingMethod = typeof (object).GetMethod ("ToString");
      var defaultRegistry = CallCreateMethodCallTransformerProvider ();

      Assert.That (((MethodInfoBasedMethodCallTransformerRegistry) defaultRegistry.Providers[0]).GetItem (existingMethod), Is.TypeOf (typeof (ToStringMethodCallTransformer)));

      var customTransformer = MockRepository.GenerateStub<IMethodCallTransformer> ();
      var customizer = CreateCustomizerStub (
          stub => stub.GetCustomMethodCallTransformers (),
          new[] { Tuple.Create<IEnumerable<MethodInfo>, IMethodCallTransformer> (new[] { existingMethod }, customTransformer) });

      PrepareServiceLocatorWithParserCustomizers (customizer);

      var registryWithOverrides = CallCreateMethodCallTransformerProvider ();

      Assert.That (((MethodInfoBasedMethodCallTransformerRegistry) registryWithOverrides.Providers[0]).GetItem(existingMethod), Is.SameAs (customTransformer));
    }

    [Test]
    public void CreateResultOperatorHandlerRegistry_RegistersDefaultTransformers ()
    {
      var nodeTypeRegistry = CallCreateResultOperatorHandlerRegistry ();

      Assert.That (nodeTypeRegistry.GetItem (typeof (CountResultOperator)), Is.TypeOf (typeof (CountResultOperatorHandler)));
    }

    [Test]
    public void CreateResultOperatorHandlerRegistry_RegistersFetchTransformer ()
    {
      var nodeTypeRegistry = CallCreateResultOperatorHandlerRegistry ();

      Assert.That (nodeTypeRegistry.GetItem (typeof (FetchOneRequest)), Is.TypeOf (typeof (FetchResultOperatorHandler)));
      Assert.That (nodeTypeRegistry.GetItem (typeof (FetchManyRequest)), Is.TypeOf (typeof (FetchResultOperatorHandler)));
    }

    [Test]
    public void CreateResultOperatorHandlerRegistry_UsesCustomizers ()
    {
      var customType1 = typeof (CountResultOperator);
      var customTransformer1 = MockRepository.GenerateStub<IResultOperatorHandler> ();
      var customType2 = typeof (CastResultOperator);
      var customTransformer2 = MockRepository.GenerateStub<IResultOperatorHandler> ();

      var customizer1 = CreateCustomizerStub (
          stub => stub.GetCustomResultOperatorHandlers (),
          new[] { Tuple.Create<IEnumerable<Type>, IResultOperatorHandler> (new[] { customType1 }, customTransformer1) });
      var customizer2 = CreateCustomizerStub (
          stub => stub.GetCustomResultOperatorHandlers (),
          new[] { Tuple.Create<IEnumerable<Type>, IResultOperatorHandler> (new[] { customType2 }, customTransformer2) });

      PrepareServiceLocatorWithParserCustomizers (customizer1, customizer2);

      var nodeTypeRegistry = CallCreateResultOperatorHandlerRegistry ();

      Assert.That (nodeTypeRegistry.GetItem (customType1), Is.SameAs (customTransformer1));
      Assert.That (nodeTypeRegistry.GetItem (customType2), Is.SameAs (customTransformer2));
    }

    [Test]
    public void CreateResultOperatorHandlerRegistry_CustomizersCanOverrideDefaults ()
    {
      var existingType = typeof (CountResultOperator);
      var defaultRegistry = CallCreateResultOperatorHandlerRegistry ();

      Assert.That (defaultRegistry.GetItem (existingType), Is.TypeOf (typeof (CountResultOperatorHandler)));

      var customTransformer = MockRepository.GenerateStub<IResultOperatorHandler>();
      var customizer = CreateCustomizerStub (
          stub => stub.GetCustomResultOperatorHandlers(),
          new[] { Tuple.Create<IEnumerable<Type>, IResultOperatorHandler> (new[] { existingType }, customTransformer) });

      PrepareServiceLocatorWithParserCustomizers (customizer);

      var registryWithOverrides = CallCreateResultOperatorHandlerRegistry ();

      Assert.That (registryWithOverrides.GetItem (existingType), Is.SameAs (customTransformer));
    }

    private MethodCallExpressionNodeTypeRegistry CallCreateNodeTypeRegistry ()
    {
      return (MethodCallExpressionNodeTypeRegistry) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (QueryFactory), "CreateNodeTypeRegistry");
    }

    private CompoundMethodCallTransformerProvider CallCreateMethodCallTransformerProvider ()
    {
      return (CompoundMethodCallTransformerProvider) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (QueryFactory), "CreateMethodCallTransformerProvider");
    }

    private ResultOperatorHandlerRegistry CallCreateResultOperatorHandlerRegistry ()
    {
      return (ResultOperatorHandlerRegistry) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (QueryFactory), "CreateResultOperatorHandlerRegistry");
    }

    private ILinqParserCustomizer CreateCustomizerStub<T> (Function<ILinqParserCustomizer, T> stubExpectation, T results)
    {
      var customizerStub = MockRepository.GenerateStub<ILinqParserCustomizer> ();
      customizerStub.Stub (stubExpectation).Return (results);
      customizerStub.Replay ();
      return customizerStub;
    }

    private void PrepareServiceLocatorWithParserCustomizers (params ILinqParserCustomizer[] extensionFactories)
    {
      var serviceLocatorStub = MockRepository.GenerateStub<IServiceLocator> ();
      serviceLocatorStub.Stub (stub => stub.GetAllInstances<ILinqParserCustomizer> ()).Return (extensionFactories);
      serviceLocatorStub.Replay ();
      ServiceLocator.SetLocatorProvider (() => serviceLocatorStub);
    }
  }
}