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

using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions
{
  [TestFixture]
  public class FilterQueryResultEventsTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;
    private IQuery _queryStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _order2 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order2));
      _order3 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order3));
      _order4 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order4));

      _queryStub = MockRepository.GenerateStub<IQuery>();

      InstallExtensionMock ();
    }

    [Test]
    public void FilterQueryResult_RaisedInAllHierarchyLevels ()
    {
      using (ExtensionStrictMock.GetMockRepository ().Ordered ())
      {
        ExtensionStrictMock
            .Expect (
                mock => mock.FilterQueryResult (
                    Arg.Is (InactiveRootTransaction),
                    Arg<QueryResult<DomainObject>>.Matches (qr => qr.ToArray().SequenceEqual (new[] { _order1 }))))
            .Return (new QueryResult<DomainObject> (_queryStub, new[] { _order2 }))
            .WhenCalled (mi => Assert.That (InactiveRootTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.FilterQueryResult (
                    Arg.Is (InactiveMiddleTransaction),
                    Arg<QueryResult<DomainObject>>.Matches (qr => qr.ToArray ().SequenceEqual (new[] { _order2 }))))
            .Return (new QueryResult<DomainObject> (_queryStub, new[] { _order3 }))
            .WhenCalled (mi => Assert.That (InactiveMiddleTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.FilterQueryResult (
                    Arg.Is (ActiveSubTransaction),
                    Arg<QueryResult<DomainObject>>.Matches (qr => qr.ToArray ().SequenceEqual (new[] { _order3 }))))
            .Return (new QueryResult<DomainObject> (_queryStub, new[] { _order4 }))
            .WhenCalled (mi => Assert.That (ActiveSubTransaction.IsActive, Is.True));
      }

      var result = ActiveSubTransaction.Execute (() => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.ID == _order1.ID).ToList());

      ExtensionStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new[] { _order4 }));
    }

    [Test]
    public void FilterCustomQueryResult_RaisedInAllHierarchyLevels ()
    {
      var query = QueryFactory.CreateCustomQuery (
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT [OrderNo] FROM [Order] WHERE ID=@1",
          new QueryParameterCollection { { "@1", _order1.ID } });
      var fakeQueryResultRow1 = MockRepository.GenerateStub<IQueryResultRow> ();
      var fakeQueryResultRow2 = MockRepository.GenerateStub<IQueryResultRow> ();
      var fakeQueryResultRow3 = MockRepository.GenerateStub<IQueryResultRow> ();

      using (ExtensionStrictMock.GetMockRepository ().Ordered ())
      {
        ExtensionStrictMock
            .Expect (
                mock => mock.FilterCustomQueryResult (
                    Arg.Is (InactiveRootTransaction),
                    Arg<IQuery>.Is.Anything,
                    Arg<IEnumerable<IQueryResultRow>>.Matches (qrrs => qrrs.Select (qrr => qrr.GetConvertedValue<int>(0)).Single() == 1)))
            .Return (new[] { fakeQueryResultRow1 })
            .WhenCalled (mi => Assert.That (InactiveRootTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.FilterCustomQueryResult (
                    Arg.Is (InactiveMiddleTransaction),
                    Arg<IQuery>.Is.Anything,
                    Arg<IEnumerable<IQueryResultRow>>.List.Equal (new[] { fakeQueryResultRow1 })))
            .Return (new[] { fakeQueryResultRow2 })
            .WhenCalled (mi => Assert.That (InactiveMiddleTransaction.IsActive, Is.False));

        ExtensionStrictMock
            .Expect (
                mock => mock.FilterCustomQueryResult (
                    Arg.Is (ActiveSubTransaction),
                    Arg<IQuery>.Is.Anything,
                    Arg<IEnumerable<IQueryResultRow>>.List.Equal (new[] { fakeQueryResultRow2 })))
            .Return (new[] { fakeQueryResultRow3 })
            .WhenCalled (mi => Assert.That (ActiveSubTransaction.IsActive, Is.True));
      }
      ExtensionStrictMock.Replay();

      var result = ActiveSubTransaction.QueryManager.GetCustom (query, qrr => qrr).ToList();

      ExtensionStrictMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { fakeQueryResultRow3 }));
    }
  }
}