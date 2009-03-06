// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Logging;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  [Explicit ("Spike only executed on demand")]
  public class EagerFetchingSpike : ClientTransactionBaseTest
  {
    [Test]
    public void GetOrders_AndRegisterFetchedItems ()
    {
      LogManager.InitializeConsole();

      Console.WriteLine ("Executing queries");

      var orderQuery = QueryFactory.CreateCollectionQuery (
          "orders",
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name,
          "SELECT [o].* FROM [Order] [o] WHERE [o].[OrderNo] IN (1,2,3,4)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      var orders = ClientTransaction.Current.QueryManager.GetCollection (orderQuery);
      Assert.That (
          orders.ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  Order.GetObject (DomainObjectIDs.Order1),
                  Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem),
                  Order.GetObject (DomainObjectIDs.Order2),
                  Order.GetObject (DomainObjectIDs.Order3)
              }));

      var orderItemsQuery = QueryFactory.CreateCollectionQuery (
          "orderItems",
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name,
          "SELECT [i].* FROM [Order] [o] INNER JOIN [OrderItem] [i] ON [o].[ID] = [i].[OrderID] WHERE [o].[OrderNo] IN (1,2,3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      var orderItems = ClientTransaction.Current.QueryManager.GetCollection (orderItemsQuery);
      Assert.That (
          orderItems.ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem1),
                  OrderItem.GetObject (DomainObjectIDs.OrderItem2),
                  OrderItem.GetObject (DomainObjectIDs.OrderItem3)
              }));

      var fetchedEndPointDefinition =
          MappingConfiguration.Current.ClassDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      RegisterFetchedRelatedObjects (orders.AsEnumerable(), fetchedEndPointDefinition, orderItems.AsEnumerable());

      Console.WriteLine ("Checking results");

      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);
      var order3 = Order.GetObject (DomainObjectIDs.Order3);
      var orderWithoutOrderItems = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      Assert.That (
          order1.OrderItems,
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem1),
                  OrderItem.GetObject (DomainObjectIDs.OrderItem2)
              }));
      Assert.That (
          order2.OrderItems,
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem3)
              }));

      Assert.That (order3.OrderItems, Is.Empty);
      Assert.That (orderWithoutOrderItems.OrderItems, Is.Empty);
    }

    [Test]
    public void GetOrders_AndRegisterFetchedItems_WithLinq ()
    {
      LogManager.InitializeConsole();

      Console.WriteLine ("Executing queries");

      var linqQuery = from o in QueryFactory.CreateLinqQuery<Order>()
                      where new[] { 1, 2, 3, 4 }.Contains (o.OrderNumber)
                      select o;
      var orders = linqQuery.ToArray();

      Assert.That (
          orders.ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  Order.GetObject (DomainObjectIDs.Order1),
                  Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem),
                  Order.GetObject (DomainObjectIDs.Order2),
                  Order.GetObject (DomainObjectIDs.Order3)
              }));


      FetchRelatedObjects (linqQuery, orders, o => o.OrderItems);

      Console.WriteLine ("Checking results");

      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);
      var order3 = Order.GetObject (DomainObjectIDs.Order3);
      var orderWithoutOrderItems = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      Assert.That (
          order1.OrderItems,
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem1),
                  OrderItem.GetObject (DomainObjectIDs.OrderItem2)
              }));
      Assert.That (
          order2.OrderItems,
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem3)
              }));

      Assert.That (
          order3.OrderItems,
          Is.EquivalentTo (
              new[]
              {
                  OrderItem.GetObject (DomainObjectIDs.OrderItem4)
              }));
      Assert.That (orderWithoutOrderItems.OrderItems, Is.Empty);
    }

    private void FetchRelatedObjects<TOriginal, TRelated> (IQueryable<TOriginal> originalQuery, IEnumerable<TOriginal> originalQueryResult, 
        Expression<Func<TOriginal, IEnumerable<TRelated>>> relatedObjectSelector)
    {
      var queryProvider = (QueryProvider) originalQuery.Provider;
      var originalQueryModel = queryProvider.GenerateQueryModel (originalQuery.Expression);
      var fetchQueryModel = FetchQueryModelCreatingVisitor.GetFetchQueryModel (originalQueryModel, relatedObjectSelector);

      Console.WriteLine (fetchQueryModel);

      var relatedStorageProviderID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TRelated)).StorageProviderID;
      var relatedStorageProvider = DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (relatedStorageProviderID);
      var linqSqlGenerator = relatedStorageProvider.LinqSqlGenerator;
      
      var newExecutor = ObjectFactory.Create<QueryExecutor<TRelated>> (ParamList.Create (linqSqlGenerator));
      var relatedObjects = newExecutor.ExecuteCollection (fetchQueryModel).Cast<TRelated>();

      var originalClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TOriginal));
      var relationMember = (PropertyInfo) ((MemberExpression) relatedObjectSelector.Body).Member;
      var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (relationMember);
      var fetchedEndPointDefinition = originalClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName);

      RegisterFetchedRelatedObjects (originalQueryResult.Cast<DomainObject>(), fetchedEndPointDefinition, relatedObjects.Cast<DomainObject>());
    }

    private void RegisterFetchedRelatedObjects (
        IEnumerable<DomainObject> originalObjects,
        IRelationEndPointDefinition fetchedRelationEndPointDefinition,
        IEnumerable<DomainObject> relatedObjects)
    {
      Console.WriteLine ("Collating results");
      MultiDictionary<DomainObject, DomainObject> orderItemsPerOrder = CollateFetchedRelatedObjects (
          fetchedRelationEndPointDefinition, relatedObjects);

      Console.WriteLine ("Registering results");
      RegisterFetchedRelatedObjects (originalObjects, fetchedRelationEndPointDefinition, orderItemsPerOrder);
    }

    private MultiDictionary<DomainObject, DomainObject> CollateFetchedRelatedObjects (
        IRelationEndPointDefinition fetchedRelationEndPointDefinition,
        IEnumerable<DomainObject> fetchedRelatedObjects)
    {
      Assert.That (fetchedRelationEndPointDefinition.IsVirtual, Is.True);
      var oppositeEndPointDefinition =
          fetchedRelationEndPointDefinition.RelationDefinition.GetOppositeEndPointDefinition (fetchedRelationEndPointDefinition);
      Assert.That (oppositeEndPointDefinition.IsVirtual, Is.False);

      var reversePropertyAccessorData = new PropertyAccessorData (oppositeEndPointDefinition.ClassDefinition, oppositeEndPointDefinition.PropertyName);

      var collatedResult = new MultiDictionary<DomainObject, DomainObject>();
      foreach (var fetchedRelatedObject in fetchedRelatedObjects)
      {
        var originatingObject =
            (DomainObject)
            new PropertyAccessor (fetchedRelatedObject, reversePropertyAccessorData, ClientTransaction.Current).GetValueWithoutTypeCheck();
        if (originatingObject != null)
          collatedResult[originatingObject].Add (fetchedRelatedObject);
      }
      return collatedResult;
    }

    private void RegisterFetchedRelatedObjects (
        IEnumerable<DomainObject> originalObjects,
        IRelationEndPointDefinition fetchedRelationEndPointDefinition,
        MultiDictionary<DomainObject, DomainObject> collatedRelatedObjects)
    {
      foreach (var originalObject in originalObjects)
      {
        var id = new RelationEndPointID (originalObject.ID, fetchedRelationEndPointDefinition);
        Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id], Is.Null);

        var relatedObjects = DomainObjectCollection.Create (fetchedRelationEndPointDefinition.PropertyType);
        relatedObjects.AddRange (collatedRelatedObjects[originalObject]);
        ClientTransactionMock.DataManager.RelationEndPointMap.RegisterCollectionEndPoint (id, relatedObjects);

        Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id], Is.Not.Null);
        Assert.That (
            ((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id]).OppositeDomainObjects, Is.SameAs (relatedObjects));
      }
    }
  }

  public class FetchQueryModelCreatingVisitor : IQueryVisitor
  {
    public static QueryModel GetFetchQueryModel<TOriginal, TRelated> (
        QueryModel originalQueryModel, Expression<Func<TOriginal, IEnumerable<TRelated>>> relatedObjectSelector)
    {
      var visitor = new FetchQueryModelCreatingVisitor (relatedObjectSelector, typeof (TOriginal), typeof (TRelated));
      originalQueryModel.Accept (visitor);
      return visitor._fetchQueryModel;
    }

    private readonly List<IBodyClause> _bodyClauses = new List<IBodyClause>();
    private readonly LambdaExpression _newSelectExpression;
    private readonly Type _originalType;
    private readonly Type _relatedType;

    private QueryModel _fetchQueryModel;
    private MainFromClause _newMainFromClause;

    private FetchQueryModelCreatingVisitor (LambdaExpression newSelectExpression, Type originalType, Type relatedType)
    {
      _newSelectExpression = newSelectExpression;
      _originalType = originalType;
      _relatedType = relatedType;
    }

    void IQueryVisitor.VisitQueryModel (QueryModel queryModel)
    {
      _newMainFromClause = new MainFromClause (queryModel.MainFromClause.Identifier, queryModel.MainFromClause.QuerySource);

      _bodyClauses.Clear();
      foreach (var bodyClause in queryModel.BodyClauses)
        bodyClause.Accept (this);

      var newFromClause = CreateNewFromClause (_bodyClauses.Last(), _newSelectExpression);
      _bodyClauses.Add (newFromClause);

      var newSelectClause = new SelectClause (_bodyClauses.Last(), Expression.Lambda (newFromClause.Identifier, newFromClause.Identifier));
      _fetchQueryModel = new QueryModel (_newSelectExpression.Type, _newMainFromClause, newSelectClause);
      foreach (var bodyClause in _bodyClauses)
        _fetchQueryModel.AddBodyClause (bodyClause);
    }

    private MemberFromClause CreateNewFromClause (IBodyClause previousClause, LambdaExpression newFromExpression)
    {
      var sourceParameter = Expression.Parameter (_originalType, "transparent");
      var collectionElementParameter = Expression.Parameter (_relatedType, "x");
      var newProjectionExpression = Expression.Lambda (collectionElementParameter, sourceParameter, collectionElementParameter);
      return new MemberFromClause (previousClause, collectionElementParameter, newFromExpression, newProjectionExpression);
    }

    void IQueryVisitor.VisitMainFromClause (MainFromClause fromClause)
    {
      throw new System.NotImplementedException();
    }

    void IQueryVisitor.VisitAdditionalFromClause (AdditionalFromClause fromClause)
    {
      _bodyClauses.Add (
          new AdditionalFromClause (GetPreviousClause(), fromClause.Identifier, fromClause.FromExpression, fromClause.ProjectionExpression));
    }

    private IClause GetPreviousClause ()
    {
      if (_bodyClauses.Count == 0)
        return _newMainFromClause;
      else
        return _bodyClauses.Last();
    }

    public void VisitMemberFromClause (MemberFromClause fromClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitSubQueryFromClause (SubQueryFromClause clause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitJoinClause (JoinClause joinClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitLetClause (LetClause letClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitWhereClause (WhereClause whereClause)
    {
      _bodyClauses.Add (new WhereClause (GetPreviousClause(), whereClause.BoolExpression));
    }

    public void VisitOrderByClause (OrderByClause orderByClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitOrderingClause (OrderingClause orderingClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitSelectClause (SelectClause selectClause)
    {
      throw new System.NotImplementedException();
    }

    public void VisitGroupClause (GroupClause groupClause)
    {
      throw new System.NotImplementedException();
    }
  }
}
