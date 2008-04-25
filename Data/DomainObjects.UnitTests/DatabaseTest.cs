using System;
using System.Data;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public abstract class DatabaseTest
  {
    public const string DatabaseName = "TestDomain";
    public const string TestDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost; Max Pool Size=1;";
    public const string MasterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost; Max Pool Size=1;";

    public const string DefaultStorageProviderID = "DefaultStorageProvider";
    public const string c_testDomainProviderID = "TestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    private DatabaseAgent _databaseAgent;
    private string _createTestDataFileName;
    private bool _isDatabaseModifyable;

    protected DatabaseTest (DatabaseAgent databaseAgent, string createTestDataFileName)
    {
      ArgumentUtility.CheckNotNull ("databaseAgent", databaseAgent);
      ArgumentUtility.CheckNotNullOrEmpty ("createTestDataFileName", createTestDataFileName);

      _databaseAgent = databaseAgent;
      _createTestDataFileName = createTestDataFileName;
    }

    [SetUp]
    public virtual void SetUp ()
    {
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _databaseAgent.ExecuteBatch (_createTestDataFileName, true);
      }
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _databaseAgent.SetDatabaseReadOnly (DatabaseName);
        _isDatabaseModifyable = false;
      }
    }

    protected void SetDatabaseModifyable ()
    {
      if (!_isDatabaseModifyable)
      {
        _isDatabaseModifyable = true;
        _databaseAgent.SetDatabaseReadWrite (DatabaseName);
      }
    }

    protected IDbCommand CreateCommand (string table, Guid id, IDbConnection connection)
    {
      IDbCommand command = connection.CreateCommand ();
      command.CommandText = string.Format ("SELECT * FROM [{0}] where ID = @id", table);

      IDbDataParameter parameter = command.CreateParameter ();
      parameter.ParameterName = "@id";
      parameter.Value = id;
      command.Parameters.Add (parameter);

      return command;
    }
  }
}
