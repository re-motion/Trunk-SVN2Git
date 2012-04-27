// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : ClientTransactionBaseTest
  {
    private ClassDefinition _orderClassDefinition;
    private ClassDefinition _customerClassDefinition;
    private DefaultSqlPreparationStage _preparationStage;
    private DefaultMappingResolutionStage _resolutionStage;
    private DefaultSqlGenerationStage _generationStage;
    private QueryModel _order1QueryModel;
    private DomainObjectQueryExecutor _orderExecutor;
    private DomainObjectQueryExecutor _customerExecutor;
    private SqlStorageTypeInformationProvider _sqlStorageTypeInformationProvider;
    private TypeConversionProvider _typeConversionProvider;

    // TODO 4831: Rewrite tests for this class

    public override void SetUp ()
    {
      base.SetUp ();

      _orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _customerClassDefinition = DomainObjectIDs.Customer1.ClassDefinition;
      var storageNameProvider = new ReflectionBasedStorageNameProvider ();
      var resolver = new MappingResolver (
          new StorageSpecificExpressionResolver (new RdbmsPersistenceModelProvider (), storageNameProvider));
      var generator = new UniqueIdentifierGenerator ();
      _preparationStage = new DefaultSqlPreparationStage (
          CompoundMethodCallTransformerProvider.CreateDefault (), ResultOperatorHandlerRegistry.CreateDefault (), generator);
      _resolutionStage = new DefaultMappingResolutionStage (resolver, generator);
      _generationStage = new DefaultSqlGenerationStage ();
      _sqlStorageTypeInformationProvider = new SqlStorageTypeInformationProvider ();
      _typeConversionProvider = TypeConversionProvider.Create ();

      _order1QueryModel = ParseQuery ((from o in QueryFactory.CreateLinqQuery<Order> () where o.OrderNumber == 1 select o).Expression);

      _orderExecutor = new DomainObjectQueryExecutor (
          _orderClassDefinition,
          _sqlStorageTypeInformationProvider,
          new DomainObjectQueryGenerator (new SqlQueryGenerator (_preparationStage, _resolutionStage, _generationStage), _typeConversionProvider));
      _customerExecutor = new DomainObjectQueryExecutor (
          _customerClassDefinition, _sqlStorageTypeInformationProvider,
          new DomainObjectQueryGenerator (new SqlQueryGenerator (_preparationStage, _resolutionStage, _generationStage), _typeConversionProvider));
    }

    [Test]
    public void StorageTypeInformationProvider ()
    {
      Assert.That (_orderExecutor.StorageTypeInformationProvider, Is.SameAs (_sqlStorageTypeInformationProvider));
    }

    [Test]
    public void ExecuteScalar ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Count ());
      QueryModel model = ParseQuery (expression);

      var count = _orderExecutor.ExecuteScalar<int> (model);
      Assert.That (count, Is.EqualTo (6));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteScalar_NoCurrentTransaction ()
    {
      var expression =
          ExpressionHelper.MakeExpression (() => (from o in QueryFactory.CreateLinqQuery<Order> () where o.OrderNumber == 1 select o).Count ());
      QueryModel model = ParseQuery (expression);

      using (ClientTransactionScope.EnterNullScope ())
      {
        _orderExecutor.ExecuteScalar<int> (model);
      }
    }

    [Test]
    public void ExecuteScalar_Boolean ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Any ());
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteScalar<bool> (model);
      Assert.That (result, Is.True);
    }

    [Test]
    public void ExecuteScalar_NullableResult_NonNullableValue ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Count ());
      QueryModel model = ParseQuery (expression);

      var count = _orderExecutor.ExecuteScalar<int?> (model);
      Assert.That (count, Is.EqualTo (6));
    }

    [Test]
    public void ExecuteScalar_NullableResult_NullValue ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Select (x => (int?) null).First ());
      QueryModel model = ParseQuery (expression);

      var count = _orderExecutor.ExecuteScalar<int?> (model);
      Assert.That (count, Is.Null);
    }

    [Test]
    public void ExecuteScalar_WithFetches ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Max ());
      QueryModel queryModel = ParseQuery (expression);

      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      queryModel.ResultOperators.Add (fetchRequest);

      var fakeQuery = QueryFactory.CreateScalarQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderDefinition,
          "SELECT COUNT(*) FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection ());

      var generatorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator>();
      generatorMock
          .Expect (
              mock => mock.CreateQuery (
                  Arg<string>.Is.Anything,
                  Arg.Is (_orderClassDefinition),
                  Arg.Is (queryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything,
                  Arg.Is (QueryType.Scalar)))
          .WhenCalled (
              mi =>
              {
                var actualQueryModel = (QueryModel) mi.Arguments[2];
                Assert.That (actualQueryModel.ResultOperators, Has.No.Member (fetchRequest));

                var rs = (IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3];
                Assert.That (rs.Count (), Is.EqualTo (1));
                Assert.That (rs.Single ().FetchRequest, Is.SameAs (fetchRequest));
              })
          .Return (fakeQuery);

      var executor = new DomainObjectQueryExecutor (_orderClassDefinition, _sqlStorageTypeInformationProvider, generatorMock);
      var result = executor.ExecuteScalar<int> (queryModel);

      Assert.That (result, Is.EqualTo (1));
      generatorMock.VerifyAllExpectations ();
    }

    [Test]
    public void ExecuteScalar_WithFetches_LeavesNonTrailingFetches ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Max ());
      QueryModel queryModel = ParseQuery (expression);

      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      queryModel.ResultOperators.Insert (0, fetchRequest);

      var fakeQuery = QueryFactory.CreateScalarQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderDefinition,
          "SELECT COUNT(*) FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection ());

      var generatorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator> ();
      generatorMock
          .Expect (
              mock => mock.CreateQuery (
                  Arg<string>.Is.Anything,
                  Arg.Is (_orderClassDefinition),
                  Arg.Is (queryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything,
                  Arg.Is (QueryType.Scalar)))
          .WhenCalled (
              mi =>
              {
                var actualQueryModel = (QueryModel) mi.Arguments[2];
                Assert.That (actualQueryModel.ResultOperators, Has.Member (fetchRequest));

                var rs = (IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3];
                Assert.That (rs.Count (), Is.EqualTo (0));
              })
          .Return (fakeQuery);

      var executor = new DomainObjectQueryExecutor (_orderClassDefinition, _sqlStorageTypeInformationProvider, generatorMock);
      executor.ExecuteScalar<int> (queryModel);

      generatorMock.VerifyAllExpectations ();
    }

    [Test]
    public void ExecuteSingle ()
    {
      var expression = ExpressionHelper.MakeExpression (
          () => (from o in QueryFactory.CreateLinqQuery<Order> () orderby o.OrderNumber select o).First ());
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<Order> (model, true);
      Assert.That (result, Is.SameAs (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    public void ExecuteSingle_CanHandleScalarPrimitives ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Max (o => o.OrderNumber));
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<int> (model, true);
      Assert.That (result, Is.EqualTo (6));
    }

    [Test]
    public void ExecuteSingle_CanHandleScalarStrings ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Customer> ().Max (o => o.Name));
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<string> (model, true);
      Assert.That (result, Is.EqualTo ("Kunde 4"));
    }

    [Test]
    public void ExecuteSingle_CanHandleScalarDateTimes ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Max (o => o.DeliveryDate));
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<DateTime> (model, true);
      Assert.That (result, Is.EqualTo (new DateTime (2006, 3, 1)));
    }

    [Test]
    public void ExecuteSingle_CanHandleScalarNullables ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().Max (o => (int?) o.OrderNumber));
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<int?> (model, true);
      Assert.That (result, Is.EqualTo (6));
    }

    [Test]
    public void ExecuteSingle_CanHandleInterfaceDomainObjects ()
    {
      var expression =
          ExpressionHelper.MakeExpression (() => (from o in QueryFactory.CreateLinqQuery<Order> () orderby o.OrderNumber select (IOrder) o).First ());
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<IOrder> (model, true);
      Assert.That (result, Is.SameAs (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteSingle_NoCurrentTransaction ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().First ());
      QueryModel model = ParseQuery (expression);

      using (ClientTransactionScope.EnterNullScope ())
      {
        _orderExecutor.ExecuteSingle<int> (model, true);
      }
    }

    [Test]
    public void ExecuteSingle_DefaultIfEmpty_True ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().First (o => false));
      QueryModel model = ParseQuery (expression);

      var result = _orderExecutor.ExecuteSingle<Order> (model, true);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteSingle_DefaultIfEmpty_False ()
    {
      var expression = ExpressionHelper.MakeExpression (() => QueryFactory.CreateLinqQuery<Order> ().First (o => false));
      QueryModel model = ParseQuery (expression);

      _orderExecutor.ExecuteSingle<Order> (model, false);
    }

    [Test]
    public void ExecuteCollection ()
    {
      IEnumerable<Order> orders = _orderExecutor.ExecuteCollection<Order> (_order1QueryModel);

      var expected = new[]
                     {
                         Order.GetObject (DomainObjectIDs.Order1)
                     };
      Assert.That (orders.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void ExecuteCollection_WithFetches ()
    {
      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      _order1QueryModel.ResultOperators.Add (fetchRequest);

      var fakeQuery = QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderDefinition,
          "SELECT [o].* FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      var generatorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator> ();
      generatorMock
          .Expect (
              mock => mock.CreateQuery (
                  Arg<string>.Is.Anything,
                  Arg.Is (_orderClassDefinition),
                  Arg.Is (_order1QueryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything,
                  Arg.Is (QueryType.Collection)))
          .WhenCalled (
              mi =>
              {
                var actualQueryModel = (QueryModel) mi.Arguments[2];
                Assert.That (actualQueryModel.ResultOperators, Has.No.Member (fetchRequest));

                var rs = (IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3];
                Assert.That (rs.Count (), Is.EqualTo (1));
                Assert.That (rs.Single ().FetchRequest, Is.SameAs (fetchRequest));
              })
          .Return (fakeQuery);

      generatorMock.Replay ();

      var executor = new DomainObjectQueryExecutor (_orderClassDefinition, _sqlStorageTypeInformationProvider, generatorMock);
      var orders = executor.ExecuteCollection<Order> (_order1QueryModel).ToArray ();

      Assert.That (orders, Is.EqualTo (new[] { Order.GetObject (DomainObjectIDs.Order1) }));
      generatorMock.VerifyAllExpectations ();
    }

    [Test]
    public void ExecuteCollection_WithFetches_LeavesNonTrailingFetches ()
    {
      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      _order1QueryModel.ResultOperators.Add (fetchRequest);
      _order1QueryModel.ResultOperators.Add (new DistinctResultOperator ());

      var fakeQuery = QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderDefinition,
          "SELECT [o].* FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));

      var generatorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator> ();
      generatorMock
          .Expect (
              mock => mock.CreateQuery (
                  Arg<string>.Is.Anything,
                  Arg.Is (_orderClassDefinition),
                  Arg.Is (_order1QueryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything,
                  Arg.Is (QueryType.Collection)))
          .WhenCalled (
              mi =>
              {
                var actualQueryModel = (QueryModel) mi.Arguments[2];
                Assert.That (actualQueryModel.ResultOperators, Has.Member (fetchRequest));

                var rs = (IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3];
                Assert.That (rs.Count (), Is.EqualTo (0));
              })
          .Return (fakeQuery);

      generatorMock.Replay ();

      var executor = new DomainObjectQueryExecutor (_orderClassDefinition, _sqlStorageTypeInformationProvider, generatorMock);
      executor.ExecuteCollection<Order> (_order1QueryModel).ToArray ();

      generatorMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteCollection_NoCurrentTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _orderExecutor.ExecuteCollection<Order> (_order1QueryModel);
      }
    }

    //[Test]
    //public void CreateQuery_FromModel_Scalar ()
    //{
    //  var expression = ExpressionHelper.MakeExpression (
    //      () => (from order in QueryFactory.CreateLinqQuery<Order> () where order.OrderNumber == 1 select order).Count ());
    //  var queryModel = ParseQuery (expression);

    //  var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Scalar);
    //  Assert.That (query.Statement, Is.EqualTo ("SELECT COUNT(*) AS [value] FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] = @1)"));
    //  Assert.That (query.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
    //  Assert.That (
    //      query.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (Order)).StorageEntityDefinition.StorageProviderDefinition));
    //  Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
    //  Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
    //}

    //[Test]
    //public void CreateQuery_FromModel_Collection ()
    //{
    //  var queryable = from order in QueryFactory.CreateLinqQuery<Order> () where order.OrderNumber == 1 select order;
    //  var queryModel = ParseQuery (queryable.Expression);

    //  var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //  Assert.That (
    //      query.Statement,
    //      Is.EqualTo (
    //          "SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[OrderNo],[t0].[DeliveryDate],[t0].[OfficialID],[t0].[CustomerID],[t0].[CustomerIDClassID] "
    //          + "FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] = @1)"));
    //  Assert.That (query.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
    //  Assert.That (
    //      query.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (Order)).StorageEntityDefinition.StorageProviderDefinition));
    //  Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
    //  Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
    //}

    //[Test]
    //public void CreateQuery_EagerFetchQueries ()
    //{
    //  var relationMember = typeof (Order).GetProperty ("OrderItems");
    //  var fetchRequest = new FetchManyRequest (relationMember);
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, _order1QueryModel, 0);

    //  var query = _orderExecutor.CreateQuery ("<dynamic query>", _order1QueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

    //  var orderItemsRelationEndPointDefinition =
    //      DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

    //  Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
    //  var fetchQuery = query.EagerFetchQueries.Single ();
    //  Assert.That (fetchQuery.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
    //  Assert.That (
    //      fetchQuery.Value.Statement,
    //      Is.EqualTo (
    //          "SELECT DISTINCT [t3].[ID],[t3].[ClassID],[t3].[Timestamp],[t3].[Position],[t3].[Product],[t3].[OrderID] "
    //          +
    //          "FROM (SELECT [t2].[ID],[t2].[ClassID],[t2].[Timestamp],[t2].[OrderNo],[t2].[DeliveryDate],[t2].[OfficialID],[t2].[CustomerID],[t2].[CustomerIDClassID] "
    //          + "FROM [OrderView] AS [t2] WHERE ([t2].[OrderNo] = @1)) AS [q1] "
    //          + "CROSS JOIN [OrderItemView] AS [t3] WHERE ([q1].[ID] = [t3].[OrderID])"));
    //  Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo (1));
    //  Assert.That (
    //      fetchQuery.Value.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem)).StorageEntityDefinition.StorageProviderDefinition));
    //  Assert.That (fetchQuery.Value.QueryType, Is.EqualTo (QueryType.Collection));
    //}

    //[Test]
    //public void CreateQuery_EagerFetchQueries_AfterOtherResultOperators ()
    //{
    //  var queryable = (from order in QueryFactory.CreateLinqQuery<Order> () where order.OrderNumber == 1 select order).Take (1);
    //  var queryModel = ParseQuery (queryable.Expression);
    //  var relationMember = typeof (Order).GetProperty ("OrderItems");
    //  var fetchRequest = new FetchManyRequest (relationMember);
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 1);

    //  var result = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    //  Assert.That (result.EagerFetchQueries.Count, Is.EqualTo (1));
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The property "
    //                                                                      +
    //                                                                      "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.NotInMappingRelatedObjects' is not a relation end point. Fetching it is not "
    //                                                                      + "supported by this LINQ provider.")]
    //public void CreateQuery_EagerFetchQueries_ForNonRelationProperty ()
    //{
    //  var relationMember = typeof (Order).GetProperty ("NotInMappingRelatedObjects");
    //  var fetchRequest = new FetchManyRequest (relationMember);
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, _order1QueryModel, 0);

    //  _orderExecutor.CreateQuery ("<dynamic query>", _order1QueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The member 'OnLoadedLoadMode' is a 'Field', which cannot be fetched by "
    //                                                                      + "this LINQ provider. Only properties can be fetched.")]
    //public void CreateQuery_EagerFetchQueries_ForField ()
    //{
    //  var relationMember = typeof (Order).GetField ("OnLoadedLoadMode");
    //  var fetchRequest = new FetchOneRequest (relationMember);
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, _order1QueryModel, 0);

    //  _orderExecutor.CreateQuery ("<dynamic query>", _order1QueryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    //}

    //[Test]
    //public void CreateQuery_EagerFetchQueries_WithSortExpression ()
    //{
    //  var queryable = from c in QueryFactory.CreateLinqQuery<Customer> () where c.Name == "Kunde 1" select c;
    //  var queryModel = ParseQuery (queryable.Expression);
    //  var relationMember = typeof (Customer).GetProperty ("Orders");
    //  var fetchRequest = new FetchManyRequest (relationMember);
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

    //  var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

    //  var ordersRelationEndPointDefinition =
    //      DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

    //  Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
    //  var fetchQuery = query.EagerFetchQueries.Single ();
    //  Assert.That (fetchQuery.Key, Is.SameAs (ordersRelationEndPointDefinition));
    //  Assert.That (
    //      fetchQuery.Value.Statement,
    //      Is.EqualTo (
    //          "SELECT [q2].[ID],[q2].[ClassID],[q2].[Timestamp],[q2].[OrderNo],[q2].[DeliveryDate],[q2].[OfficialID],[q2].[CustomerID],[q2].[CustomerIDClassID] "
    //          + "FROM ("
    //            + "SELECT DISTINCT [t4].[ID],[t4].[ClassID],[t4].[Timestamp],[t4].[OrderNo],[t4].[DeliveryDate],[t4].[OfficialID],[t4].[CustomerID],[t4].[CustomerIDClassID] "
    //            + "FROM ("
    //              + "SELECT [t3].[ID],[t3].[ClassID],[t3].[Timestamp],[t3].[Name],[t3].[IndustrialSectorID],[t3].[CustomerSince],[t3].[CustomerType] "
    //              + "FROM [CustomerView] AS [t3] WHERE ([t3].[Name] = @1)"
    //            + ") AS [q1] "
    //            + "CROSS JOIN [OrderView] AS [t4] WHERE ([q1].[ID] = [t4].[CustomerID])"
    //          + ") AS [q2] "
    //          + "ORDER BY [q2].[OrderNo] ASC"));

    //  Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
    //  Assert.That (
    //      fetchQuery.Value.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem)).StorageEntityDefinition.StorageProviderDefinition));
    //}

    //[Test]
    //public void CreateQuery_EagerFetchQueries_Recursive ()
    //{
    //  var queryable = from c in QueryFactory.CreateLinqQuery<Customer> () where c.Name == "Kunde 1" select c;
    //  var queryModel = ParseQuery (queryable.Expression);

    //  var fetchRequest = new FetchManyRequest (typeof (Customer).GetProperty ("Orders"));
    //  fetchRequest.GetOrAddInnerFetchRequest (new FetchManyRequest (typeof (Order).GetProperty ("OrderItems")));
    //  var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

    //  var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

    //  var ordersRelationEndPointDefinition =
    //      DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");
    //  var orderItemsRelationEndPointDefinition =
    //      DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

    //  Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
    //  var fetchQuery1 = query.EagerFetchQueries.Single ();
    //  Assert.That (fetchQuery1.Key, Is.SameAs (ordersRelationEndPointDefinition));

    //  Assert.That (
    //      fetchQuery1.Value.Statement,
    //      Is.EqualTo (
    //          "SELECT [q2].[ID],[q2].[ClassID],[q2].[Timestamp],[q2].[OrderNo],[q2].[DeliveryDate],[q2].[OfficialID],[q2].[CustomerID],[q2].[CustomerIDClassID] "
    //          + "FROM ("
    //            + "SELECT DISTINCT [t4].[ID],[t4].[ClassID],[t4].[Timestamp],[t4].[OrderNo],[t4].[DeliveryDate],[t4].[OfficialID],[t4].[CustomerID],[t4].[CustomerIDClassID] "
    //            + "FROM ("
    //              + "SELECT [t3].[ID],[t3].[ClassID],[t3].[Timestamp],[t3].[Name],[t3].[IndustrialSectorID],[t3].[CustomerSince],[t3].[CustomerType] "
    //              + "FROM [CustomerView] AS [t3] WHERE ([t3].[Name] = @1)"
    //            + ") AS [q1] "
    //            + "CROSS JOIN [OrderView] AS [t4] "
    //            + "WHERE ([q1].[ID] = [t4].[CustomerID])"
    //          + ") AS [q2] "
    //          + "ORDER BY [q2].[OrderNo] ASC"));
    //  Assert.That (fetchQuery1.Value.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (fetchQuery1.Value.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (fetchQuery1.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
    //  Assert.That (
    //      fetchQuery1.Value.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (Customer)).StorageEntityDefinition.StorageProviderDefinition));
    //  Assert.That (fetchQuery1.Value.EagerFetchQueries.Count, Is.EqualTo (1));

    //  var fetchQuery2 = fetchQuery1.Value.EagerFetchQueries.Single ();
    //  Assert.That (fetchQuery2.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
    //  Assert.That (
    //      fetchQuery2.Value.Statement,
    //      Is.EqualTo (
    //          "SELECT DISTINCT [t9].[ID],[t9].[ClassID],[t9].[Timestamp],[t9].[Position],[t9].[Product],[t9].[OrderID] "
    //          +
    //          "FROM (SELECT [t8].[ID],[t8].[ClassID],[t8].[Timestamp],[t8].[OrderNo],[t8].[DeliveryDate],[t8].[OfficialID],[t8].[CustomerID],[t8].[CustomerIDClassID] "
    //          +
    //          "FROM (SELECT [t7].[ID],[t7].[ClassID],[t7].[Timestamp],[t7].[Name],[t7].[IndustrialSectorID],[t7].[CustomerSince],[t7].[CustomerType] "
    //          + "FROM [CustomerView] AS [t7] WHERE ([t7].[Name] = @1)) AS [q5] CROSS JOIN [OrderView] AS [t8] "
    //          + "WHERE ([q5].[ID] = [t8].[CustomerID])) AS [q6] CROSS JOIN [OrderItemView] AS [t9] WHERE ([q6].[ID] = [t9].[OrderID])"));
    //  Assert.That (fetchQuery2.Value.Parameters.Count, Is.EqualTo (1));
    //  Assert.That (fetchQuery2.Value.Parameters[0].Name, Is.EqualTo ("@1"));
    //  Assert.That (fetchQuery2.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
    //  Assert.That (
    //      fetchQuery2.Value.StorageProviderDefinition,
    //      Is.SameAs (MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem)).StorageEntityDefinition.StorageProviderDefinition));
    //}

    //[Test]
    //public void CreateQuery_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryGeneratorMixin> ().EnterScope ())
    //  {
    //    var executor = ObjectFactory.Create<DomainObjectQueryExecutor> (
    //        ParamList.Create (
    //            _orderClassDefinition,
    //            _preparationStage,
    //            _resolutionStage,
    //            _generationStage,
    //            _sqlStorageTypeInformationProvider,
    //            _typeConversionProvider));

    //    executor.CreateQuery ("<dynamic query>", _order1QueryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryGeneratorMixin> (executor).CreateQueryCalled, Is.True);
    //  }
    //}

    //[Test]
    //public void CreateQueryFromModel_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryGeneratorMixin> ().EnterScope ())
    //  {
    //    var executor = ObjectFactory.Create<DomainObjectQueryExecutor> (
    //        ParamList.Create (
    //            _orderClassDefinition,
    //            _preparationStage,
    //            _resolutionStage,
    //            _generationStage,
    //            _sqlStorageTypeInformationProvider,
    //            _typeConversionProvider));

    //    executor.CreateQuery ("<dynamic query>", _order1QueryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryGeneratorMixin> (executor).CreateQueryFromModelCalled, Is.True);
    //  }
    //}

    //[Test]
    //public void CreateQueryFromModelWithClassDefinition_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryGeneratorMixin> ().EnterScope ())
    //  {
    //    var executor = ObjectFactory.Create<DomainObjectQueryExecutor> (
    //        ParamList.Create (
    //            _orderClassDefinition,
    //            _preparationStage,
    //            _resolutionStage,
    //            _generationStage,
    //            _sqlStorageTypeInformationProvider,
    //            _typeConversionProvider));

    //    executor.CreateQuery ("<dynamic query>", _order1QueryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryGeneratorMixin> (executor).CreateQueryFromModelWithClassDefinitionCalled, Is.True);
    //  }
    //}

    //[Test]
    //public void CreateQuery_UnaryExpression_ThrowsNoException ()
    //{
    //  var query = (from c in QueryFactory.CreateLinqQuery<Customer> () select c).Cast<Company> ();
    //  var queryModel = ParseQuery (query.Expression);
    //  var executor =
    //      ObjectFactory.Create<DomainObjectQueryExecutor> (
    //          ParamList.Create (
    //              _orderClassDefinition,
    //              _preparationStage,
    //              _resolutionStage,
    //              _generationStage,
    //              _sqlStorageTypeInformationProvider,
    //              _typeConversionProvider));

    //  executor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //}

    //[Test]
    //public void CreateSqlCommand ()
    //{
    //  var result = _orderExecutor.CreateSqlCommand (_order1QueryModel, false);

    //  Assert.That (
    //      result.CommandText,
    //      Is.EqualTo (
    //          "SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[OrderNo],[t0].[DeliveryDate],[t0].[OfficialID],[t0].[CustomerID],[t0].[CustomerIDClassID] "
    //          + "FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] = @1)"));
    //  Assert.That (result.Parameters, Is.EqualTo (new[] { new CommandParameter ("@1", 1) }));
    //}

    //[Test]
    //public void CreateSqlCommand_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryGeneratorMixin> ().EnterScope ())
    //  {
    //    var executor = ObjectFactory.Create<DomainObjectQueryExecutor> (
    //        ParamList.Create (
    //            _orderClassDefinition,
    //            _preparationStage,
    //            _resolutionStage,
    //            _generationStage,
    //            _sqlStorageTypeInformationProvider,
    //            _typeConversionProvider));

    //    executor.CreateSqlCommand (_order1QueryModel, false);
    //    Assert.That (Mixin.Get<TestQueryGeneratorMixin> (executor).CreateSqlCommandCalled, Is.True);
    //  }
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage =
    //    "There was an error preparing or resolving query 'from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o]' for "
    //    + "SQL generation. Unsupported feature. Expression: 'Foo'")]
    //public void CreateSqlCommand_NotSupportedException_InPreparationStage ()
    //{
    //  var preparationStageStub = MockRepository.GenerateStub<ISqlPreparationStage> ();
    //  preparationStageStub
    //      .Stub (stub => stub.PrepareSqlStatement (_order1QueryModel, null))
    //      .Throw (new NotSupportedException ("Unsupported feature. Expression: 'Foo'"));

    //  var executor = new DomainObjectQueryExecutor (
    //      _orderClassDefinition, preparationStageStub, _resolutionStage, _generationStage, _sqlStorageTypeInformationProvider, _typeConversionProvider);

    //  executor.CreateSqlCommand (_order1QueryModel, false);
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage =
    //    "There was an error preparing or resolving query 'from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o]' for "
    //    + "SQL generation. Unsupported feature. Expression: 'Foo'")]
    //public void CreateSqlCommand_NotSupportedException_InResolutionStage ()
    //{
    //  var resolutionStageStub = MockRepository.GenerateStub<IMappingResolutionStage> ();
    //  resolutionStageStub
    //      .Stub (stub => stub.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<IMappingResolutionContext>.Is.Anything))
    //      .Throw (new NotSupportedException ("Unsupported feature. Expression: 'Foo'"));

    //  var executor = new DomainObjectQueryExecutor (
    //      _orderClassDefinition, _preparationStage, resolutionStageStub, _generationStage, _sqlStorageTypeInformationProvider, _typeConversionProvider);

    //  executor.CreateSqlCommand (_order1QueryModel, false);
    //}

    //[Test]
    //[ExpectedException (typeof (UnmappedItemException), ExpectedMessage =
    //    "Query 'from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o]' contains an unmapped item. Unsupported item.")]
    //public void CreateSqlCommand_UnmappedItemException_InResolutionStage ()
    //{
    //  var resolutionStageStub = MockRepository.GenerateStub<IMappingResolutionStage> ();
    //  resolutionStageStub
    //      .Stub (stub => stub.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<IMappingResolutionContext>.Is.Anything))
    //      .Throw (new UnmappedItemException ("Unsupported item."));

    //  var executor = new DomainObjectQueryExecutor (
    //      _orderClassDefinition, _preparationStage, resolutionStageStub, _generationStage, _sqlStorageTypeInformationProvider, _typeConversionProvider);

    //  executor.CreateSqlCommand (_order1QueryModel, false);
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage =
    //    "There was an error generating SQL for the query "
    //    + "'from Order order in DomainObjectQueryable<Order> where ([order].OrderNumber = 1) select [order]'. "
    //    + "Unsupported item. Exception: 'Foo'")]
    //public void CreateSqlCommand_NotSupportedException_InSqlGenerationStage ()
    //{
    //  var expression = (from order in QueryFactory.CreateLinqQuery<Order> () where order.OrderNumber == 1 select order).Expression;
    //  var queryModel = ParseQuery (expression);

    //  var generationStageStub = MockRepository.GenerateStub<ISqlGenerationStage> ();
    //  generationStageStub
    //      .Stub (stub => stub.GenerateTextForOuterSqlStatement (Arg<SqlCommandBuilder>.Is.Anything, Arg<SqlStatement>.Is.Anything))
    //      .Throw (new NotSupportedException ("Unsupported item. Exception: 'Foo'"));

    //  var executor = new DomainObjectQueryExecutor (
    //      _orderClassDefinition, _preparationStage, _resolutionStage, generationStageStub, _sqlStorageTypeInformationProvider, _typeConversionProvider);
    //  executor.CreateSqlCommand (queryModel, false);
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage =
    //    "This query provider does not support the given query ('from Order o in DomainObjectQueryable<Order> select [o].ID'). re-store only supports "
    //    + "queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.")]
    //public void CreateSqlCommand_ColumnInSelectProjection_ExceptionWithFlagTrue ()
    //{
    //  var expression = (from o in QueryFactory.CreateLinqQuery<Order> () select o.ID).Expression;
    //  QueryModel queryModel = ParseQuery (expression);

    //  _orderExecutor.CreateSqlCommand (queryModel, true);
    //}

    //[Test]
    //public void CreateSqlCommand_ColumnInSelectProjection_NoExceptionWithFlagFalse ()
    //{
    //  var expression = (from o in QueryFactory.CreateLinqQuery<Order> () select o.ID).Expression;
    //  QueryModel queryModel = ParseQuery (expression);

    //  var result = _orderExecutor.CreateSqlCommand (queryModel, false);

    //  Assert.That (result, Is.Not.Null);
    //}

    //[Test]
    //[ExpectedException (typeof (NotSupportedException), ExpectedMessage =
    //    "This query provider does not support the given query ('DomainObjectQueryable<Order> => GroupBy([o].OrderNumber, [o])'). re-store only "
    //    + "supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects. GroupBy must be executed in memory, "
    //    + "for example by issuing AsEnumerable() before performing the grouping operation.")]
    //public void CreateSqlCommand_GroupByAtTopLevel_ExceptionWithFlagTrue ()
    //{
    //  var expression = ExpressionHelper.MakeExpression (() => (from o in QueryFactory.CreateLinqQuery<Order> () select o).GroupBy (o => o.OrderNumber));
    //  QueryModel queryModel = ParseQuery (expression);

    //  _orderExecutor.CreateSqlCommand (queryModel, true);
    //}

    private static QueryModel ParseQuery (Expression queryExpression)
    {
      var parser = QueryParser.CreateDefault ();
      return parser.GetParsedQuery (queryExpression);
    }
  }
}