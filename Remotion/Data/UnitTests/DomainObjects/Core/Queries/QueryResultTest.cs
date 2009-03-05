// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryResultTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private Order _order2;
    private Order _order3;
    
    private IQuery _query;
    private IQuery _queryWithCustomType;
    
    private QueryResult<Order> _result;
    private QueryResult<Order> _resultWithDuplicates;
    private QueryResult<Order> _resultWithNulls;
    private QueryResult<Order> _resultWithCustomType;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);

      _query = MockRepository.GenerateStub<IQuery> ();
      _queryWithCustomType = MockRepository.GenerateStub<IQuery> ();
      _queryWithCustomType.Stub (stub => stub.CollectionType).Return (typeof (OrderCollection));

      _result = new QueryResult<Order> (_query, new[] { _order1, _order2, _order3 });
      _resultWithDuplicates = new QueryResult<Order> (_query, new[] { _order1, _order2, _order3, _order1 });
      _resultWithNulls = new QueryResult<Order> (_query, new[] { _order1, _order2, _order3, null });
      _resultWithCustomType = new QueryResult<Order> (_queryWithCustomType, new[] { _order1, _order2, _order3 });
    }

    [Test]
    public void Count ()
    {
      Assert.That (_result.Count, Is.EqualTo (3));
    }

    [Test]
    public void Query ()
    {
      Assert.That (_result.Query, Is.SameAs (_query));
    }

    [Test]
    public void ContainsDuplicates_False ()
    {
      Assert.That (_result.ContainsDuplicates (), Is.False);
    }

    [Test]
    public void ContainsDuplicates_True ()
    {
      Assert.That (_resultWithDuplicates.ContainsDuplicates (), Is.True);
    }

    [Test]
    public void ContainsNulls_False ()
    {
      Assert.That (_result.ContainsNulls (), Is.False);
    }

    [Test]
    public void ContainsNulls_True ()
    {
      Assert.That (_resultWithNulls.ContainsNulls (), Is.True);
    }

    [Test]
    public void AsEnumerable ()
    {
      Assert.That (_result.AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (_resultWithDuplicates.AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order1 }));
      Assert.That (_resultWithNulls.AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, null }));
    }

    [Test]
    public void AsEnumerable_Interface ()
    {
      Assert.That (((IQueryResult) _result).AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (((IQueryResult) _resultWithDuplicates).AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order1 }));
      Assert.That (((IQueryResult) _resultWithNulls).AsEnumerable ().ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, null }));
    }

    [Test]
    public void ToArray ()
    {
      Assert.That (_result.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (_resultWithDuplicates.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order1 }));
      Assert.That (_resultWithNulls.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, null }));
    }

    [Test]
    public void ToArray_AlwaysReturnsCopy ()
    {
      var array = _result.ToArray ();
      array[0] = null;

      var array2 = _result.ToArray ();
      Assert.That (array2, Is.Not.SameAs (array));
      Assert.That (array[0], Is.Null);
      Assert.That (array2[0], Is.Not.Null);
    }

    [Test]
    public void ToArray_Interface ()
    {
      Assert.That (((IQueryResult) _result).ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (((IQueryResult) _resultWithDuplicates).ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, _order1 }));
      Assert.That (((IQueryResult) _resultWithNulls).ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3, null }));
    }

    [Test]
    public void ToObjectList ()
    {
      var list = _result.ToObjectList ();
      Assert.That (list, Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (list.IsReadOnly, Is.False);
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "Cannot create an ObjectList for the query result: Item 3 of "
        + "argument domainObjects is null.")]
    public void ToObjectList_WithNull ()
    {
      _resultWithNulls.ToObjectList ();
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "Cannot create an ObjectList for the query result: Item 3 of "
        + "argument domainObjects is a duplicate ('Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid').")]
    public void ToObjectList_WithDuplicates ()
    {
      _resultWithDuplicates.ToObjectList ();
    }

    [Test]
    public void Interface_ToObjectList ()
    {
      var list = ((IQueryResult) _result).ToObjectList ();
      Assert.That (list, Is.EqualTo (new[] { _order1, _order2, _order3 }));
      Assert.That (list.IsReadOnly, Is.False);
    }

    [Test]
    public void ToCustomCollection_WithCollectionType ()
    {
      var collection = _resultWithCustomType.ToCustomCollection ();
      Assert.That (collection, Is.InstanceOfType (typeof (OrderCollection)));
      Assert.That (collection, Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void ToCustomCollection_WithoutCollectionType ()
    {
      var collection = _result.ToCustomCollection ();
      Assert.That (collection, Is.InstanceOfType (typeof (DomainObjectCollection)));
      Assert.That (collection, Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "Cannot create a custom collection of type 'DomainObjectCollection' for the "
        + "query result: Item 3 of argument domainObjects is null.")]
    public void ToCustomCollection_WithNull ()
    {
      _resultWithNulls.ToCustomCollection ();
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "Cannot create a custom collection of type 'DomainObjectCollection' for the "
        + "query result: Item 3 of argument domainObjects is a duplicate ('Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid').")]
    public void ToCustomCollection_WithDuplicates ()
    {
      _resultWithDuplicates.ToCustomCollection ();
    }

    [Test]
    public void ToCustomCollection_WithoutTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        var collection = _resultWithCustomType.ToCustomCollection();
        Assert.That (collection, Is.InstanceOfType (typeof (OrderCollection)));
        Assert.That (collection, Is.EqualTo (new[] { _order1, _order2, _order3 }));
      }
    }
  }
}