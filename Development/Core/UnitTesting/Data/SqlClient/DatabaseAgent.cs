/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

    public int ExecuteBatch (string sqlFileName, bool useTransaction)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      int count = 0;
      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();
        if (useTransaction)
        {
          using (SqlTransaction transaction = connection.BeginTransaction ())
          {
            count = ExecuteBatch (connection, transaction, sqlFileName);
            transaction.Commit ();
          }
        }
        else
        {
          count = ExecuteBatch (connection, null, sqlFileName);
        }
      }

      return count;
    }

    public int ExecuteCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();
        return ExecuteCommand (connection, null, commandText);
      }
    }

    protected virtual int ExecuteBatch (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      int count = 0;
      foreach (string commandText in GetCommandTextBatchesFromFile (sqlFileName))
        count += ExecuteCommand (connection, transaction, commandText);
      return count;
    }

    private int ExecuteCommand (SqlConnection connection, SqlTransaction transaction, string commandText)
    {
      using (SqlCommand command = new SqlCommand (commandText, connection, transaction))
      {
        return command.ExecuteNonQuery();
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
