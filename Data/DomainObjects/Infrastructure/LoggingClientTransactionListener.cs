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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Text;

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
      s_log.Debug ("SubTransactionCreating");
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      s_log.Debug ("SubTransactionCreated");
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      s_log.DebugFormat ("NewObjectCreating: {0}", type.FullName);
    }

    public void ObjectLoading (ObjectID id)
    {
      s_log.DebugFormat ("ObjectLoading: {0}", id);
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      s_log.DebugFormat ("ObjectInitializedFromDataContainer: {0}", id);
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      s_log.DebugFormat ("ObjectsLoaded: {0}", GetDomainObjectsString(domainObjects));
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      s_log.DebugFormat ("ObjectDeleting: {0}", GetDomainObjectString (domainObject));
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      s_log.DebugFormat ("ObjectDeleted: {0}", GetDomainObjectString (domainObject));
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      s_log.DebugFormat ("PropertyValueReading: {0} ({1}, {2})", propertyValue.Name, valueAccess, dataContainer.ID);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      s_log.DebugFormat ("PropertyValueRead: {0}=={1} ({2}, {3})", propertyValue.Name, value ?? "<null>", valueAccess, dataContainer.ID);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.DebugFormat ("PropertyValueChanging: {0} {1}->{2} ({3})", propertyValue.Name, oldValue ?? "<null>", newValue ?? "<null>", dataContainer.ID);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.DebugFormat ("PropertyValueChanged: {0} {1}->{2} ({3})", propertyValue.Name, oldValue ?? "<null>", newValue ?? "<null>", dataContainer.ID);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      s_log.DebugFormat ("RelationReading: {0} ({1}, {2})", propertyName, valueAccess, GetDomainObjectString (domainObject));
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      s_log.DebugFormat ("RelationRead: {0}=={1} ({2}, {3})", propertyName, GetDomainObjectString (relatedObject), valueAccess, GetDomainObjectString (domainObject));
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      s_log.DebugFormat ("RelationRead: {0} ({1}, {2}): {3}", propertyName, valueAccess, domainObject.ID, GetDomainObjectsString (relatedObjects));
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      s_log.DebugFormat ("RelationChanging: {0}: {1}->{2} /{3}", propertyName, 
          GetDomainObjectString (oldRelatedObject), GetDomainObjectString (newRelatedObject), GetDomainObjectString (domainObject));
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      s_log.DebugFormat ("RelationChanged: {0} ({1})", propertyName, GetDomainObjectString (domainObject));
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      s_log.DebugFormat ("FilterQueryResult: {0}: {1} ({2})", query.ID, GetDomainObjectsString (queryResult), query.Statement);
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      s_log.DebugFormat ("TransactionCommitting: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      s_log.DebugFormat ("TransactionCommitted: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      s_log.DebugFormat ("TransactionRollingBack: {0}", GetDomainObjectsString (domainObjects));
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      s_log.DebugFormat ("TransactionRolledBack: {0}", GetDomainObjectsString (domainObjects));
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      s_log.DebugFormat ("RelationEndPointMapRegistering: {0}", endPoint.ID);
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      s_log.DebugFormat ("RelationEndPointMapUnregistering: {0}", endPointID);
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      s_log.DebugFormat("RelationEndPointMapPerformingDelete: {0}", SeparatedStringBuilder.Build (", ", endPointIDs));
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      s_log.DebugFormat ("RelationEndPointMapCopyingFrom: {0} relations", source.Count);
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap destination)
    {
      s_log.DebugFormat ("RelationEndPointMapCopyingTo");
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      s_log.DebugFormat ("DataManagerMarkingObjectDiscarded: {0}", id);
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      s_log.DebugFormat ("DataManagerCopyingFrom");
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      s_log.Debug ("DataManagerCopyingTo");
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      s_log.DebugFormat ("DataContainerMapRegistering: {0}", container.ID);
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      s_log.DebugFormat ("DataContainerMapUnregistering: {0}", container.ID);
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      s_log.DebugFormat ("DataContainerMapCopyingFrom: {0} objects", source.Count);
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      s_log.DebugFormat ("DataContainerMapCopyingTo");
    }

    private string GetDomainObjectsString (DomainObjectCollection domainObjects)
    {
      return SeparatedStringBuilder.Build (", ", domainObjects,
        delegate (DomainObject domainObject) { return GetDomainObjectString (domainObject); });
    }

    private string GetDomainObjectString (DomainObject domainObject)
    {
      return domainObject != null ? domainObject.ID.ToString () : "<null>";
    }
  }
}
