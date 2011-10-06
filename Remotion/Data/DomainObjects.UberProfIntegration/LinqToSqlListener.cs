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
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements <see cref="IPersistenceListener"/> for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// <seealso cref="LinqToSqlAppenderProxy"/>
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [Serializable]
  public class LinqToSqlListener : IPersistenceListener, IClientTransactionExtension
  {
    #region Implementation of IClientTransactionExtension

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinitiont)
    {
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject
    {
      return queryResult;
    }

    public void Committing (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void Committed (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void RollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void RolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
    }


    #endregion

    #region Implementation of IPersistenceListener

    public void ConnectionOpened (Guid connectionID)
    {
    }

    public void ConnectionClosed (Guid connectionID)
    {
    }
    
    #endregion

    private readonly LinqToSqlAppenderProxy _appenderProxy;
    private readonly Guid _clientTransactionID;

    public LinqToSqlListener (Guid clientTransactionID, LinqToSqlAppenderProxy appenderProxy)
    {
      ArgumentUtility.CheckNotNull ("appenderProxy", appenderProxy);

      _clientTransactionID = clientTransactionID;
      _appenderProxy = appenderProxy;
    }

    public LinqToSqlAppenderProxy AppenderProxy
    {
      get { return _appenderProxy; }
    }

    public Guid ClientTransactionID
    {
      get { return _clientTransactionID; }
    }

    public string Key
    {
      get { return typeof (LinqToSqlListener).FullName; }
    }

    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      _appenderProxy.ConnectionStarted (_clientTransactionID);
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      _appenderProxy.ConnectionDisposed (_clientTransactionID);
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
      _appenderProxy.StatementRowCount (_clientTransactionID, queryID, rowCount);
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
      _appenderProxy.StatementError (_clientTransactionID, e);
    }

    public void QueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
      _appenderProxy.CommandDurationAndRowCount (_clientTransactionID, durationOfQueryExecution.Milliseconds, null);
    }

    public void QueryExecuting (
        Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      _appenderProxy.StatementExecuted (_clientTransactionID, queryID, AppendParametersToCommandText (queryID, commandText, parameters));
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
      _appenderProxy.TransactionBegan (_clientTransactionID, isolationLevel);
    }

    public void TransactionCommitted (Guid connectionID)
    {
      _appenderProxy.TransactionCommit (_clientTransactionID);
    }

    public void TransactionDisposed (Guid connectionID)
    {
      _appenderProxy.TransactionDisposed (_clientTransactionID);
    }

    public void TransactionRolledBack (Guid connectionID)
    {
      _appenderProxy.TransactionRolledBack (_clientTransactionID);
    }

    private string AppendParametersToCommandText (Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append ("-- Statement ").Append (queryID).AppendLine();
      builder.AppendLine (commandText);
      builder.AppendLine ("-- Ignore unbounded result sets: TOP *"); // Format with space and asterisk is important to trigger RexEx in profiler.
      builder.AppendLine ("-- Parameters:");
      foreach (string key in parameters.Keys)
      {
        string parameterName = key;
        if (!parameterName.StartsWith ("@"))
          parameterName = "@" + parameterName;
        object value = parameters[key];

        builder.Append ("-- ");
        builder.Append (parameterName);
        builder.Append (" = [-[");
        builder.Append (value);
        builder.Append ("]-] [-[");
        builder.Append ("Type"); //parameter.DbType
        builder.Append (" (");
        builder.Append (0); // parameter.Size
        builder.AppendLine (")]-]");
      }

      return builder.ToString();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}