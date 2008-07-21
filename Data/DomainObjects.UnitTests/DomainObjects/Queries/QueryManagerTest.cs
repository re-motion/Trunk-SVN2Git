/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Queries
{
  [TestFixture]
  public class QueryManagerTest : ClientTransactionBaseTest
  {
    private RootQueryManager _queryManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _queryManager = new RootQueryManager (ClientTransactionMock);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (ClientTransactionMock, _queryManager.ClientTransaction);
    }

    [Test]
    public void GetScalarWithoutParameter ()
    {
      Assert.AreEqual (42, _queryManager.GetScalar (new Query ("QueryWithoutParameter")));
    }

    [Test]
    public void GetCollection ()
    {
      Query query = new Query ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      DomainObjectCollection customers = _queryManager.GetCollection (query);

      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers[0].ID);
      Assert.AreEqual (typeof (Customer), customers[0].GetPublicDomainObjectType());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetCollectionWithScalarQuery ()
    {
      _queryManager.GetCollection (new Query ("OrderNoSumByCustomerNameQuery"));
    }

    [Test]
    public void GetCollectionWithObjectList ()
    {
      Query query = new Query ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      ObjectList<Customer> customers = _queryManager.GetCollection<Customer> (query);
      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers[0].ID);
      Assert.IsTrue (query.CollectionType.IsAssignableFrom (customers.GetType ()));
    }

    [Test]
    public void GetCollectionWithObjectListWorksWhenAssignableCollectionType ()
    {
      Query query = new Query ("OrderByOfficialQuery");
      query.Parameters.Add ("@officialID", DomainObjectIDs.Official1);

      ObjectList<Order> orders = _queryManager.GetCollection<Order> (query);
      Assert.AreEqual (5, orders.Count);
      Assert.That (orders, Is.EquivalentTo (new object[]
      {
        Order.GetObject (DomainObjectIDs.Order1),
        Order.GetObject (DomainObjectIDs.Order2),
        Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem),
        Order.GetObject (DomainObjectIDs.Order3),
        Order.GetObject (DomainObjectIDs.Order4),
      }));
      Assert.IsTrue (query.CollectionType.IsAssignableFrom (orders.GetType ()));
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "The query returned an object of type "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer, which cannot be added to an ObjectList<Order>.")]
    public void GetCollectionWithObjectListThrowsWhenInvalidT ()
    {
      Query query = new Query ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      _queryManager.GetCollection<Order> (query);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "The query definition specifies a collection type of "
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.SpecificOrderCollection, which is not compatible with ObjectList<Order>.")]
    public void GetCollectionWithObjectListThrowsWhenUnassignableCollectionType ()
    {
      Query query = new Query ("QueryWithSpecificCollectionType");

      _queryManager.GetCollection<Order> (query);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetScalarWithCollectionQuery ()
    {
      _queryManager.GetScalar (new Query ("OrderQuery"));
    }

    [Test]
    public void GetStoredProcedureResult ()
    {
      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (new Query ("StoredProcedureQuery"));

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.Order2, orders[1].ID, "Order2");
    }

    [Test]
    public void GetStoredProcedureResultWithParameter ()
    {
      Query query = new Query ("StoredProcedureQueryWithParameter");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);
      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (query);

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orders[1].ID, "OrderWithoutOrderItem");
    }

    [Test]
    public void QueryingEnlists ()
    {
      Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in transaction

      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (new Query ("StoredProcedureQuery"));
      Assert.AreEqual (2, orders.Count, "Order count");

      foreach (Order order in orders)
        Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));

      int orderNumberSum = 0;
      foreach (Order order in orders)
        orderNumberSum += order.OrderNumber;

      Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1).OrderNumber + Order.GetObject (DomainObjectIDs.Order2).OrderNumber, orderNumberSum);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void QueriedObjectsMightNotBeEnlistableInOtherTransaction ()
    {
      ClientTransactionMock newTransaction = new ClientTransactionMock ();
      using (newTransaction.EnterDiscardingScope ())
      {
        Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in newTransaction
      }

      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (new Query ("StoredProcedureQuery"));
      Assert.AreEqual (2, orders.Count, "Order count");

      using (newTransaction.EnterDiscardingScope ())
      {
        foreach (Order order in orders)
        {
          if (!order.CanBeUsedInTransaction (newTransaction))
            newTransaction.EnlistDomainObject (order);  // this throws because there is already _another_ instance of Order1 enlisted
        }
      }
    }
  }
}
