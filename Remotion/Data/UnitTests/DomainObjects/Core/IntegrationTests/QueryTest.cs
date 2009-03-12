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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Logging;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetCollectionWithExistingObjects ()
    {
      var computer2 = Computer.GetObject (DomainObjectIDs.Computer2);
      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Computer].* FROM [Computer] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", DomainObjectIDs.Computer2); // preloaded
      query.Parameters.Add ("@2", DomainObjectIDs.Computer3);
      query.Parameters.Add ("@3", DomainObjectIDs.Computer1); // preloaded

      var resultArray = queryManager.GetCollection (query).ToArray();
      Assert.That (resultArray, Is.EqualTo (new[] {computer2, Computer.GetObject (DomainObjectIDs.Computer3), computer1}));
    }

    [Test]
    public void GetCollectionWithNullValues ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(), typeof (DomainObjectCollection));
      
      query.Parameters.Add ("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add ("@3", DomainObjectIDs.Computer4); // no employee
      query.Parameters.Add ("@2", DomainObjectIDs.Computer1); // employee 3
      
      var result = queryManager.GetCollection (query);
      Assert.That (result.ContainsNulls (), Is.True);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { null, null, Employee.GetObject (DomainObjectIDs.Employee3)}));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not supported.")]
    public void GetCollectionWithDuplicates ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Order].* FROM [OrderItem] INNER JOIN [Order] ON [OrderItem].[OrderID] = [Order].[ID] WHERE [Order].[OrderNo] = 1",
          new QueryParameterCollection (), typeof (DomainObjectCollection));
      queryManager.GetCollection (query);
    }

    [Test]
    public void EagerFetching ()
    {
      LogManager.InitializeConsole();

      var ordersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderID,
          "SELECT * FROM [Order] WHERE OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition =
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          DomainObjectIDs.OrderItem1.StorageProviderID,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE o.OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      var id1 = new RelationEndPointID (DomainObjectIDs.Order1, relationEndPointDefinition);
      var id2 = new RelationEndPointID (DomainObjectIDs.Order2, relationEndPointDefinition);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Null);

      var result = ClientTransactionMock.QueryManager.GetCollection (ordersQuery);
      Assert.That (result.ToArray(), Is.EquivalentTo (new[] {Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2)} ));

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Not.Null);

      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id1]).OppositeDomainObjects,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id2]).OppositeDomainObjects,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem3) }));
    }
  }
}
