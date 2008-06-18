using System;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.UnitTesting.Data.SqlClient
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
      _commandMock = _mockRepository.CreateMock<IDbCommand> ();

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

    private void SetupCommandExpectations (string commandText, IDbTransaction transaction, Proc actualCommandExpectation)
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