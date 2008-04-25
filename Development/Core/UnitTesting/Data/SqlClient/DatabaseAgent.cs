using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Data.SqlClient
{
  /// <summary>Use the <see cref="DatabaseAgent"/> for setting up the database during unit testing.</summary>
  public class DatabaseAgent
  {
    private string _connectionString;

    public DatabaseAgent (string connectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);

      _connectionString = connectionString;
    }

    public void SetDatabaseReadWrite (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("database", database);
      ExecuteCommand (string.Format ("ALTER DATABASE [{0}] SET READ_WRITE WITH ROLLBACK IMMEDIATE", database));
    }

    public void SetDatabaseReadOnly (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("database", database);
      ExecuteCommand (string.Format ("ALTER DATABASE [{0}] SET READ_ONLY WITH ROLLBACK IMMEDIATE", database));
    }

    public void ExecuteBatch (string sqlFileName, bool useTransaction)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();
        if (useTransaction)
        {
          using (SqlTransaction transaction = connection.BeginTransaction ())
          {
            ExecuteBatch (connection, transaction, sqlFileName);
            transaction.Commit ();
          }
        }
        else
        {
          ExecuteBatch (connection, null, sqlFileName);
        }
      }
    }

    public void ExecuteCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();
        ExecuteCommand (connection, null, commandText);
      }
    }

    protected virtual void ExecuteBatch (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      foreach (string commandText in GetCommandTextBatchesFromFile (sqlFileName))
        ExecuteCommand (connection, transaction, commandText);
    }

    private void ExecuteCommand (SqlConnection connection, SqlTransaction transaction, string commandText)
    {
      using (SqlCommand command = new SqlCommand (commandText, connection, transaction))
      {
        command.ExecuteNonQuery();
      }
    }

    private string[] GetCommandTextBatchesFromFile (string sqlFileName)
    {
      if (!Path.IsPathRooted (sqlFileName))
      {
        string assemblyUrl = typeof (DatabaseAgent).Assembly.CodeBase;
        Uri uri = new Uri (assemblyUrl);
        sqlFileName = Path.Combine (Path.GetDirectoryName(uri.LocalPath), sqlFileName);
      }
      string fileContent = File.ReadAllText (sqlFileName, Encoding.Default);
      return fileContent.Split (new string[] {"\r\nGO\r\n", "\nGO\n"}, StringSplitOptions.RemoveEmptyEntries);
    }
  }
}