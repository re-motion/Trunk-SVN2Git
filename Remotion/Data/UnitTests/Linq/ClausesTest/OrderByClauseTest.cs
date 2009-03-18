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
using Remotion.Data.Linq;
using Rhino.Mocks;
using Remotion.Data.Linq.Clauses;


namespace Remotion.Data.UnitTests.Linq.ClausesTest
{
  [TestFixture]
  public class OrderByClauseTest
  {
    [Test]
    public void InitializeWithoutOrdering()
    {
      var previousClause = ExpressionHelper.CreateClause ();
      var orderBy = new OrderByClause (previousClause);

      Assert.AreEqual (0, orderBy.OrderingList.Count);
      Assert.IsNotNull (orderBy.PreviousClause);
      Assert.AreSame (previousClause, orderBy.PreviousClause);
    }
    
    [Test]
    public void AddOrderings()
    {
      var previousClause = ExpressionHelper.CreateClause ();
      var orderBy = new OrderByClause (previousClause);

      Ordering ordering1 = ExpressionHelper.CreateOrdering ();
      Ordering ordering2 = ExpressionHelper.CreateOrdering ();

      orderBy.AddOrdering (ordering1);
      orderBy.AddOrdering (ordering2);

      Assert.That (orderBy.OrderingList, Is.EqualTo (new object[] { ordering1, ordering2 }));
      Assert.AreEqual (2, orderBy.OrderingList.Count);

      Assert.IsNotNull (orderBy.PreviousClause);
    }

    [Test]
    public void Accept()
    {
      OrderByClause orderByClause = ExpressionHelper.CreateOrderByClause ();

      var repository = new MockRepository();
      var visitorMock = repository.StrictMock<IQueryVisitor>();

      visitorMock.VisitOrderByClause(orderByClause);

      repository.ReplayAll();

      orderByClause.Accept (visitorMock);

      repository.VerifyAll();
    }

    [Test]
    public void QueryModelAtInitialization ()
    {
      OrderByClause orderByClause = ExpressionHelper.CreateOrderByClause ();
      Assert.IsNull (orderByClause.QueryModel);
    }

    [Test]
    public void SetQueryModel ()
    {
      OrderByClause orderByClause = ExpressionHelper.CreateOrderByClause ();
      QueryModel model = ExpressionHelper.CreateQueryModel ();
      orderByClause.SetQueryModel (model);
      Assert.IsNotNull (orderByClause.QueryModel);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void SetQueryModelWithNull_Exception ()
    {
      OrderByClause orderByClause = ExpressionHelper.CreateOrderByClause ();

      orderByClause.SetQueryModel (null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "QueryModel is already set")]
    public void SetQueryModelTwice_Exception ()
    {
      OrderByClause orderByClause = ExpressionHelper.CreateOrderByClause ();
      QueryModel model = ExpressionHelper.CreateQueryModel ();
      orderByClause.SetQueryModel (model);
      orderByClause.SetQueryModel (model);
    }

    [Test]
    public void Clone ()
    {
      var originalClause = ExpressionHelper.CreateOrderByClause ();
      var newPreviousClause = ExpressionHelper.CreateClause ();
      var clone = originalClause.Clone (newPreviousClause);

      Assert.That (clone, Is.Not.Null);
      Assert.That (clone, Is.Not.SameAs (originalClause));
      Assert.That (clone.PreviousClause, Is.SameAs (newPreviousClause));
      Assert.That (clone.QueryModel, Is.Null);
    }

    [Test]
    public void Clone_Orderings ()
    {
      var orderByClause = ExpressionHelper.CreateOrderByClause ();
      var ordering = ExpressionHelper.CreateOrdering();
      orderByClause.AddOrdering (ordering);

      var newPreviousClause = ExpressionHelper.CreateMainFromClause ();
      var clone = orderByClause.Clone (newPreviousClause);

      Assert.That (clone.OrderingList.Count, Is.EqualTo (1));

      Assert.That (clone.OrderingList[0], Is.Not.SameAs (orderByClause.OrderingList[0]));
      Assert.That (clone.OrderingList[0].Expression, Is.SameAs (orderByClause.OrderingList[0].Expression));
      Assert.That (clone.OrderingList[0].OrderingDirection, Is.EqualTo (orderByClause.OrderingList[0].OrderingDirection));
      Assert.That (clone.OrderingList[0].QueryModel, Is.Null);
      Assert.That (clone.OrderingList[0].OrderByClause, Is.SameAs (clone));
    }
  }
}
