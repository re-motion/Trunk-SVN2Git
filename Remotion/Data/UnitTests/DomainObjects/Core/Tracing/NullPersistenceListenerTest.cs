// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Tracing
{
  [TestFixture]
  public class NullPersistenceListenerTest
  {
    private IPersistenceListener _listener;
    private Guid _connectionID;

    [SetUp]
    public void SetUp ()
    {
      _listener = NullPersistenceListener.Instance;
      _connectionID = Guid.NewGuid();
    }

    [Test]
    public void IsNull ()
    {
      var result = _listener.IsNull;
      Assert.That (result, Is.True);
    }

    [Test]
    public void ConnectionOpened ()
    {
      _listener.ConnectionOpened (_connectionID);
    }

    [Test]
    public void ConnectionClosed ()
    {
      _listener.ConnectionClosed (_connectionID);
    }

    [Test]
    public void TransactionBegan ()
    {
      _listener.TransactionBegan (_connectionID, IsolationLevel.Chaos);      
    }

    [Test]
    public void TransactionCommitted ()
    {
      _listener.TransactionCommitted (_connectionID);      
    }

    [Test]
    public void TransactionRolledBack ()
    {
      _listener.TransactionRolledBack (_connectionID);
    }

    [Test]
    public void TransactionDisposed ()
    {
      _listener.TransactionDisposed (_connectionID);
    }

    [Test]
    public void QueryExecuting ()
    {
      var queryID = Guid.NewGuid();
      var commandText = "commandText";
      var parameters = MockRepository.GenerateMock<IDictionary<string, object>>();
      _listener.QueryExecuting (_connectionID, queryID, commandText, parameters);
    }

    [Test]
    public void QueryExecuted ()
    {
      var queryID = Guid.NewGuid();
      var durationOfQueryExecution = new TimeSpan();
      _listener.QueryExecuted (_connectionID, queryID, durationOfQueryExecution);
    }

    [Test]
    public void QueryCompleted ()
    {
      var queryID = Guid.NewGuid();
      var durationOfDataRead = new TimeSpan();
      var rowCount = 6;
      _listener.QueryCompleted (_connectionID, queryID, durationOfDataRead, rowCount);
    }

    [Test]
    public void QueryError ()
    {
      var queryID = Guid.NewGuid();
      var ex = new Exception();
      _listener.QueryError (_connectionID, queryID, ex);
    }
  }
}