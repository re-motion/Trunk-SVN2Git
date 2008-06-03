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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectGraphTraverserTest : ClientTransactionBaseTest
  {
    private Order GetTestGraph ()
    {
      Order root = Order.NewObject ();
      root.Official = Official.NewObject ();
      root.OrderTicket = OrderTicket.NewObject ();
      root.OrderItems.Add (OrderItem.NewObject ());
      root.OrderItems.Add (OrderItem.NewObject ());
      root.Customer = Customer.NewObject ();
      root.Customer.Ceo = Ceo.NewObject ();
      return root;
    }

    private Order GetDeepTestGraph ()
    {
      Order root = Order.NewObject ();
      root.Official = Official.NewObject ();
      root.OrderTicket = OrderTicket.NewObject ();
      root.OrderItems.Add (OrderItem.NewObject ());
      root.OrderItems.Add (OrderItem.NewObject ());
      root.Customer = Customer.NewObject ();
      root.Customer.Ceo = Ceo.NewObject ();
      root.Customer.IndustrialSector = IndustrialSector.NewObject ();
      root.Customer.IndustrialSector.Companies.Add (Company.NewObject ());
      root.Customer.IndustrialSector.Companies[1].Ceo = Ceo.NewObject();
      root.Customer.IndustrialSector.Companies.Add (Company.NewObject());
      root.Customer.IndustrialSector.Companies[2].Ceo = Ceo.NewObject();
      return root;
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRoot ()
    {
      Order order = GetTestGraph();
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      Assert.That (graph, List.Contains (order));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRelatedObjects ()
    {
      Order order = GetTestGraph();
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      foreach (DomainObject relatedObject in order.Properties.GetAllRelatedObjects())
        Assert.That (graph, List.Contains (relatedObject));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsIndirectRelatedObjects ()
    {
      Order order = GetTestGraph();
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      Assert.That (graph, List.Contains (order.Customer.Ceo));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_TraversalFilter ()
    {
      MockRepository repository = new MockRepository();

      Order order = GetDeepTestGraph();
      IGraphTraversalStrategy strategy = repository.CreateMock<IGraphTraversalStrategy>();

      using (repository.Unordered())
      {
        Expect.Call (strategy.ShouldProcessObject (order)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Official)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.OrderTicket)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.OrderItems[0])).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.OrderItems[1])).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.Ceo)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.IndustrialSector)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.IndustrialSector.Companies[1])).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.IndustrialSector.Companies[1].Ceo)).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.IndustrialSector.Companies[2])).Return (true);
        Expect.Call (strategy.ShouldProcessObject (order.Customer.IndustrialSector.Companies[2].Ceo)).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order, 0, order.Properties[typeof (Order), "Official"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order, 0, order.Properties[typeof (Order), "OrderTicket"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order, 0, order.Properties[typeof (Order), "OrderItems"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order, 0, order.Properties[typeof (Order), "Customer"])).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order.Official, 1, order.Official.Properties[typeof (Official), "Orders"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.OrderTicket, 1, order.OrderTicket.Properties[typeof (OrderTicket), "Order"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.OrderItems[0], 1, order.OrderItems[0].Properties[typeof (OrderItem), "Order"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.OrderItems[1], 1, order.OrderItems[1].Properties[typeof (OrderItem), "Order"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer, 1, order.Customer.Properties[typeof (Customer), "Orders"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer, 1, order.Customer.Properties[typeof (Company), "Ceo"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer, 1, order.Customer.Properties[typeof (Company), "IndustrialSector"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer, 1, order.Customer.Properties[typeof (Company), "ClassWithoutRelatedClassIDColumnAndDerivation"])).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.Ceo, 2, order.Customer.Ceo.Properties[typeof (Ceo), "Company"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector, 2, order.Customer.IndustrialSector.Properties[typeof (IndustrialSector), "Companies"])).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[1], 3, order.Customer.IndustrialSector.Companies[1].Properties[typeof (Company), "IndustrialSector"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[1], 3, order.Customer.IndustrialSector.Companies[1].Properties[typeof (Company), "ClassWithoutRelatedClassIDColumnAndDerivation"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[1], 3, order.Customer.IndustrialSector.Companies[1].Properties[typeof (Company), "Ceo"])).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[2], 3, order.Customer.IndustrialSector.Companies[2].Properties[typeof (Company), "IndustrialSector"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[2], 3, order.Customer.IndustrialSector.Companies[2].Properties[typeof (Company), "ClassWithoutRelatedClassIDColumnAndDerivation"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[2], 3, order.Customer.IndustrialSector.Companies[2].Properties[typeof (Company), "Ceo"])).Return (true);

        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[1].Ceo, 4, order.Customer.IndustrialSector.Companies[1].Ceo.Properties[typeof (Ceo), "Company"])).Return (true);
        Expect.Call (strategy.ShouldFollowLink (order, order.Customer.IndustrialSector.Companies[2].Ceo, 4, order.Customer.IndustrialSector.Companies[2].Ceo.Properties[typeof (Ceo), "Company"])).Return (true);
      }

      repository.ReplayAll();

      Set<DomainObject> result = order.GetGraphTraverser (strategy).GetFlattenedRelatedObjectGraph();
      Assert.That (result, Is.EquivalentTo (new DomainObject[] {order, order.Official, order.OrderTicket, order.OrderItems[0], order.OrderItems[1],
          order.Customer, order.Customer.Ceo, order.Customer.Ceo.Company, order.Customer.IndustrialSector,
          order.Customer.IndustrialSector.Companies[1], order.Customer.IndustrialSector.Companies[1].Ceo,
          order.Customer.IndustrialSector.Companies[2], order.Customer.IndustrialSector.Companies[2].Ceo }));

      repository.VerifyAll();
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Set<DomainObject> graph = order.GetGraphTraverser (new TestTraversalStrategy (true, false)).GetFlattenedRelatedObjectGraph ();

      Set<DomainObject> expected = new Set<DomainObject> (
          order,
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Customer1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Official1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.IndustrialSector1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Partner1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.PartnerWithoutCeo, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Supplier1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Distributor2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person7, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person3, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person6, false));

      Assert.That (graph, Is.EquivalentTo(expected));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink_IncludeObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Set<DomainObject> graph = order.GetGraphTraverser (new TestTraversalStrategy (false, false)).GetFlattenedRelatedObjectGraph ();

      Set<DomainObject> expected = new Set<DomainObject> (
          order,
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Customer1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Official1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.IndustrialSector1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Partner1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.PartnerWithoutCeo, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Supplier1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Distributor2, false));

      Assert.That (graph, Is.EquivalentTo (expected));
    }
     
    [Test]
    public void Traversal_NotAffectedByNotProcessingAnObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Set<DomainObject> graph = order.GetGraphTraverser (new TestTraversalStrategy (false, true)).GetFlattenedRelatedObjectGraph ();

      Set<DomainObject> expected = new Set<DomainObject> (RepositoryAccessor.GetObject (DomainObjectIDs.Distributor2, false));

      Assert.That (graph, Is.EquivalentTo (expected));
    }

    class TestTraversalStrategy : IGraphTraversalStrategy
    {
      private readonly bool _includePersons;
      private readonly bool _includeOnlyDistributors;

      public TestTraversalStrategy (bool includePersons, bool includeOnlyDistributors)
      {
        _includePersons = includePersons;
        _includeOnlyDistributors = includeOnlyDistributors;
      }

      public bool ShouldProcessObject (DomainObject domainObject)
      {
        return (!_includeOnlyDistributors || domainObject is Distributor)
            && (_includePersons || !(domainObject is Person));
      }

      public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
      {
        return !typeof (Ceo).IsAssignableFrom (linkProperty.PropertyType)
          && !typeof (Order).IsAssignableFrom (linkProperty.PropertyType)
          && !typeof (ObjectList<Order>).IsAssignableFrom (linkProperty.PropertyType);
      }
    }
  }
}
