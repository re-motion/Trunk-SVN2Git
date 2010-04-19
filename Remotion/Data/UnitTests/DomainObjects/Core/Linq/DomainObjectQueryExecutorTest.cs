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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : ClientTransactionBaseTest
  {
    private ClassDefinition _computerClassDefinition;
    private ClassDefinition _orderClassDefinition;
    private ClassDefinition _customerClassDefinition;
    private ClassDefinition _companyClassDefinition;
    private DomainObjectQueryExecutor _computerExecutor;
    private DomainObjectQueryExecutor _orderExecutor;
    private DomainObjectQueryExecutor _customerExecutor;
    private DomainObjectQueryExecutor _companyExecutor;
    private DefaultSqlPreparationStage _preparationStage;
    private DefaultMappingResolutionStage _resolutionStage;
    private DefaultSqlGenerationStage _generationStage;
    private SqlPreparationContext _context;

    public override void SetUp ()
    {
      base.SetUp();
      _computerClassDefinition = DomainObjectIDs.Computer1.ClassDefinition;
      _orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _customerClassDefinition = DomainObjectIDs.Customer1.ClassDefinition;
      _companyClassDefinition = DomainObjectIDs.Company1.ClassDefinition;

      var resolver = new MappingResolver();
      var generator = new UniqueIdentifierGenerator();
      _context = new SqlPreparationContext();
      _preparationStage = new DefaultSqlPreparationStage (MethodCallTransformerRegistry.CreateDefault(), _context, generator);
      _resolutionStage = new DefaultMappingResolutionStage (resolver, generator);
      _generationStage = new DefaultSqlGenerationStage();

      _computerExecutor = new DomainObjectQueryExecutor (_computerClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      _orderExecutor = new DomainObjectQueryExecutor (_orderClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      _customerExecutor = new DomainObjectQueryExecutor (_customerClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      _companyExecutor = new DomainObjectQueryExecutor (_companyClassDefinition, _preparationStage, _resolutionStage, _generationStage);
    }

    [Test]
    public void ExecuteScalar ()
    {
      var expression = ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer>() select computer).Count());
      QueryModel model = ParseQuery (expression);

      var count = _computerExecutor.ExecuteScalar<int> (model);
      Assert.That (count, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteScalar_NoCurrentTransaction ()
    {
      var expression = ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer>() select computer).Count());
      QueryModel model = ParseQuery (expression);

      using (ClientTransactionScope.EnterNullScope())
      {
        _computerExecutor.ExecuteScalar<int> (model);
      }
    }

    [Test]
    public void ExecuteScalar_WithFetches ()
    {
      var expression = ExpressionHelper.MakeExpression (
          () =>
          (from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order).Count());
      QueryModel queryModel = ParseQuery (expression);

      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      queryModel.ResultOperators.Insert (0, fetchRequest);

      var mockQuery = QueryFactory.CreateScalarQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderID,
          "SELECT COUNT(*) FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection());

      var executorMock = new MockRepository().PartialMock<DomainObjectQueryExecutor> (
          _orderClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      executorMock
          .Expect (
          mock => mock.CreateQuery (
                      Arg<string>.Is.Anything,
                      Arg.Is (queryModel),
                      Arg<IEnumerable<FetchQueryModelBuilder>>.Matches (rs => rs.Count() == 1 && rs.Single().FetchRequest == fetchRequest),
                      Arg.Is (QueryType.Scalar)))
          .Return (mockQuery);

      executorMock.Replay();

      var result = executorMock.ExecuteScalar<int> (queryModel);
      Assert.That (result, Is.EqualTo (1));

      executorMock.VerifyAllExpectations();
    }

    [Test]
    public void ExecuteSingle ()
    {
      var expression =
          ExpressionHelper.MakeExpression (
              () => (from computer in QueryFactory.CreateLinqQuery<Computer>() orderby computer.ID select computer).First());
      QueryModel model = ParseQuery (expression);

      var result = _computerExecutor.ExecuteSingle<Computer> (model, true);
      Assert.That (result, Is.SameAs (Computer.GetObject (DomainObjectIDs.Computer5)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteSingle_NoCurrentTransaction ()
    {
      var expression =
          ExpressionHelper.MakeExpression (
              () => (from computer in QueryFactory.CreateLinqQuery<Computer>() orderby computer.ID select computer).First());
      QueryModel model = ParseQuery (expression);

      using (ClientTransactionScope.EnterNullScope())
      {
        _computerExecutor.ExecuteSingle<int> (model, true);
      }
    }

    [Test]
    public void ExecuteSingle_DefaultIfEmpty_True ()
    {
      var expression =
          ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer>() where false select computer).First());
      QueryModel model = ParseQuery (expression);

      var result = _computerExecutor.ExecuteSingle<Computer> (model, true);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteSingle_DefaultIfEmpty_False ()
    {
      var expression =
          ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer>() where false select computer).First());
      QueryModel model = ParseQuery (expression);

      _computerExecutor.ExecuteSingle<Computer> (model, false);
    }

    [Test]
    public void ExecuteCollection ()
    {
      var query = from computer in QueryFactory.CreateLinqQuery<Computer>() select computer;
      QueryModel model = ParseQuery (query.Expression);

      IEnumerable<Computer> computers = _computerExecutor.ExecuteCollection<Computer> (model);

      var computerList = new ArrayList();
      foreach (Computer computer in computers)
        computerList.Add (computer);

      var expected = new[]
                     {
                         Computer.GetObject (DomainObjectIDs.Computer1),
                         Computer.GetObject (DomainObjectIDs.Computer2),
                         Computer.GetObject (DomainObjectIDs.Computer3),
                         Computer.GetObject (DomainObjectIDs.Computer4),
                         Computer.GetObject (DomainObjectIDs.Computer5),
                     };
      Assert.That (computerList.ToArray(), Is.EquivalentTo (expected));
    }

    [Test]
    public void ExecuteCollection_WithFetches ()
    {
      var query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      QueryModel queryModel = ParseQuery (query.Expression);

      var fetchRequest = new FetchManyRequest (typeof (Order).GetProperty ("OrderItems"));
      queryModel.ResultOperators.Add (fetchRequest);

      var mockQuery = QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderID,
          "SELECT [o].* FROM [Order] [o] WHERE [o].[OrderNo] = 1",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var executorMock = new MockRepository().PartialMock<DomainObjectQueryExecutor> (
          _orderClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      executorMock
          .Expect (
          mock => mock.CreateQuery (
                      Arg<string>.Is.Anything,
                      Arg.Is (queryModel),
                      Arg<IEnumerable<FetchQueryModelBuilder>>.Matches (rs => rs.Count() == 1 && rs.Single().FetchRequest == fetchRequest),
                      Arg.Is (QueryType.Collection)))
          .Return (mockQuery);

      executorMock.Replay();

      var orders = executorMock.ExecuteCollection<Order> (queryModel).ToArray();
      Assert.That (orders, Is.EqualTo (new[] { Order.GetObject (DomainObjectIDs.Order1) }));

      executorMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteCollection_NoCurrentTransaction ()
    {
      var query = from computer in QueryFactory.CreateLinqQuery<Computer>() select computer;
      QueryModel model = ParseQuery (query.Expression);

      using (ClientTransactionScope.EnterNullScope())
      {
        _computerExecutor.ExecuteCollection<Computer> (model);
      }
    }

    [Test]
    public void ExecuteCollection_ExecutesGroupByInMemory ()
    {
      var query = from computer in QueryFactory.CreateLinqQuery<Computer>() group computer by computer;
      QueryModel model = ParseQuery (query.Expression);

      var computers = _computerExecutor.ExecuteCollection<IGrouping<Computer, Computer>> (model).ToArray();
      Assert.That (computers.Length, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Cannot execute a query with a GroupBy clause that specifies fetch requests "
                          + "because GroupBy is simulated in-memory.")]
    public void ExecuteCollection_WithGroupBy_WithFetchRequests ()
    {
      var query = from computer in QueryFactory.CreateLinqQuery<Computer>() group computer by computer;
      QueryModel queryModel = ParseQuery (query.Expression);

      var relationMember = typeof (IGrouping<Computer, Computer>).GetProperty ("Key");
      queryModel.ResultOperators.Add (new FetchOneRequest (relationMember));

      _computerExecutor.ExecuteCollection<IGrouping<Computer, Computer>> (queryModel);
    }

    [Test]
    public void ExecuteCollection_WithGroupBy_WithOtherOperators_Before ()
    {
      var query = (from company1 in QueryFactory.CreateLinqQuery<Company>()
                   from company2 in QueryFactory.CreateLinqQuery<Company>()
                   where company1.ID == DomainObjectIDs.Partner1
                   select company1).Distinct().Cast<Partner>().GroupBy (p => p, p => p.ContactPerson);
      QueryModel model = ParseQuery (query.Expression);

      var partners = _companyExecutor.ExecuteCollection<IGrouping<Partner, Person>> (model).ToArray();
      Assert.That (partners.Length, Is.EqualTo (1));
      Assert.That (partners.First().Key.ID, Is.EqualTo (DomainObjectIDs.Partner1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot execute a query with a GroupBy clause that contains other result "
                                                                          +
                                                                          "operators after the GroupResultOperator because GroupBy is simulated in-memory."
        )]
    public void ExecuteCollection_WithGroupBy_WithOtherOperators_After ()
    {
      var query = (from computer in QueryFactory.CreateLinqQuery<Computer>() group computer by computer).Distinct();
      QueryModel model = ParseQuery (query.Expression);

      _computerExecutor.ExecuteCollection<IGrouping<Computer, Computer>> (model);
    }

    [Test]
    public void ExecuteScalar_WithParameters ()
    {
      var expression =
          ExpressionHelper.MakeExpression (
              () => (from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order).Count());
      QueryModel model = ParseQuery (expression);

      var count = _orderExecutor.ExecuteScalar<int> (model);

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void ExecuteCollection_WithParameters ()
    {
      var query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      QueryModel queryModel = ParseQuery (query.Expression);

      IEnumerable<Order> orders = _orderExecutor.ExecuteCollection<Order> (queryModel);
      
      var expected = new[] { Order.GetObject (DomainObjectIDs.Order1), };
      Assert.That (orders.ToArray(), Is.EquivalentTo (expected));
    }

    [Test]
    public void CreateQuery_Scalar ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var commandParameters = new[] { new CommandParameter ("x", "y") };

      var query = _orderExecutor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, "x", commandParameters, QueryType.Scalar);
      Assert.That (query.Statement, Is.EqualTo ("x"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("x"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo ("y"));
      Assert.That (query.StorageProviderID, Is.EqualTo (classDefinition.StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
    }

    [Test]
    public void CreateQuery_Collection ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var commandParameters = new[] { new CommandParameter ("x", "y") };

      var query = _orderExecutor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, "x", commandParameters, QueryType.Collection);
      Assert.That (query.Statement, Is.EqualTo ("x"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("x"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo ("y"));
      Assert.That (query.StorageProviderID, Is.EqualTo (classDefinition.StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    public void CreateQuery_FromModel_Scalar ()
    {
      var expression =
          ExpressionHelper.MakeExpression (
              () => (from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order).Count());
      var queryModel = ParseQuery (expression);

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Scalar);
      Assert.That (query.Statement, Is.EqualTo ("SELECT COUNT(*) FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] = @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (
          query.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
    }

    [Test]
    public void CreateQuery_FromModel_Collection ()
    {
      var queryable = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      var queryModel = ParseQuery (queryable.Expression);

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
      Assert.That (
          query.Statement,
          Is.EqualTo (
              "SELECT [t0].* FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] = @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (
          query.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    public void CreateQuery_EagerFetchQueries ()
    {
      var queryable = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      var queryModel = ParseQuery (queryable.Expression);
      var relationMember = typeof (Order).GetProperty ("OrderItems");
      var fetchRequest = new FetchManyRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      var orderItemsRelationEndPointDefinition =
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery = query.EagerFetchQueries.Single();
      Assert.That (fetchQuery.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
      Assert.That (
          fetchQuery.Value.Statement,
          Is.EqualTo (
              "SELECT DISTINCT [t3].* "
              + "FROM (SELECT [t2].* "
              + "FROM [OrderView] AS [t2] WHERE ([t2].[OrderNo] = @1)) AS [q1] "
              + "CROSS JOIN [OrderItemView] AS [t3] WHERE ([q1].[ID] = [t3].[OrderID])"));
      Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (
          fetchQuery.Value.StorageProviderID,
          Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
      Assert.That (fetchQuery.Value.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This query provider does not support result operators occurring after "
                                                                              +
                                                                              "fetch requests. The objects on which the fetching is performed must be the same objects that are returned from the query. Rewrite the "
                                                                              +
                                                                              "query to perform the fetching after applying all other result operators or call AsEnumerable after the last fetch request in order to "
                                                                              + "execute all subsequent result operators in memory.")]
    public void CreateQuery_EagerFetchQueries_BeforeOtherResultOperators ()
    {
      var queryable = (from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order).Take (1);
      var queryModel = ParseQuery (queryable.Expression);
      var relationMember = typeof (Order).GetProperty ("OrderItems");
      var fetchRequest = new FetchManyRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    }

    [Test]
    public void CreateQuery_EagerFetchQueries_AfterOtherResultOperators ()
    {
      var queryable = (from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order).Take (1);
      var queryModel = ParseQuery (queryable.Expression);
      var relationMember = typeof (Order).GetProperty ("OrderItems");
      var fetchRequest = new FetchManyRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 1);

      var result = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
      Assert.That (result.EagerFetchQueries.Count, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The property "
                                                                          +
                                                                          "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.NotInMappingRelatedObjects' is not a relation end point. Fetching it is not "
                                                                          + "supported by this LINQ provider.")]
    public void CreateQuery_EagerFetchQueries_ForNonRelationProperty ()
    {
      var query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      var queryModel = ParseQuery (query.Expression);
      var relationMember = typeof (Order).GetProperty ("NotInMappingRelatedObjects");
      var fetchRequest = new FetchManyRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The member 'OnLoadedLoadMode' is a 'Field', which cannot be fetched by "
                                                                          + "this LINQ provider. Only properties can be fetched.")]
    public void CreateQuery_EagerFetchQueries_ForField ()
    {
      var query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      var queryModel = ParseQuery (query.Expression);
      var relationMember = typeof (Order).GetField ("OnLoadedLoadMode");
      var fetchRequest = new FetchOneRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);
    }

    [Test]
    public void CreateQuery_EagerFetchQueries_WithSortExpression ()
    {
      var queryable = from c in QueryFactory.CreateLinqQuery<Customer>() where c.Name == "Kunde 1" select c;
      var queryModel = ParseQuery (queryable.Expression);
      var relationMember = typeof (Customer).GetProperty ("Orders");
      var fetchRequest = new FetchManyRequest (relationMember);
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      var ordersRelationEndPointDefinition =
          DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery = query.EagerFetchQueries.Single();
      Assert.That (fetchQuery.Key, Is.SameAs (ordersRelationEndPointDefinition));
      Assert.That (
          fetchQuery.Value.Statement,
          Is.EqualTo (
              "SELECT DISTINCT [t3].* "
              + "FROM (SELECT [t2].* "
              + "FROM [CustomerView] AS [t2] WHERE ([t2].[Name] = @1)) AS [q1] CROSS JOIN [OrderView] AS [t3] WHERE ([q1].[ID] = [t3].[CustomerID]) "
              + "ORDER BY OrderNo asc"));

      Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (
          fetchQuery.Value.StorageProviderID,
          Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
    }

    [Test]
    public void CreateQuery_EagerFetchQueries_Recursive ()
    {
      var queryable = from c in QueryFactory.CreateLinqQuery<Customer>() where c.Name == "Kunde 1" select c;
      var queryModel = ParseQuery (queryable.Expression);

      var fetchRequest = new FetchManyRequest (typeof (Customer).GetProperty ("Orders"));
      fetchRequest.GetOrAddInnerFetchRequest (new FetchManyRequest (typeof (Order).GetProperty ("OrderItems")));
      var fetchQueryModelBuilder = new FetchQueryModelBuilder (fetchRequest, queryModel, 0);

      var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchQueryModelBuilder }, QueryType.Collection);

      var ordersRelationEndPointDefinition =
          DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");
      var orderItemsRelationEndPointDefinition =
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery1 = query.EagerFetchQueries.Single();
      Assert.That (fetchQuery1.Key, Is.SameAs (ordersRelationEndPointDefinition));
      Assert.That (
          fetchQuery1.Value.Statement,
          Is.EqualTo (
              "SELECT DISTINCT [t3].* "
              +
              "FROM (SELECT [t2].* "
              + "FROM [CustomerView] AS [t2] WHERE ([t2].[Name] = @1)) AS [q1] CROSS JOIN [OrderView] AS [t3] "
              + "WHERE ([q1].[ID] = [t3].[CustomerID]) ORDER BY OrderNo asc"));
      Assert.That (fetchQuery1.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery1.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery1.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (
          fetchQuery1.Value.StorageProviderID,
          Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Customer)).StorageProviderID));
      Assert.That (fetchQuery1.Value.EagerFetchQueries.Count, Is.EqualTo (1));

      var fetchQuery2 = fetchQuery1.Value.EagerFetchQueries.Single();
      Assert.That (fetchQuery2.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
      Assert.That (
          fetchQuery2.Value.Statement,
          Is.EqualTo (
              "SELECT DISTINCT [t8].* "
              + "FROM (SELECT [t7].* "
              + "FROM (SELECT [t6].* "
              + "FROM [CustomerView] AS [t6] WHERE ([t6].[Name] = @1)) AS [q4] CROSS JOIN [OrderView] AS [t7] "
              + "WHERE ([q4].[ID] = [t7].[CustomerID])) AS [q5] CROSS JOIN [OrderItemView] AS [t8] WHERE ([q5].[ID] = [t8].[OrderID])"));
      Assert.That (fetchQuery2.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery2.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery2.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (
          fetchQuery2.Value.StorageProviderID,
          Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void CreateQuery_ColumnInSelectProjection_ThrowsException ()
    {
      var expression = ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer>() select computer));
      QueryModel queryModel = ParseQuery (expression);

      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (int));
      var sqlTable = new SqlTable (unresolvedTableInfo);
      var sqlStatement = new SqlStatement (
          new SqlColumnExpression (typeof (Order), "o", "ID"), new[] { sqlTable }, new Ordering[] { }, null, null, false, false);

      var executorMock = new MockRepository().PartialMock<DomainObjectQueryExecutor> (
          _computerClassDefinition, _preparationStage, _resolutionStage, _generationStage);
      executorMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (executorMock, "TransformAndResolveQueryModel", queryModel))
          .Return (sqlStatement);
      executorMock.Replay();
      executorMock.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    }

    // TODO Review 2404: These tests need to be enabled before the task can be closed. Rename the existing mixin to Legacy..., 
    //add identical TestQueryExecutorMixin but change signature of CreateStatement override to return SqlCommandData

    //TODO: 2404 uncomment when LegacyDomainObjectQueryable is refactored
    //[Test]
    //public void CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
    //  {
    //    var queryable = new LegacyDomainObjectQueryable<Order> ();
    //    Assert.That (Mixin.Get<TestQueryExecutorMixin> (((DefaultQueryProvider) queryable.Provider).Executor), Is.Not.Null);
    //  }
    //}

    //TODO: 2404 uncomment when LegacyDomainObjectQueryable is refactored
    //[Test]
    //public void GetStatement_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (LegacyDomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
    //  {
    //    var queryable = new LegacyDomainObjectQueryable<Computer> (_sqlGenerator);
    //    var executor = queryable.GetExecutor ();

    //    var query = from computer in QueryFactory.CreateLinqQuery<Computer> () select computer;
    //    executor.CreateStatement (ParseQuery (query.Expression));
    //    Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).GetStatementCalled, Is.True);
    //  }
    //}

    //TODO: 2404 uncomment when LegacyDomainObjectQueryable is refactored
    //[Test]
    //public void CreateQuery_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (LegacyDomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
    //  {
    //    var queryable = new LegacyDomainObjectQueryable<Order> (_sqlGenerator);
    //    var executor = queryable.GetExecutor ();

    //    ClassDefinition classDefinition = executor.StartingClassDefinition;
    //    var query = from computer in QueryFactory.CreateLinqQuery<Computer> () select computer;
    //    CommandData statement = executor.CreateStatement (ParseQuery (query.Expression));

    //    executor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, statement.Statement, statement.Parameters, QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryCalled, Is.True);
    //  }
    //}

    //TODO 2404
    //[Test]
    //public void CreateQueryFromModel_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
    //  {
    //    var query = from computer in QueryFactory.CreateLinqQuery<Computer> () select computer;
    //    var queryModel = ParseQuery (query.Expression);
    //    var executor = ObjectFactory.Create<LegacyDomainObjectQueryExecutor> (ParamList.Create (_sqlGenerator, _orderClassDefinition));

    //    executor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelCalled, Is.True);
    //  }
    //}

    //TODO 2404
    //[Test]
    //public void CreateQueryFromModelWithClassDefinition_CanBeMixed ()
    //{
    //  using (MixinConfiguration.BuildNew ().ForClass (typeof (LegacyDomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
    //  {
    //    var query = from computer in QueryFactory.CreateLinqQuery<Computer> () select computer;
    //    var queryModel = ParseQuery (query.Expression);
    //    var executor = ObjectFactory.Create<LegacyDomainObjectQueryExecutor> (ParamList.Create (_sqlGenerator, _orderClassDefinition));

    //    executor.CreateQuery ("<dynamic query>", queryModel, new FetchQueryModelBuilder[0], QueryType.Collection);
    //    Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelWithClassDefinitionCalled, Is.True);
    //  }
    //}

    private static QueryModel ParseQuery (Expression queryExpression)
    {
      var parser = new QueryParser();
      return parser.GetParsedQuery (queryExpression);
    }
  }
}