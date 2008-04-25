using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

namespace Remotion.Data.DomainObjects.PerformanceTests.Database
{
public class TestDataLoader : IDisposable
{
  // types

  // static members and constants

  private const string c_testDomainFilename = "CreateTestData.sql";

  // member fields

  private SqlConnection _connection;
  private SqlTransaction _transaction;

  private bool _disposed = false;

  // construction and disposing

  public TestDataLoader (string connectionString)
  {
    _connection = new SqlConnection (connectionString);
    _connection.Open ();
  }

  // methods and properties

  public void Load ()
  {
    using (_transaction = _connection.BeginTransaction ())
    {
      ExecuteSqlFile (c_testDomainFilename);

      _transaction.Commit ();  
    }
  }

  private void ExecuteSqlFile (string sqlFile)
  {
    using (SqlCommand command = new SqlCommand (ReadFile (sqlFile), _connection, _transaction))
    {
      command.ExecuteNonQuery ();
    }
  }

  private string ReadFile (string file)
  {
    using (StreamReader reader = new StreamReader (file, Encoding.Default))
    {
      return reader.ReadToEnd ();
    }
  }

  #region IDisposable Members

  public void Dispose()
  {
    Dispose (true);
    GC.SuppressFinalize(this);
  }

  #endregion

  private void Dispose (bool disposing)
  {
    if (!_disposed && disposing)
    {
      if (_connection != null)
      {
        _connection.Close ();
        _connection = null;
      }

      if (_transaction != null)
      {
        _transaction.Dispose ();
        _transaction = null;
      }

      _disposed = true;
    }
  }
}
}
