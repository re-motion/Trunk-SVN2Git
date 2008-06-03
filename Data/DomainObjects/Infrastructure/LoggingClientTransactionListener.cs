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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A listener implementation logging all transaction events.
  /// </summary>
  [Serializable]
  public class LoggingClientTransactionListener : IClientTransactionListener
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

    public void SubTransactionCreating ()
    {
      s_log.Info ("SubTransactionCreating");
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      s_log.Info ("SubTransactionCreated");
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      s_log.Info ("NewObjectCreating");
    }

    public void ObjectLoading (ObjectID id)
    {
      s_log.Info ("ObjectLoading");
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      s_log.Info ("ObjectInitializedFromDataContainer");
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      s_log.Info ("ObjectsLoaded");
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      s_log.Info ("ObjectDeleting");
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      s_log.Info ("ObjectDeleted");
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      s_log.Info ("PropertyValueReading");
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      s_log.Info ("PropertyValueRead");
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.Info ("PropertyValueChanging");
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.Info ("PropertyValueChanged");
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      s_log.Info ("RelationReading");
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      s_log.Info ("RelationRead");
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      s_log.Info ("RelationRead");
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      s_log.Info ("RelationChanging");
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      s_log.Info ("RelationChanged");
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      s_log.Info ("FilterQueryResult");
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionCommitting");
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionCommitted");
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionRollingBack");
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionRolledBack");
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      s_log.Info ("RelationEndPointMapRegistering");
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      s_log.Info ("RelationEndPointMapUnregistering");
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      s_log.Info ("RelationEndPointMapPerformingDelete");
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      s_log.Info ("RelationEndPointMapCopyingFrom");
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
      s_log.Info ("RelationEndPointMapCopyingTo");
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      s_log.Info ("DataManagerMarkingObjectDiscarded");
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      s_log.Info ("DataManagerCopyingFrom");
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      s_log.Info ("DataManagerCopyingTo");
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      s_log.Info ("DataContainerMapRegistering");
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      s_log.Info ("DataContainerMapUnregistering");
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      s_log.Info ("DataContainerMapCopyingFrom");
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      s_log.Info ("DataContainerMapCopyingTo");
    }
  }
}
