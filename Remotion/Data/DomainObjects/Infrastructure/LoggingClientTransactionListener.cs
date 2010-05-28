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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Text;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A listener implementation logging all transaction events.
  /// </summary>
  [Serializable]
  public class LoggingClientTransactionListener : IClientTransactionListener
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

    public void TransactionInitializing (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionInitializing", clientTransaction.ID);
    }

    public void TransactionDiscarding (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionDiscarding", clientTransaction.ID);
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} SubTransactionCreating", clientTransaction.ID);
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} SubTransactionCreated: {1}", clientTransaction.ID, subTransaction.ID);
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} NewObjectCreating: {1}", clientTransaction.ID, type.FullName);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectsLoading: {1}", clientTransaction.ID, GetObjectIDString (objectIDs));
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectsUnloaded: {1}", clientTransaction.ID, GetDomainObjectsString (unloadedDomainObjects));
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectsLoaded: {1}", clientTransaction.ID, GetDomainObjectsString (domainObjects));
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectsUnloading: {1}", clientTransaction.ID, GetDomainObjectsString (unloadedDomainObjects));
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectDeleting: {1}", clientTransaction.ID, GetDomainObjectString (domainObject));
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} ObjectDeleted: {1}", clientTransaction.ID, GetDomainObjectString (domainObject));
    }

    public void PropertyValueReading (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} PropertyValueReading: {1} ({2}, {3})",
            clientTransaction.ID,
            propertyValue.Name,
            valueAccess,
            dataContainer.ID);
      }
    }

    public void PropertyValueRead (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object value,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} PropertyValueRead: {1}=={2} ({3}, {4})",
            clientTransaction.ID,
            propertyValue.Name,
            value ?? "<null>",
            valueAccess,
            dataContainer.ID);
      }
    }

    public void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} PropertyValueChanging: {1} {2}->{3} ({4})",
            clientTransaction.ID,
            propertyValue.Name,
            oldValue ?? "<null>",
            newValue ?? "<null>",
            dataContainer.ID);
      }
    }

    public void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} PropertyValueChanged: {1} {2}->{3} ({4})",
            clientTransaction.ID,
            propertyValue.Name,
            oldValue ?? "<null>",
            newValue ?? "<null>",
            dataContainer.ID);
      }
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} RelationReading: {1} ({2}, {3})",
            clientTransaction.ID,
            propertyName,
            valueAccess,
            GetDomainObjectString (domainObject));
      }
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        DomainObject relatedObject,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} RelationRead: {1}=={2} ({3}, {4})",
            clientTransaction.ID,
            propertyName,
            GetDomainObjectString (relatedObject),
            valueAccess,
            GetDomainObjectString (domainObject));
      }
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled)
      {
        var domainObjectsString = relatedObjects.IsDataAvailable ? GetDomainObjectsString (relatedObjects) : "<data not loaded>";
        s_log.DebugFormat (
            "{0} RelationRead: {1} ({2}, {3}): {4}",
            clientTransaction.ID,
            propertyName,
            valueAccess,
            domainObject.ID,
            domainObjectsString);
      }
    }

    public void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} RelationChanging: {1}: {2}->{3} /{4}",
            clientTransaction.ID,
            propertyName,
            GetDomainObjectString (oldRelatedObject),
            GetDomainObjectString (newRelatedObject),
            GetDomainObjectString (domainObject));
      }
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} RelationChanged: {1} ({2})", clientTransaction.ID, propertyName, GetDomainObjectString (domainObject));
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject
    {
      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat (
            "{0} FilterQueryResult: {1}: {2} ({3})",
            clientTransaction.ID,
            queryResult.Query.ID,
            GetDomainObjectsString (queryResult.AsEnumerable().Cast<DomainObject>()),
            queryResult.Query.Statement);
      }
      return queryResult;
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionCommitting: {1}", clientTransaction.ID, GetDomainObjectsString (domainObjects));
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionCommitted: {1}", clientTransaction.ID, GetDomainObjectsString (domainObjects));
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionRollingBack: {1}", clientTransaction.ID, GetDomainObjectsString (domainObjects));
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} TransactionRolledBack: {1}", clientTransaction.ID, GetDomainObjectsString (domainObjects));
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} RelationEndPointMapRegistering: {1}", clientTransaction.ID, endPoint.ID);
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} RelationEndPointMapUnregistering: {1}", clientTransaction.ID, endPointID);
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} RelationEndPointUnloading: {1}", clientTransaction.ID, endPoint.ID);
    }

    public void DataManagerMarkingObjectInvalid (ClientTransaction clientTransaction, ObjectID id)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} DataManagerMarkingObjectInvalid: {1}", clientTransaction.ID, id);
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} DataContainerMapRegistering: {1}", clientTransaction.ID, container.ID);
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} DataContainerMapUnregistering: {1}", clientTransaction.ID, container.ID);
    }

    public void DataContainerStateChanging (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} DataContainerStateChanging: {1} {2}", clientTransaction.ID, container.ID, newDataContainerState);
    }

    public void RelationEndPointStateChanging (ClientTransaction clientTransaction, RelationEndPoint endPoint, bool newChangeState)
    {
      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("{0} RelationEndPointStateChanging: {1} {2}", clientTransaction.ID, endPoint.ID, newChangeState);
    }

    private string GetObjectIDString (IEnumerable<ObjectID> objectIDs)
    {
      return SeparatedStringBuilder.Build (", ", objectIDs);
    }

    private string GetDomainObjectsString (IEnumerable<DomainObject> domainObjects)
    {
      return SeparatedStringBuilder.Build (", ", domainObjects.Select (obj => GetDomainObjectString (obj)));
    }

    private string GetDomainObjectString (DomainObject domainObject)
    {
      return domainObject != null ? domainObject.ID.ToString () : "<null>";
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
