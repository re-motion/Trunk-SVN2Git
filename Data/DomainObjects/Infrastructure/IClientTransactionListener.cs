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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface for objects listening for events occuring in the scope of a ClientTransaction.
  /// </summary>
  /// <remarks>
  /// This is similar to <see cref="IClientTransactionExtension"/>, but where <see cref="IClientTransactionExtension"/> is for the public,
  /// <see cref="IClientTransactionListener"/> is for internal usage (and therefore provides more events).
  /// </remarks>
  public interface IClientTransactionListener
  {
    void SubTransactionCreating ();
    void SubTransactionCreated (ClientTransaction subTransaction);

    void NewObjectCreating (Type type, DomainObject instance);

    void ObjectLoading (ObjectID id);
    void ObjectsLoaded (DomainObjectCollection domainObjects);

    void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance);

    void ObjectDeleting (DomainObject domainObject);
    void ObjectDeleted (DomainObject domainObject);

    void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess);
    void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess);
    void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);
    void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);

    void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess);
    void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess);
    void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess);
    
    void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject);
    void RelationChanged (DomainObject domainObject, string propertyName);

    void FilterQueryResult (DomainObjectCollection queryResult, IQuery query);

    void TransactionCommitting (DomainObjectCollection domainObjects);
    void TransactionCommitted (DomainObjectCollection domainObjects);
    void TransactionRollingBack (DomainObjectCollection domainObjects);
    void TransactionRolledBack (DomainObjectCollection domainObjects);

    void RelationEndPointMapRegistering (RelationEndPoint endPoint);
    void RelationEndPointMapUnregistering (RelationEndPointID endPointID);
    void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs);
    void RelationEndPointMapCopyingFrom (RelationEndPointMap source);
    void RelationEndPointMapCopyingTo (RelationEndPointMap source);

    void DataManagerMarkingObjectDiscarded (ObjectID id);
    void DataManagerCopyingFrom (DataManager source);
    void DataManagerCopyingTo (DataManager destination);

    void DataContainerMapRegistering (DataContainer container);
    void DataContainerMapUnregistering (DataContainer container);
    void DataContainerMapCopyingFrom (DataContainerMap source);
    void DataContainerMapCopyingTo (DataContainerMap destination);
  }
}
