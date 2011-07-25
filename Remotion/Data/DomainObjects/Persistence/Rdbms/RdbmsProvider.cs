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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public abstract class RdbmsProvider : StorageProvider, IRdbmsProviderCommandExecutionContext
  {
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;

    private TracingDbConnection _connection;
    private TracingDbTransaction _transaction;

    protected RdbmsProvider (
        RdbmsProviderDefinition definition,
        IStorageNameProvider storageNameProvider,
        ISqlDialect sqlDialect,
        IPersistenceListener persistenceListener,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory)
        : base (definition, storageNameProvider, sqlDialect, persistenceListener)
    {
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);
      _storageProviderCommandFactory = storageProviderCommandFactory;
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

    protected abstract IDbConnection CreateConnection ();

    /// <summary> A delimiter to end a SQL statement if the database requires one, an empty string otherwise. </summary>
    public virtual string StatementDelimiter
    {
      get { return SqlDialect.StatementDelimiter; }
    }

    /// <summary> Surrounds an identifier with delimiters according to the database's syntax. </summary>
    public virtual string DelimitIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);

      return SqlDialect.DelimitIdentifier (identifier);
    }

    public virtual string GetParameterName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return SqlDialect.GetParameterName (name);
    }

    public virtual void Connect ()
    {
      CheckDisposed();

      if (!IsConnected)
      {
        try
        {
          _connection = new TracingDbConnection (CreateConnection (), PersistenceListener);
          if (string.IsNullOrEmpty (_connection.ConnectionString))
            _connection.ConnectionString = StorageProviderDefinition.ConnectionString;

          _connection.Open();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while opening connection.");
        }
      }
    }

    public virtual void Disconnect ()
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

    public override void BeginTransaction ()
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

    public override void Commit ()
    {
      CheckDisposed();

      if (_transaction == null)
        throw new InvalidOperationException ("Commit cannot be called without calling BeginTransaction first.");

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

    public override void Rollback ()
    {
      CheckDisposed();

      if (_transaction == null)
        throw new InvalidOperationException ("Rollback cannot be called without calling BeginTransaction first.");

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
      ArgumentUtility.CheckNotNull ("query", query);
      CheckQuery (query, QueryType.Collection, "query");

      Connect();

      var command = _storageProviderCommandFactory.CreateForDataContainerQuery (query);
      var dataContainers = command.Execute (this);

      var checkedSequence = CheckForDuplicates (dataContainers, "database query");
      return checkedSequence.ToArray();
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("query", query);
      CheckQuery (query, QueryType.Scalar, "query");

      Connect();

      var commandBuilder = new QueryDbCommandBuilder (query, SqlDialect, CreateValueConverter());
      using (IDbCommand command = commandBuilder.Create (this))
      {
        try
        {
          return command.ExecuteScalar();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException (e, "Error while executing SQL command for query '{0}': {1}", query.ID, e.Message);
        }
      }
    }

    public override ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("id", id);
      CheckStorageProviderID (id, "id");

      Connect();

      var command = _storageProviderCommandFactory.CreateForSingleIDLookup (id);
      return command.Execute (this);
    }

    public override IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("ids", ids);

      Connect();

      var checkedIDs = ids.Select (id => CheckStorageProviderID (id, "ids"));
      var command = _storageProviderCommandFactory.CreateForSortedMultiIDLookup (checkedIDs);
      return command.Execute (this);
    }

    public override DataContainerCollection LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition sortExpressionDefinition,
        ObjectID relatedID)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);
      CheckClassDefinition (relationEndPointDefinition.ClassDefinition, "classDefinition");

      Connect ();

      if (relationEndPointDefinition.PropertyDefinition.StorageClass == StorageClass.Transaction)
        return new DataContainerCollection ();

      var storageProviderCommand = _storageProviderCommandFactory.CreateForRelationLookup (
          relationEndPointDefinition,
          relatedID,
          sortExpressionDefinition);
      var dataContainers = storageProviderCommand.Execute (this);

      var checkedSequence = CheckForNulls (CheckForDuplicates (dataContainers, "relation lookup"), "relation lookup");
      return new DataContainerCollection (checkedSequence, true);
    }

    public override void Save (DataContainerCollection dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      Connect();

      var saveCommand = _storageProviderCommandFactory.CreateForSave (dataContainers);
      saveCommand.Execute (this);
    }

    public override void SetTimestamp (DataContainerCollection dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      Connect();

      var objectIds = dataContainers.Where (dc => dc.State != StateType.Deleted).Select (dc => dc.ID);
      var multiTimestampLookupCommand = _storageProviderCommandFactory.CreateForMultiTimestampLookup (objectIds);
      var timestampDictionary = multiTimestampLookupCommand.Execute (this).ToDictionary (result => result.ObjectID, result => result.LocatedObject);
      
      foreach (DataContainer dataContainer in dataContainers)
      {
        if (dataContainer.State != StateType.Deleted)
        {
          object timestamp;
          if (!timestampDictionary.TryGetValue (dataContainer.ID, out timestamp))
            throw new RdbmsProviderException (string.Format ("No timestamp found for object '{0}'.", dataContainer.ID));

          dataContainer.SetTimestamp (timestamp);
        }
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
        throw CreateRdbmsProviderException (e, "Error while executing SQL command: " + e.Message);
      }
    }

    public TracingDbConnection Connection
    {
      get
      {
        CheckDisposed();
        return _connection;
      }
    }

    public TracingDbTransaction Transaction
    {
      get
      {
        CheckDisposed();
        return _transaction;
      }
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> StorageProviderCommandFactory
    {
      get { return _storageProviderCommandFactory; }
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinition (classDefinition, "classDefinition");

      return new ObjectID (classDefinition.ID, Guid.NewGuid());
    }

    public virtual string GetIDColumnTypeName ()
    {
      return "uniqueidentifier";
    }
    
    public new RdbmsProviderDefinition StorageProviderDefinition
    {
      get
      {
        // CheckDisposed is not necessary here, because StorageProvider.Definition already checks this.
        return (RdbmsProviderDefinition) base.StorageProviderDefinition;
      }
    }

    public virtual TracingDbCommand CreateDbCommand ()
    {
      CheckDisposed();

      if (!IsConnected)
        throw new InvalidOperationException ("Connect must be called before a command can be created.");

      TracingDbCommand command = _connection.CreateCommand();

      try
      {
        command.SetInnerConnection (_connection);
        command.SetInnerTransaction (_transaction);
      }
      catch (Exception)
      {
        command.Dispose();
        throw;
      }

      return command;
    }

    IDbCommand IRdbmsProviderCommandExecutionContext.CreateDbCommand ()
    {
      return CreateDbCommand();
    }

    protected internal RdbmsProviderException CreateRdbmsProviderException (string formatString, params object[] args)
    {
      return CreateRdbmsProviderException (null, formatString, args);
    }

    protected internal RdbmsProviderException CreateRdbmsProviderException (Exception innerException, string formatString, params object[] args)
    {
      return new RdbmsProviderException (string.Format (formatString, args), innerException);
    }

    protected ConcurrencyViolationException CreateConcurrencyViolationException (string formatString, params object[] args)
    {
      return CreateConcurrencyViolationException (null, formatString, args);
    }

    protected ConcurrencyViolationException CreateConcurrencyViolationException (Exception innerException, string formatString, params object[] args)
    {
      return new ConcurrencyViolationException (string.Format (formatString, args), innerException);
    }

    private void DisposeTransaction ()
    {
      if (_transaction != null)
        _transaction.Dispose();

      _transaction = null;
    }

    private void DisposeConnection ()
    {
      if (_connection != null)
        _connection.Close();

      _connection = null;
    }

    private ObjectID CheckStorageProviderID (ObjectID id, string argumentName)
    {
      if (id.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ObjectID '{1}' does not match with this StorageProvider's ID '{2}'.",
            id.StorageProviderDefinition.Name,
            id,
            StorageProviderDefinition.Name);
      }
      return id;
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageEntityDefinition.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
            StorageProviderDefinition.Name);
      }
    }

    private IEnumerable<DataContainer> CheckForDuplicates (IEnumerable<DataContainer> dataContainers, string operation)
    {
      var loadedIDs = new HashSet<ObjectID> ();
      foreach (var dataContainer in dataContainers)
      {
        if (dataContainer != null)
        {
          if (loadedIDs.Contains (dataContainer.ID))
          {
            var message = string.Format ("A {0} returned duplicates of object '{1}', which is not allowed.", operation, dataContainer.ID);
            throw new RdbmsProviderException (message);
          }
          loadedIDs.Add (dataContainer.ID);
        }

        yield return dataContainer;
      }
    }

    private IEnumerable<DataContainer> CheckForNulls (IEnumerable<DataContainer> dataContainers, string operation)
    {
      foreach (var dataContainer in dataContainers)
      {
        if (dataContainer == null)
          throw new RdbmsProviderException (string.Format ("A {0} returned a NULL ID, which is not allowed.", operation));

        yield return dataContainer;
      }
    }

    /// <summary> Gets a value converter that converts database types to .NET types according to the providers type mapping rules. </summary>
    public virtual ValueConverter CreateValueConverter ()
    {
      return new ValueConverter (StorageProviderDefinition, StorageNameProvider, TypeConversionProvider);
    }

    [Obsolete ("This method has been superseded by MultiObjectLoadCommand. Use that instead. (1.13.112)", true)]
    protected internal virtual DataContainer[] LoadDataContainers (IDbCommandBuilder commandBuilder, bool allowNulls)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method has been superseded by MultiDataContainerSaveCommand. Use that instead. (1.13.113)", true)]
    protected void Save (DbCommandBuilder commandBuilder, ObjectID id)
    {
      throw new NotImplementedException ();
    }
  }
}