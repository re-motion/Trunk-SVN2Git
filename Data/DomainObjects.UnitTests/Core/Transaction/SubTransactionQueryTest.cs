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
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  [TestFixture]
  public class SubTransactionQueryTest : ClientTransactionBaseTest
  {
    [Test]
    public void ScalarQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("QueryWithoutParameter");

        Assert.AreEqual (42, ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query));
      }
    }

    [Test]
    public void ObjectQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        DomainObjectCollection queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Customer queriedObject = (Customer) queriedObjects[0];

        Assert.IsNotNull (queriedObjects);
        Assert.AreEqual (1, queriedObjects.Count);
        Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);

        Assert.AreEqual (new DateTime(2000, 1, 1), queriedObject.CustomerSince);
        Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
      }
    }

    [Test]
    public void ObjectQueryWithObjectListInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        ObjectList<Customer> queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection<Customer> (query);
        Customer queriedObject = queriedObjects[0];

        Assert.IsNotNull (queriedObjects);
        Assert.AreEqual (1, queriedObjects.Count);
        Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);

        Assert.AreEqual (new DateTime (2000, 1, 1), queriedObject.CustomerSince);
        Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
      }
    }

    [Test]
    public void ObjectQueryInSubAndRootTransaction ()
    {
      DomainObjectCollection queriedObjectsInSub;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjectsInSub = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      Query queryInRoot = new Query ("CustomerTypeQuery");
      queryInRoot.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      DomainObjectCollection queriedObjectsInRoot = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (queryInRoot);
      Assert.That (queriedObjectsInRoot, Is.EqualTo (queriedObjectsInSub));
    }

    [Test]
    public void QueriedObjectsCanBeUsedInParentTransaction ()
    {
      DomainObjectCollection queriedObjects;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      Customer queriedObject = (Customer) queriedObjects[0];

      Assert.IsNotNull (queriedObjects);
      Assert.AreEqual (1, queriedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);
    
      Assert.AreEqual (new DateTime (2000, 1, 1), queriedObject.CustomerSince);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
    }

    [Test]
    public void ChangedComittedQueriedObjectsCanBeUsedInParentTransaction ()
    {
      DomainObjectCollection queriedObjects;
      Customer queriedObject;

      Order newOrder;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        queriedObject = (Customer) queriedObjects[0];

        newOrder = Order.NewObject ();
        newOrder.Official = Official.NewObject ();
        newOrder.OrderTicket = OrderTicket.NewObject ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        queriedObject.Orders.Insert (0, newOrder);
        queriedObject.CustomerSince = null;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.IsNotNull (queriedObjects);
      Assert.AreEqual (1, queriedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);

      Assert.IsNull (queriedObject.CustomerSince);
      Assert.AreSame (newOrder, queriedObject.Orders[0]);
    }

    [Test]
    public void FilterQueryResultCalledInCorrectScope ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension extensionMock = mockRepository.Stub<IClientTransactionExtension> ();

      ClientTransactionMock.Extensions.Add ("mock", extensionMock);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("OrderQuery");
        query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

        extensionMock.FilterQueryResult (null, null, null); // expectation
        LastCall.Constraints (Mocks_Is.Same (ClientTransactionMock), Mocks_Is.Anything (), Mocks_Is.Anything ());
        LastCall.Do ((Proc<ClientTransaction, DomainObjectCollection, IQuery>) delegate
        {
          Assert.AreSame (ClientTransactionMock, ClientTransaction.Current);
        });

        mockRepository.ReplayAll ();
        ClientTransaction.Current.QueryManager.GetCollection (query);
        mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void AccessObjectInFilterQueryResult ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension extensionMock = mockRepository.Stub<IClientTransactionExtension> ();

      Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionMock.Extensions.Add ("mock", extensionMock);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Query query = new Query ("OrderQuery");
        query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

        extensionMock.FilterQueryResult (null, null, null); // expectation
        LastCall.IgnoreArguments();
        LastCall.Do ((Proc<ClientTransaction, DomainObjectCollection, IQuery>) delegate
        {
          Order.GetObject (DomainObjectIDs.Order1);
        });

        mockRepository.ReplayAll ();
        ClientTransaction.Current.QueryManager.GetCollection (query);
        mockRepository.VerifyAll ();
      }
    }
  }
}
