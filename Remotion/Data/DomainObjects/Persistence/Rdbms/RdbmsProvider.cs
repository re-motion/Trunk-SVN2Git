// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public abstract class RdbmsProvider: StorageProvider
  {
    // types

    // static members and constants


    public static string GetClassIDColumnName (string columnName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      return columnName + "ClassID";
    }

    // member fields

    private IDbConnection _connection;
    private IDbTransaction _transaction;
    private readonly DataContainerLoader _dataContainerLoader;

    // construction and disposing

    protected RdbmsProvider (RdbmsProviderDefinition definition)
        : base (definition)
    {
      _dataContainerLoader = new DataContainerLoader (this);
    }

    protected override void Dispose (bool disposing)
    {
      if (!IsDisposed)
      {
        try
        {
          if (disposing)
          {
            DisposeTransaction();
            DisposeConnection();
          }
        }
        finally
        {
          base.Dispose (disposing);
        }
      }
    }

    // abstract methods and properties

    public abstract string GetParameterName (string name);

    protected abstract IDbConnection CreateConnection();

    public abstract string GetColumnsFromSortExpression (string sortExpression);

    // methods and properties

    public virtual void Connect()
    {
      CheckDisposed();

      if (!IsConnected)
      {
        try
        {
          _connection = CreateConnection();
          if (_connection.ConnectionString == null || _connection.ConnectionString == string.Empty)
            _connection.ConnectionString = this.Definition.ConnectionString;

          _connection.Open();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while opening connection.");
        }
      }
    }

    public virtual void Disconnect()
    {
      Dispose();
    }

    public virtual bool IsConnected
    {
      get
      {
        if (_connection == null)
          return false;

        return _connection.State != ConnectionState.Closed;
      }
    }

    public override void BeginTransaction()
    {
      CheckDisposed();

      Connect();

      if (_transaction != null)
        throw new InvalidOperationException ("Cannot call BeginTransaction when a transaction is already in progress.");

      try
      {
        _transaction = _connection.BeginTransaction (IsolationLevel);
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while executing BeginTransaction.");
      }
    }

    public virtual IsolationLevel IsolationLevel
    {
      get { return IsolationLevel.Serializable; }
    }

    public override void Commit()
    {
      CheckDisposed();

      if (_transaction == null)
      {
        throw new InvalidOperationException (
            "Commit cannot be called without calling BeginTransaction first.");
      }

      try
      {
        _transaction.Commit();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while executing Commit.");
      }
      finally
      {
        DisposeTransaction();
      }
    }

    public override void Rollback()
    {
      CheckDisposed();

      if (_transaction == null)
      {
        throw new InvalidOperationException (
            "Rollback cannot be called without calling BeginTransaction first.");
      }

      try
      {
        _transaction.Rollback();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while executing Rollback.");
      }
      finally
      {
        DisposeTransaction();
      }
    }

    public override DataContainer[] ExecuteCollectionQuery (IQuery query)
    {
      CheckDisposed();
      CheckQuery (query, QueryType.Collection, "query");

      Connect();

      QueryCommandBuilder commandBuilder = new QueryCommandBuilder (this, query);
      return LoadDataContainers (commandBuilder, true);
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      // TODO: ExecuteScalarQuery must not return DBNull.Value, but null instead. Verify this with a unit test.

      CheckDisposed();
      CheckQuery (query, QueryType.Scalar, "query");

      Connect();

      QueryCommandBuilder commandBuilder = new QueryCommandBuilder (this, query);
      using (IDbCommand command = commandBuilder.Create())
      {
        try
        {
          return command.ExecuteScalar();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while executing SQL command for query '{0}'.", query.ID);
        }
      }
    }

    public override DataContainer LoadDataContainer (ObjectID id)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("id", id);
      CheckStorageProviderID (id, "id");

      Connect ();

      return _dataContainerLoader.LoadDataContainerFromID (id);
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("ids", ids);
      foreach (ObjectID id in ids)
        CheckStorageProviderID (id, "ids");

      Connect ();

      return _dataContainerLoader.LoadDataContainersFromIDs (ids);
    }

    protected internal virtual DataContainer[] LoadDataContainers (CommandBuilder commandBuilder, bool allowNulls)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      return _dataContainerLoader.LoadDataContainersFromCommandBuilder (commandBuilder, allowNulls);
    }

    public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);
      CheckClassDefinition (classDefinition, "classDefinition");

      Connect();

      return _dataContainerLoader.LoadDataContainersByRelatedID (classDefinition, propertyName, relatedID);
    }

    public override void Save (DataContainerCollection dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      Connect();

      foreach (DataContainer dataContainer in dataContainers.GetByState (StateType.New))
        Save (new InsertCommandBuilder (this, dataContainer), dataContainer.ID);

      foreach (DataContainer dataContainer in dataContainers)
      {
        if (dataContainer.State != StateType.Unchanged)
          Save (new UpdateCommandBuilder (this, dataContainer), dataContainer.ID);
      }

      foreach (DataContainer dataContainer in dataContainers.GetByState (StateType.Deleted))
        Save (new DeleteCommandBuilder (this, dataContainer), dataContainer.ID);
    }

    public override void SetTimestamp (DataContainerCollection dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      Connect();

      foreach (DataContainer dataContainer in dataContainers)
      {
        if (dataContainer.State != StateType.Deleted)
          SetTimestamp (dataContainer);
      }
    }

    public virtual IDataReader ExecuteReader (IDbCommand command, CommandBehavior behavior)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckValidEnumValue ("behavior", behavior);

      try
      {
        return command.ExecuteReader (behavior);
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while executing SQL command.");
      }
    }

    public IDbConnection Connection
    {
      get
      {
        CheckDisposed();
        return _connection;
      }
    }

    public IDbTransaction Transaction
    {
      get
      {
        CheckDisposed();
        return _transaction;
      }
    }

    public virtual DataContainerLoader DataContainerLoader
    {
      get { return _dataContainerLoader; }
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinition (classDefinition, "classDefinition");

      return new ObjectID (classDefinition.ID, Guid.NewGuid());
    }

    protected virtual void SetTimestamp (DataContainer dataContainer)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State == StateType.Deleted)
        throw CreateArgumentException ("dataContainer", "Timestamp cannot be set for a deleted DataContainer.");

      SelectCommandBuilder commandBuilder = SelectCommandBuilder.CreateForIDLookup (
          this, DelimitIdentifier ("Timestamp"), dataContainer.ClassDefinition.GetEntityName(), dataContainer.ID);

      using (IDbCommand command = commandBuilder.Create())
      {
        try
        {
          dataContainer.SetTimestamp (command.ExecuteScalar());
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while setting timestamp for object '{0}'.", dataContainer.ID);
        }
      }
    }

    protected void Save (CommandBuilder commandBuilder, ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("id", id);
      CheckStorageProviderID (id, "id");

      using (IDbCommand command = commandBuilder.Create())
      {
        if (command == null)
          return;

        int recordsAffected = 0;
        try
        {
          recordsAffected = command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while saving object '{0}'.", id);
        }

        if (recordsAffected != 1)
        {
          throw CreateConcurrencyViolationException (
              "Concurrency violation encountered. Object '{0}' has already been changed by someone else.", id);
        }
      }
    }

    public new RdbmsProviderDefinition Definition
    {
      get
      {
        // CheckDisposed is not necessary here, because StorageProvider.Definition already checks this.
        return (RdbmsProviderDefinition) base.Definition;
      }
    }

    protected internal virtual IDbCommand CreateDbCommand()
    {
      CheckDisposed();

      IDbCommand command = _connection.CreateCommand();

      try
      {
        command.Connection = _connection;
        command.Transaction = _transaction;
      }
      catch (Exception)
      {
        command.Dispose();
        throw;
      }

      return command;
    }

    protected internal RdbmsProviderException CreateRdbmsProviderException (
        string formatString,
        params object[] args)
    {
      return CreateRdbmsProviderException (null, formatString, args);
    }

    protected internal RdbmsProviderException CreateRdbmsProviderException (
        Exception innerException,
        string formatString,
        params object[] args)
    {
      return new RdbmsProviderException (string.Format (formatString, args), innerException);
    }


    protected ConcurrencyViolationException CreateConcurrencyViolationException (
        string formatString,
        params object[] args)
    {
      return CreateConcurrencyViolationException (null, formatString, args);
    }

    protected ConcurrencyViolationException CreateConcurrencyViolationException (
        Exception innerException,
        string formatString,
        params object[] args)
    {
      return new ConcurrencyViolationException (string.Format (formatString, args), innerException);
    }

    private void DisposeTransaction()
    {
      if (_transaction != null)
        _transaction.Dispose();

      _transaction = null;
    }

    private void DisposeConnection()
    {
      if (_connection != null)
        _connection.Close();

      _connection = null;
    }

    private void CheckStorageProviderID (ObjectID id, string argumentName)
    {
      if (id.StorageProviderID != ID)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ObjectID '{1}' does not match with this StorageProvider's ID '{2}'.",
            id.StorageProviderID,
            id,
            ID);
      }
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageProviderID != ID)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageProviderID,
            ID);
      }
    }

    /// <summary> Gets a value converter that converts database types to .NET types according to the providers type mapping rules. </summary>
    public virtual ValueConverter CreateValueConverter ()
    {
      return new ValueConverter (this, TypeConversionProvider);
    }

    /// <summary> Surrounds an identifier with delimiters according to the database's syntax. </summary>
    public abstract string DelimitIdentifier (string identifier);

    /// <summary> A delimiter to end a SQL statement if the database requires one, an empty string otherwise. </summary>
    public abstract string StatementDelimiter { get; }
  }
}
