// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.Core
{
  [TestFixture]
  public class QueryableBaseTest
  {
    private IQueryProvider _providerMock;
    private MockRepository _mockRepository;
    private Expression _intArrayExpression;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _providerMock = _mockRepository.StrictMock<IQueryProvider> ();

      _intArrayExpression = ExpressionHelper.CreateNewIntArrayExpression ();
    }

    [Test]
    public void Initialize_WithProviderAndExpression ()
    {
      QueryableBase<int> queryable = new TestQueryable<int> (_providerMock, _intArrayExpression);

      Assert.That (queryable.Provider, Is.SameAs (_providerMock));
      Assert.That (queryable.Expression, Is.SameAs (_intArrayExpression));

      Assert.That (queryable.ElementType, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void Initialize_WithProvider ()
    {
      QueryableBase<int> queryable = new TestQueryable<int> (_providerMock);

      Assert.That (queryable.Provider, Is.SameAs (_providerMock));
      Assert.That (queryable.Expression, Is.Not.Null);
      Assert.That (queryable.Expression.NodeType, Is.EqualTo (ExpressionType.Constant));
    }

    [Test]
    public void Initialize_WithParserAndExecutor ()
    {
      var fakeExecutor = _mockRepository.Stub<IQueryExecutor>();
      var fakeParser = _mockRepository.Stub<IQueryParser> ();
      QueryableBase<int> queryable = new TestQueryable<int> (fakeParser, fakeExecutor);

      Assert.That (queryable.Expression, Is.Not.Null);
      Assert.That (queryable.Expression.NodeType, Is.EqualTo (ExpressionType.Constant));
      Assert.That (queryable.Provider, Is.InstanceOf (typeof (DefaultQueryProvider)));

      var queryProvider = ((DefaultQueryProvider) queryable.Provider);
      Assert.That (queryProvider.Executor, Is.SameAs (fakeExecutor));
      Assert.That (queryProvider.QueryableType, Is.SameAs (typeof (TestQueryable<>)));
      Assert.That (queryProvider.QueryParser, Is.SameAs (fakeParser));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'expression' is a 'System.Int32[]', which cannot be assigned to type 'System.Collections.Generic.IEnumerable`1[System.String]'."
        + "\r\nParameter name: expression")]
    public void ConstructorThrowsTypeException ()
    {
      new TestQueryable<string> (_providerMock, _intArrayExpression);
    }

    [Test]
    public void GenericGetEnumerator ()
    {
      _providerMock.Expect (mock => mock.Execute<IEnumerable<int>> (_intArrayExpression)).Return (new List<int>(0));

      _providerMock.Replay ();
      QueryableBase<int> queryable = new TestQueryable<int> (_providerMock, _intArrayExpression);
      queryable.GetEnumerator();
      _providerMock.VerifyAllExpectations();
    }

    [Test]
    public void GetEnumerator()
    {
      _providerMock.Expect (mock => mock.Execute<IEnumerable> (_intArrayExpression)).Return (new List<int>());

      _providerMock.Replay ();
      QueryableBase<int> queryable = new TestQueryable<int> (_providerMock, _intArrayExpression);
      ((IEnumerable)queryable).GetEnumerator ();
      _providerMock.VerifyAllExpectations ();
    }
  }
}
