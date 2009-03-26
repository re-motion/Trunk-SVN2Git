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
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class QueryExecutorTest : ClientTransactionBaseTest
  {
    private SqlServerGenerator _sqlGenerator;

    public override void SetUp ()
    {
      base.SetUp ();
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
    }
    
    [Test]
    public void ExecuteSingle()
    {
      SetDatabaseModifyable();

      Computer.GetObject (DomainObjectIDs.Computer1).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer2).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer3).Delete ();
      Computer.GetObject (DomainObjectIDs.Computer4).Delete ();

      ClientTransaction.Current.Commit();

      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      object instance = executor.ExecuteSingle (model, new FetchManyRequest[0]);
      Assert.IsNotNull (instance);
      Assert.AreSame (Computer.GetObject (DomainObjectIDs.Computer5), instance);
    }

    [Test]
    [ExpectedException (ExpectedMessage = "ExecuteSingle must return a single object, but the query returned 5 objects.")]
    public void ExecuteSingle_TooManyObjects()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      executor.ExecuteSingle (GetParsedSimpleQuery (), new FetchManyRequest[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteSingle_NoCurrentTransaction ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        executor.ExecuteSingle (model, new FetchManyRequest[0]);
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery();

      IEnumerable computers = executor.ExecuteCollection (model, new FetchManyRequest[0]);

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
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ExecuteCollection_WrongType ()
    {
      var executor = new QueryExecutor<DomainObject> (_sqlGenerator);
      executor.ExecuteCollection (GetParsedSimpleQuery (), new FetchManyRequest[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void QueryExecutor_ExecuteCollection_NoCurrentTransaction ()
    {
      var executor = new QueryExecutor<Computer> (_sqlGenerator);
      QueryModel model = GetParsedSimpleQuery ();

      using (ClientTransactionScope.EnterNullScope ())
      {
        executor.ExecuteCollection (model, new FetchManyRequest[0]);
      }
    }

    [Test]
    public void ExecuteSingle_WithParameters ()
    {
      var executor = new QueryExecutor<Order> (_sqlGenerator);
      QueryModel model = GetParsedSimpleWhereQuery ();

      var order = (Order) executor.ExecuteSingle (model, new FetchManyRequest[0]);

      Order expected = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreSame (expected, order);
    }

    [Test]
    public void ExecuteCollection_WithParameters()
    {
      var executor = new QueryExecutor<Order> (_sqlGenerator);
      QueryModel model = GetParsedSimpleWhereQuery ();

      IEnumerable orders = executor.ExecuteCollection (model, new FetchManyRequest[0]);

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
    public void CreateQuery()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var statement = "x";
      var commandParameters = new [] { new CommandParameter("x", "y") };
      var executor = new QueryExecutor<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
      
      var query = executor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, statement, commandParameters);
      Assert.That (query.Statement, Is.EqualTo (statement));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("x"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo ("y"));
      Assert.That (query.StorageProviderID, Is.EqualTo (classDefinition.StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
    }

    [Test]
    public void CreateQuery_FromModel ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var executor = new QueryExecutor<Order> (new SqlServerGenerator (DatabaseInfo.Instance));

      var query = executor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0]);
      Assert.That (query.Statement, Is.EqualTo ("SELECT [order].* FROM [OrderView] [order] WHERE ([order].[OrderNo] = @1)"));
      Assert.That (query.Parameters.Count, Is.EqualTo (1));
      Assert.That (query.Parameters[0].Name, Is.EqualTo ("@1"));
      Assert.That (query.Parameters[0].Value, Is.EqualTo (1));
      Assert.That (query.StorageProviderID, Is.EqualTo (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID));
      Assert.That (query.ID, Is.EqualTo ("<dynamic query>"));
    }

    [Test]
    public void CreateQuery_EagerFetchQueries ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var executor = new QueryExecutor<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
      var fetchRequest = new FetchManyRequest ((Expression<Func<Order,IEnumerable<OrderItem>>>)  (o => o.OrderItems));

      var query = executor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest });

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
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.NotInMappingRelatedObjects' is not a relation end point. Fetching it is not "
        + "supported by this LINQ provider.")]
    public void CreateQuery_EagerFetchQueries_ForNonRelationProperty ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var executor = new QueryExecutor<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
      var fetchRequest = new FetchManyRequest ((Expression<Func<Order, IEnumerable<OrderItem>>>) (o => o.NotInMappingRelatedObjects));

      executor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The member 'LastLoadMode' is a 'Field', which cannot be fetched by "
        + "this LINQ provider. Only properties can be fetched.")]
    public void CreateQuery_EagerFetchQueries_ForField ()
    {
      var queryModel = GetParsedSimpleWhereQuery ();
      var executor = new QueryExecutor<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
      var fetchRequest = new FetchOneRequest ((Expression<Func<Order, LoadMode>>) (o => o.LastLoadMode));

      executor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest });
    }

    [Test]
    public void CreateQuery_EagerFetchQueries_WithSortExpression ()
    {
      var queryModel = GetParsedSimpleCustomerQuery();
      var executor = new QueryExecutor<Customer> (new SqlServerGenerator (DatabaseInfo.Instance));
      var fetchRequest = new FetchManyRequest ((Expression<Func<Customer, IEnumerable<Order>>>) (c => c.Orders));

      var query = executor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest });

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
    public void CreateQuery_EagerFetchQueries_Recursive ()
    {
      var queryModel = GetParsedSimpleCustomerQuery ();
      var executor = new QueryExecutor<Customer> (new SqlServerGenerator (DatabaseInfo.Instance));
      var fetchRequest = new FetchManyRequest ((Expression<Func<Customer, IEnumerable<Order>>>) (c => c.Orders));
      LambdaExpression relatedObjectSelector = (Expression<Func<Order, IEnumerable<OrderItem>>>)(o => o.OrderItems);
      fetchRequest.GetOrAddInnerFetchRequest (new FetchManyRequest (relatedObjectSelector));

      var query = executor.CreateQuery ("<dynamic query>", queryModel, new[] { fetchRequest });

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
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (queryable.Provider.Executor), Is.Not.Null);
      }
    }

    [Test]
    public void GetClassDefinition_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        executor.GetClassDefinition();
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).GetClassDefinitionCalled, Is.True);
      }
    }

    [Test]
    public void GetStatement_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Computer> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        executor.CreateStatement (GetParsedSimpleQuery ());
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).GetStatementCalled, Is.True);
      }
    }

    [Test]
    public void CreateQuery_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (new SqlServerGenerator (DatabaseInfo.Instance));
        var executor = queryable.GetExecutor();

        ClassDefinition classDefinition = executor.GetClassDefinition();
        CommandData statement = executor.CreateStatement(GetParsedSimpleQuery());

        executor.CreateQuery ("<dynamic query>", classDefinition.StorageProviderID, statement.Statement, statement.Parameters);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryCalled, Is.True);
      }
    }

    [Test]
    public void CreateQueryFromModel_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryModel = GetParsedSimpleQuery ();
        var executor = ObjectFactory.Create<QueryExecutor<Order>>(ParamList.Create (new SqlServerGenerator (DatabaseInfo.Instance)));

        executor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0]);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelCalled, Is.True);
      }
    }

    [Test]
    public void CreateQueryFromModelWithClassDefinition_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (QueryExecutor<>)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryModel = GetParsedSimpleQuery ();
        var executor = ObjectFactory.Create<QueryExecutor<Order>> (ParamList.Create (new SqlServerGenerator (DatabaseInfo.Instance)));

        executor.CreateQuery ("<dynamic query>", queryModel, new FetchManyRequest[0]);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor).CreateQueryFromModelWithClassDefinitionCalled, Is.True);
      }
    }

    private QueryModel GetParsedSimpleQuery ()
    {
      IQueryable<Computer> query = from computer in QueryFactory.CreateLinqQuery<Computer>() select computer;
      return new QueryParser (query.Expression).GetParsedQuery ();
    }

    private QueryModel GetParsedSimpleWhereQuery ()
    {
      IQueryable<Order> query = from order in QueryFactory.CreateLinqQuery<Order>() where order.OrderNumber == 1 select order;
      return new QueryParser (query.Expression).GetParsedQuery ();
    }

    private QueryModel GetParsedSimpleCustomerQuery ()
    {
      IQueryable<Customer> query = from c in QueryFactory.CreateLinqQuery<Customer> () where c.Name == "Kunde 1" select c;
      return new QueryParser (query.Expression).GetParsedQuery ();
    }

    
  }
}
