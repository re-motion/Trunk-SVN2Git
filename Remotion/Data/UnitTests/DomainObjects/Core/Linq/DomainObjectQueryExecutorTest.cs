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
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.Linq;
using Remotion.Mixins;
using Remotion.Reflection;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : ClientTransactionBaseTest
  {
    private SqlServerGenerator _sqlGenerator;
    private ClassDefinition _computerClassDefinition;
    private ClassDefinition _orderClassDefinition;
    private ClassDefinition _customerClassDefinition;
    private DomainObjectQueryExecutor _computerExecutor;
    private DomainObjectQueryExecutor _orderExecutor;
    private DomainObjectQueryExecutor _customerExecutor;

    public override void SetUp ()
    {
      base.SetUp ();
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
      _computerClassDefinition = DomainObjectIDs.Computer1.ClassDefinition;
      _orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _customerClassDefinition = DomainObjectIDs.Customer1.ClassDefinition;
      _computerExecutor = new DomainObjectQueryExecutor (_sqlGenerator, _computerClassDefinition);
      _orderExecutor = new DomainObjectQueryExecutor (_sqlGenerator, _orderClassDefinition);
      _customerExecutor = new DomainObjectQueryExecutor (_sqlGenerator, _customerClassDefinition);
    }
    
    [Test]
    public void ExecuteScalar()
    {
      QueryModel model = GetParsedCountQuery ();

      var count = _computerExecutor.ExecuteScalar<int> (model, new FetchManyRequest[0]);
      Assert.That (count, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteScalar_NoCurrentTransaction ()
    {
      QueryModel model = GetParsedCountQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        _computerExecutor.ExecuteScalar<int> (model, new FetchManyRequest[0]);
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      QueryModel model = GetParsedSimpleQuery();

      IEnumerable<Computer> computers = _computerExecutor.ExecuteCollection<Computer> (model, new FetchManyRequest[0]);

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
      Assert.That (computerList, Is.EquivalentTo (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteCollection_NoCurrentTransaction ()
    {
      QueryModel model = GetParsedSimpleQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        _computerExecutor.ExecuteCollection<Computer> (model, new FetchManyRequest[0]);
      }
    }

    [Test]
    public void ExecuteScalar_WithParameters ()
    {
      QueryModel model = GetParsedSimpleWhereCountQuery ();

      var count = _orderExecutor.ExecuteScalar<int> (model, new FetchManyRequest[0]);

      Assert.That (count, Is.EqualTo (1));
    }

    [Test]
    public void ExecuteCollection_WithParameters()
    {
      QueryModel model = GetParsedSimpleWhereQuery ();

      IEnumerable<Order> orders = _orderExecutor.ExecuteCollection<Order> (model, new FetchManyRequest[0]);

      var orderList = new ArrayList ();
      foreach (Order order in orders) 
        orderList.Add (order);

      var expected = new[]
                         {
                             Order.GetObject (DomainObjectIDs.Order1),

                         };
      Assert.That (orderList, Is.EquivalentTo (expected));
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
    public void CreateQuery_Collection()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var commandParameters = new [] { new CommandParameter("x", "y") };

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
      var queryModel = GetParsedSimpleWhereCountQuery();

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0], QueryType.Scalar);
      Assert.That (query.Statement, Is.EqualTo ("SELECT COUNT (*) FROM [OrderView] [order] WHERE ([order].[OrderNo] = @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (query.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Scalar));
    }

    [Test]
    public void CreateQuery_FromModel_Collection ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0], QueryType.Collection);
      Assert.That (query.Statement, Is.EqualTo ("SELECT [order].* FROM [OrderView] [order] WHERE ([order].[OrderNo] = @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (query.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
      Assert.That (query.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    public void CreateQuery_EagerFetchQueries ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var fetchRequest = new FetchManyRequest ((Expression<Func<Order,IEnumerable<OrderItem>>>)  (o => o.OrderItems));

      var query = _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest }, QueryType.Collection);

      var orderItemsRelationEndPointDefinition = 
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery = query.EagerFetchQueries.Single ();
      Assert.That (fetchQuery.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
      Assert.That (fetchQuery.Value.Statement, Is.EqualTo (
          "SELECT [#fetch0].* FROM [OrderView] [order], [OrderItemView] [#fetch0] "
          + "WHERE (([order].[OrderNo] = @1) AND "
          +        "(([order].[ID] IS NULL AND [#fetch0].[OrderID] IS NULL) OR [order].[ID] = [#fetch0].[OrderID]))"));
      Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (fetchQuery.Value.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
      Assert.That (fetchQuery.Value.QueryType, Is.EqualTo (QueryType.Collection));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.NotInMappingRelatedObjects' is not a relation end point. Fetching it is not "
        + "supported by this LINQ provider.")]
    public void CreateQuery_EagerFetchQueries_ForNonRelationProperty ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var fetchRequest = new FetchManyRequest ((Expression<Func<Order, IEnumerable<OrderItem>>>) (o => o.NotInMappingRelatedObjects));

      _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest }, QueryType.Collection);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The member 'LastLoadMode' is a 'Field', which cannot be fetched by "
        + "this LINQ provider. Only properties can be fetched.")]
    public void CreateQuery_EagerFetchQueries_ForField ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var fetchRequest = new FetchOneRequest ((Expression<Func<Order, LoadMode>>) (o => o.LastLoadMode));

      _orderExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest }, QueryType.Collection);
    }

    [Test]
    public void CreateQuery_EagerFetchQueries_WithSortExpression ()
    {
      var queryModel = GetParsedSimpleCustomerQuery();
      var fetchRequest = new FetchManyRequest ((Expression<Func<Customer, IEnumerable<Order>>>) (c => c.Orders));

      var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest }, QueryType.Collection);

      var ordersRelationEndPointDefinition =
          DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery = query.EagerFetchQueries.Single ();
      Assert.That (fetchQuery.Key, Is.SameAs (ordersRelationEndPointDefinition));
      Assert.That (fetchQuery.Value.Statement, Is.EqualTo (
          "SELECT * FROM (SELECT [#fetch0].* FROM [CustomerView] [c], [OrderView] [#fetch0] "
          + "WHERE (([c].[Name] = @1) AND "
          + "(([c].[ID] IS NULL AND [#fetch0].[CustomerID] IS NULL) OR [c].[ID] = [#fetch0].[CustomerID]))) [result] ORDER BY OrderNo asc"));
      Assert.That (fetchQuery.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (fetchQuery.Value.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
    }

    [Test]
    [Ignore ("TODO 1178")]
    public void CreateQuery_EagerFetchQueries_Recursive ()
    {
      var queryModel = GetParsedSimpleCustomerQuery ();
      var fetchRequest = new FetchManyRequest ((Expression<Func<Customer, IEnumerable<Order>>>) (c => c.Orders));
      LambdaExpression relatedObjectSelector = (Expression<Func<Order, IEnumerable<OrderItem>>>)(o => o.OrderItems);
      fetchRequest.GetOrAddInnerFetchRequest (new FetchManyRequest (relatedObjectSelector));

      var query = _customerExecutor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest }, QueryType.Collection);

      var ordersRelationEndPointDefinition =
          DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");
      var orderItemsRelationEndPointDefinition =
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      Assert.That (query.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery1 = query.EagerFetchQueries.Single ();
      Assert.That (fetchQuery1.Key, Is.SameAs (ordersRelationEndPointDefinition));
      Assert.That (fetchQuery1.Value.Statement, Is.EqualTo (
          "SELECT * FROM (SELECT [#fetch0].* FROM [CustomerView] [c], [OrderView] [#fetch0] "
          + "WHERE (([c].[Name] = @1) AND "
          +         "(([c].[ID] IS NULL AND [#fetch0].[CustomerID] IS NULL) OR [c].[ID] = [#fetch0].[CustomerID]))) [result] ORDER BY OrderNo asc"));
      Assert.That (fetchQuery1.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery1.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery1.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (fetchQuery1.Value.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Customer)).StorageProviderID));

      Assert.That (fetchQuery1.Value.EagerFetchQueries.Count, Is.EqualTo (1));
      var fetchQuery2 = fetchQuery1.Value.EagerFetchQueries.Single ();
      Assert.That (fetchQuery2.Key, Is.SameAs (orderItemsRelationEndPointDefinition));
      Assert.That (fetchQuery2.Value.Statement, Is.EqualTo (
          "SELECT [#fetch1].* FROM [CustomerView] [c], [OrderView] [#fetch0], [OrderItemView] [#fetch1] "
          + "WHERE ((([c].[Name] = @1) AND "
          +       "(([c].[ID] IS NULL AND [#fetch0].[CustomerID] IS NULL) OR [c].[ID] = [#fetch0].[CustomerID])) AND "
          +       "(([#fetch0].[ID] IS NULL AND [#fetch1].[OrderID] IS NULL) OR [#fetch0].[ID] = [#fetch1].[OrderID]))"));
      Assert.That (fetchQuery2.Value.Parameters.Count, Is.EqualTo (1));
      Assert.That (fetchQuery2.Value.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (fetchQuery2.Value.Parameters[0].Value, Is.EqualTo ("Kunde 1"));
      Assert.That (fetchQuery2.Value.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem)).StorageProviderID));
    }

    [Test]
    public void QueryExecutor_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (_sqlGenerator);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (queryable.Provider.Executor), Is.Not.Null);
      }
    }

    [Test]
    public void GetStatement_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Computer> (_sqlGenerator);
        var executor = queryable.GetExecutor();

        executor.CreateStatement (GetParsedSimpleQuery ());
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).GetStatementCalled, Is.True);
      }
    }

    [Test]
    public void CreateQuery_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (_sqlGenerator);
        var executor = queryable.GetExecutor();

        ClassDefinition classDefinition = executor.StartingClassDefinition;
        CommandData statement = executor.CreateStatement(GetParsedSimpleQuery());

        executor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, statement.Statement, statement.Parameters, QueryType.Collection);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryCalled, Is.True);
      }
    }

    [Test]
    public void CreateQueryFromModel_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryModel = GetParsedSimpleQuery ();
        var executor = ObjectFactory.Create<DomainObjectQueryExecutor>(ParamList.Create (_sqlGenerator, _orderClassDefinition));

        executor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0], QueryType.Collection);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelCalled, Is.True);
      }
    }

    [Test]
    public void CreateQueryFromModelWithClassDefinition_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryModel = GetParsedSimpleQuery ();
        var executor = ObjectFactory.Create<DomainObjectQueryExecutor> (ParamList.Create (_sqlGenerator, _orderClassDefinition));

        executor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0], QueryType.Collection);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelWithClassDefinitionCalled, Is.True);
      }
    }

    private QueryModel GetParsedSimpleQuery ()
    {
      IQueryable<Computer> query = from computer in QueryFactory.CreateLinqQuery<Computer>() select computer;
      return ExpressionHelper.ParseQuery (query.Expression);
    }

    private QueryModel GetParsedCountQuery ()
    {
      var expression = ExpressionHelper.MakeExpression (() => (from computer in QueryFactory.CreateLinqQuery<Computer> () select computer).Count());
      return ExpressionHelper.ParseQuery (expression);
    }

    private QueryModel GetParsedSimpleWhereQuery ()
    {
      IQueryable<Order> query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      return ExpressionHelper.ParseQuery (query.Expression);
    }

    private QueryModel GetParsedSimpleWhereCountQuery ()
    {
      // 1
      var expression = ExpressionHelper.MakeExpression (() => (from order in QueryFactory.CreateLinqQuery<Order> () where order.OrderNumber == 1 select order).Count ());
      return ExpressionHelper.ParseQuery (expression);
    }

    private QueryModel GetParsedSimpleCustomerQuery ()
    {
      IQueryable<Customer> query = from c in QueryFactory.CreateLinqQuery<Customer> () where c.Name == "Kunde 1" select c;
      return ExpressionHelper.ParseQuery (query.Expression);
    }

    
  }
}
