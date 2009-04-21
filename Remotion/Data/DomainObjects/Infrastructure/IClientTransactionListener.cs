// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

    /// <summary>
    /// Indicates a new <see cref="DomainObject"/> instance is being created. This event is called while the <see cref="DomainObject"/> base 
    /// constructor is executing before the subclass constructors have run and before the object has got its <see cref="ObjectID"/> or 
    /// <see cref="DataContainer"/>. If this method throws an exception, the object construction will be canceled and no side effects will remain.
    /// </summary>
    void NewObjectCreating (Type type, DomainObject instance);

    void ObjectLoading (ObjectID id);
    void ObjectsLoaded (DomainObjectCollection domainObjects);

    void ObjectGotID (DomainObject instance, ObjectID id);

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

    QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T: DomainObject;

    void TransactionCommitting (DomainObjectCollection domainObjects);
    void TransactionCommitted (DomainObjectCollection domainObjects);
    void TransactionRollingBack (DomainObjectCollection domainObjects);
    void TransactionRolledBack (DomainObjectCollection domainObjects);

    void RelationEndPointMapRegistering (RelationEndPoint endPoint);
    void RelationEndPointMapUnregistering (RelationEndPointID endPointID);
    void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs);
    void RelationEndPointMapCopyingFrom (RelationEndPointMap source);
    void RelationEndPointMapCopyingTo (RelationEndPointMap destination);

    void DataManagerMarkingObjectDiscarded (ObjectID id);
    void DataManagerCopyingFrom (DataManager source);
    void DataManagerCopyingTo (DataManager destination);

    void DataContainerMapRegistering (DataContainer container);
    void DataContainerMapUnregistering (DataContainer container);
    void DataContainerMapCopyingFrom (DataContainerMap source);
    void DataContainerMapCopyingTo (DataContainerMap destination);
  }
}
