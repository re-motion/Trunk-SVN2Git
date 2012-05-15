// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : StandardMappingTest
  {
    private ClassDefinition _orderClassDefinition;
    private IDomainObjectQueryGenerator _queryGeneratorMock;
    private DomainObjectQueryExecutor _queryExecutor;

    private IQueryManager _queryManagerMock;
    private ClientTransactionScope _transactionScope;

    private QueryModel _someQueryModel;
    private Order _someOrder;
    private IExecutableQuery<int> _scalarExecutableQueryMock;
    private IExecutableQuery<IEnumerable<Order>> _collectionExecutableQueryMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _queryGeneratorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator>();
      _queryExecutor = new DomainObjectQueryExecutor (_orderClassDefinition, _queryGeneratorMock);

      _queryManagerMock = MockRepository.GenerateStrictMock<IQueryManager> ();
      var transaction = ClientTransactionObjectMother.CreateTransactionWithQueryManager<ClientTransaction> (_queryManagerMock);
      _transactionScope = transaction.EnterDiscardingScope ();

      _someQueryModel = QueryModelObjectMother.Create();
      MockRepository.GenerateStub<IQuery>();
      _someOrder = DomainObjectMother.CreateFakeObject<Order>();

      _scalarExecutableQueryMock = MockRepository.GenerateStrictMock<IExecutableQuery<int>>();
      _collectionExecutableQueryMock = MockRepository.GenerateStrictMock<IExecutableQuery<IEnumerable<Order>>> ();
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown ();
    }

    [Test]
    public void ExecuteScalar ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateScalarQuery<int> (
              "<dynamic query>", _orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition, _someQueryModel))
          .Return (_scalarExecutableQueryMock);

      var fakeResult = 7;
      _scalarExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations();
      _scalarExecutableQueryMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Scalar queries cannot perform eager fetching.")]
    public void ExecuteScalar_WithFetchRequests ()
    {
      ExpectCreateQueryWithFetchQueryModelBuilders (_someQueryModel, _collectionExecutableQueryMock);

      _queryExecutor.ExecuteScalar<int> (_someQueryModel);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteScalar_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteScalar<int> (_someQueryModel);
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateSequenceQuery<Order> (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ()))
          .Return (_collectionExecutableQueryMock);

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return(fakeResult);

      var result = _queryExecutor.ExecuteCollection<Order> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _collectionExecutableQueryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs(fakeResult));
    }

    [Test]
    public void ExecuteCollection_WithFetchRequests ()
    {
      ExpectCreateQueryWithFetchQueryModelBuilders (_someQueryModel, _collectionExecutableQueryMock);

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      var result = _queryExecutor.ExecuteCollection<Order> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _collectionExecutableQueryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs(fakeResult));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteCollection_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteCollection<Order> (_someQueryModel);
      }
    }

    [Test]
    public void ExecuteSingle()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateSequenceQuery<Order> ("<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ()))
          .Return (_collectionExecutableQueryMock);

      var fakeResult = new[] { _someOrder };
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      var result = _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);

      _queryGeneratorMock.VerifyAllExpectations ();
      _collectionExecutableQueryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_someOrder));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void ExecuteSingle_MoreThanOneItem ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateSequenceQuery<Order> (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ()))
          .Return (_collectionExecutableQueryMock);

      var fakeResult = new[] { _someOrder, DomainObjectMother.CreateFakeObject<Order>() };
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
    }

    [Test]
    public void ExecuteSingle_ZeroItems_ReturnDefaultTrue ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateSequenceQuery<Order> (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ()))
          .Return (_collectionExecutableQueryMock);

      var fakeResult = new Order[0];
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      var result = _queryExecutor.ExecuteSingle<Order> (_someQueryModel, true);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteSingle_ZeroItems_ReturnDefaultFalse ()
    {
       _queryGeneratorMock
          .Expect (mock => mock.CreateSequenceQuery<Order> (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ()))
          .Return (_collectionExecutableQueryMock);

      var fakeResult = new Order[0];
      _collectionExecutableQueryMock.Expect (mock => mock.Execute (_queryManagerMock)).Return (fakeResult);

      _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteSingle_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
      }
    }

    private void CheckFetchQueryModelBuilder (
    FetchQueryModelBuilder builder, FetchRequestBase expectedFetchRequest, QueryModel expectedQueryModel, int expectedResultOperatorPosition)
    {
      Assert.That (builder.FetchRequest, Is.SameAs (expectedFetchRequest));
      Assert.That (builder.SourceItemQueryModel, Is.SameAs (expectedQueryModel));
      Assert.That (builder.ResultOperatorPosition, Is.EqualTo (expectedResultOperatorPosition));
    }

    private FetchRequestBase AddFetchRequest ()
    {
      var relationMember = MemberInfoFromExpressionUtility.GetProperty ((Order o) => o.OrderTicket);
      var fetchRequest = new FetchOneRequest (relationMember);
      _someQueryModel.ResultOperators.Add (fetchRequest);
      return fetchRequest;
    }

    private ResultOperatorBase AddSomeResultOperator ()
    {
      var someResultOperator = new DistinctResultOperator ();
      _someQueryModel.ResultOperators.Add (someResultOperator);
      return someResultOperator;
    }

    private void ExpectCreateQueryWithFetchQueryModelBuilders<T> (QueryModel expectedQueryModel, IExecutableQuery<IEnumerable<T>> executableQuery)
    {
      var nonTrailingFetchRequest = AddFetchRequest();
      var someResultOperator = AddSomeResultOperator ();
      var trailingFetchRequest1 = AddFetchRequest ();
      var trailingFetchRequest2 = AddFetchRequest ();
      Assert.That (
          expectedQueryModel.ResultOperators,
          Is.EqualTo (new[] { nonTrailingFetchRequest, someResultOperator, trailingFetchRequest1, trailingFetchRequest2 }));

      _queryGeneratorMock
          .Expect (
              mock => mock.CreateSequenceQuery<T> (
                  Arg<string>.Is.Anything,
                  Arg<ClassDefinition>.Is.Anything,
                  Arg.Is (expectedQueryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything))
          .Return (executableQuery)
          .WhenCalled (mi =>
          {
            Assert.That (expectedQueryModel.ResultOperators, Is.EqualTo (new[] { nonTrailingFetchRequest, someResultOperator }));

            var builders = ((IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3]).ToArray ();
            Assert.That (builders, Has.Length.EqualTo (2));
            CheckFetchQueryModelBuilder (builders[0], trailingFetchRequest2, expectedQueryModel, 3);
            CheckFetchQueryModelBuilder (builders[1], trailingFetchRequest1, expectedQueryModel, 2);
          });
    }
  }
}