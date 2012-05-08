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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Queries
{
  [TestFixture]
  public class CustomQueryDatabaseBehaviorTest : QueryTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;
    private IPersistenceExtension _persistenceExtensionMock;

    public override void SetUp ()
    {
      base.SetUp();

      var persistenceExtensionFactoryStub = MockRepository.GenerateStub<IPersistenceExtensionFactory>();
      _persistenceExtensionMock = MockRepository.GenerateMock<IPersistenceExtension>();

      persistenceExtensionFactoryStub
          .Stub (stub => stub.CreatePersistenceExtensions (TestableClientTransaction.ID))
          .Return (new[] { _persistenceExtensionMock });

      var locator = new DefaultServiceLocator();
      locator.Register (typeof (IPersistenceExtensionFactory), () => persistenceExtensionFactoryStub);
      _serviceLocatorScope = new ServiceLocatorScope (locator);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();
      base.TearDown();
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void CompleteIteration_CompletelyExecutesQuery ()
    {
      _persistenceExtensionMock.Replay();

      var query = QueryFactory.CreateQueryFromConfiguration ("CustomQuery");

      QueryManager.GetCustom (query, ExtractCustomObject_RawValues).ToList();

      _persistenceExtensionMock
        .AssertWasCalled (mock => mock.ConnectionOpened (Arg<Guid>.Is.Anything));
      _persistenceExtensionMock
          .AssertWasCalled (
              mock =>
              mock.QueryExecuting (
                  Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<string>.Is.Anything, Arg<IDictionary<string, object>>.Is.Anything));
      _persistenceExtensionMock
        .AssertWasCalled (mock => mock.QueryExecuted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything));
      _persistenceExtensionMock.AssertWasCalled (mock => mock.QueryCompleted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything, Arg<int>.Is.Anything));
      _persistenceExtensionMock.AssertWasCalled (mock => mock.ConnectionClosed (Arg<Guid>.Is.Anything));
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void NoIteration_DoesNotOpenConnection ()
    {
      _persistenceExtensionMock.Replay ();

      var query = QueryFactory.CreateQueryFromConfiguration ("CustomQuery");

      QueryManager.GetCustom (query, ExtractCustomObject_RawValues);

      _persistenceExtensionMock
        .AssertWasNotCalled (mock => mock.ConnectionOpened (Arg<Guid>.Is.Anything));
      _persistenceExtensionMock
          .AssertWasNotCalled (
              mock =>
              mock.QueryExecuting (
                  Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<string>.Is.Anything, Arg<IDictionary<string, object>>.Is.Anything));
      _persistenceExtensionMock
        .AssertWasNotCalled (mock => mock.QueryExecuted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything));
      _persistenceExtensionMock
        .AssertWasNotCalled (mock => mock.QueryCompleted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything, Arg<int>.Is.Anything));
      _persistenceExtensionMock.AssertWasNotCalled (mock => mock.ConnectionClosed (Arg<Guid>.Is.Anything));
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void DuringIteration_QueryStaysActive ()
    {
      _persistenceExtensionMock.Replay ();

      var query = QueryFactory.CreateQueryFromConfiguration ("CustomQuery");

      var result = QueryManager.GetCustom (query, ExtractCustomObject_RawValues);

      using (var iterator = result.GetEnumerator ())
      {
        iterator.MoveNext();

        _persistenceExtensionMock
        .AssertWasCalled (mock => mock.ConnectionOpened (Arg<Guid>.Is.Anything));
        _persistenceExtensionMock
            .AssertWasCalled (
                mock =>
                mock.QueryExecuting (
                    Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<string>.Is.Anything, Arg<IDictionary<string, object>>.Is.Anything));
        _persistenceExtensionMock
          .AssertWasCalled (mock => mock.QueryExecuted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything));
        _persistenceExtensionMock.AssertWasNotCalled (mock => mock.QueryCompleted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything, Arg<int>.Is.Anything));
        _persistenceExtensionMock.AssertWasNotCalled (mock => mock.ConnectionClosed (Arg<Guid>.Is.Anything));
      }

      _persistenceExtensionMock.AssertWasCalled (mock => mock.QueryCompleted (Arg<Guid>.Is.Anything, Arg<Guid>.Is.Anything, Arg<TimeSpan>.Is.Anything, Arg<int>.Is.Anything));
      _persistenceExtensionMock.AssertWasCalled (mock => mock.ConnectionClosed (Arg<Guid>.Is.Anything));
    }

    private object ExtractCustomObject_RawValues (IQueryResultRow queryResultRow)
    {
      return new[]
             {
                 queryResultRow.GetRawValue (0), queryResultRow.GetRawValue (1), queryResultRow.GetRawValue (2), queryResultRow.GetRawValue (3),
                 queryResultRow.GetRawValue (4)
             };
    }
  }
}