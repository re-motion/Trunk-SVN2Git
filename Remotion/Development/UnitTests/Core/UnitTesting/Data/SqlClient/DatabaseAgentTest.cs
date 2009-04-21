// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Data.SqlClient
{
  [TestFixture]
  public class DatabaseAgentTest
  {
    private MockRepository _mockRepository;
    private IDbCommand _commandMock;
    private IDbConnection _connectionStub;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _connectionStub = _mockRepository.Stub<IDbConnection> ();
      _commandMock = _mockRepository.StrictMock<IDbCommand> ();

      SetupResult.For (_connectionStub.CreateCommand ()).Return (_commandMock);
    }

    [Test]
    public void ExecuteScalarCommand ()
    {
      SetupCommandExpectations ("my command", null, delegate { Expect.Call (_commandMock.ExecuteScalar ()).Return ("foo"); });

      _mockRepository.ReplayAll();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      object result = agent.ExecuteScalarCommand ("my command");
      Assert.That (result, Is.EqualTo ("foo"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteCommand_ReturnsCount ()
    {
      SetupCommandExpectations ("my command", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery()).Return (15); });

      _mockRepository.ReplayAll ();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      int result = agent.ExecuteCommand ("my command");
      Assert.That (result, Is.EqualTo (15));

      _mockRepository.VerifyAll ();
    }

    private void SetupCommandExpectations (string commandText, IDbTransaction transaction, Action actualCommandExpectation)
    {
      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered ())
        {
          _commandMock.CommandType = CommandType.Text;
          _commandMock.CommandText = commandText;
          _commandMock.Transaction = transaction;
        }

        actualCommandExpectation ();
        _commandMock.Dispose ();
      }
    }
  }
}