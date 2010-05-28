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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface for objects listening for events occuring in the scope of a ClientTransaction.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is similar to <see cref="IClientTransactionExtension"/>, but where <see cref="IClientTransactionExtension"/> is for the public,
  /// <see cref="IClientTransactionListener"/> is for internal usage (and therefore provides more events).
  /// </para>
  /// <para>
  /// The <see cref="ClientTransaction.Current"/> property is not guaranteed to be set to the affected <see cref="ClientTransaction"/> when 
  /// a notification method is executed. Implementations that require access to the calling transaction must have the transaction passed to them via
  /// the constructors.
  /// </para>
  /// </remarks>
  public interface IClientTransactionListener : INullObject
  {
    void TransactionInitializing (ClientTransaction clientTransaction);
    void TransactionDiscarding (ClientTransaction clientTransaction);

    void SubTransactionCreating (ClientTransaction clientTransaction);
    void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction);


    /// <summary>
    /// Indicates a new <see cref="DomainObject"/> instance is being created. This event is called while the <see cref="DomainObject"/> base 
    /// constructor is executing before the subclass constructors have run and before the object has got its <see cref="ObjectID"/> or 
    /// <see cref="DataContainer"/>. If this method throws an exception, the object construction will be canceled and no side effects will remain.
    /// </summary>
    void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance);

    void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs);
    void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects);

    void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects);
    void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects);

    void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject);
    void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject);

    void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess);
    void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess);
    void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);
    void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);

    void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess);
    void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess);

    /// <summary>
    /// Indicates that a relation has been read.
    /// </summary>
    /// <param name="clientTransaction"></param>
    /// <param name="domainObject">The domain object owning the relation that has been read.</param>
    /// <param name="propertyName">The name of the property that has been read.</param>
    /// <param name="relatedObjects">
    ///   A read-only wrapper of the related object data that is returned to the reader. Implementors should check the 
    ///   <see cref="ReadOnlyDomainObjectCollectionAdapter{T}.IsDataAvailable"/> property before accessing the collection data in order to avoid reloading 
    ///   an unloaded collection end-point.
    /// </param>
    /// <param name="valueAccess">An indicator whether the current or original values have been read.</param>
    void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess);

    /// <summary>
    /// Indicates that a relation is about to change. 
    /// This method might be invoked more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, the method is invoked once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    /// <param name="clientTransaction"></param>
    /// <param name="domainObject">The domain object holding the relation being changed.</param>
    /// <param name="propertyName">The name of the property that changes.</param>
    /// <param name="oldRelatedObject">The related object that is removed from the relation, or <see langword="null" /> if a new item is added without 
    ///   replacing an old one.</param>
    /// <param name="newRelatedObject">The related object that is added to the relation, or <see langword="null" /> if an old item is removed without 
    ///   being replaced by a new one.</param>
    void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject);

    /// <summary>
    /// Indicates that a relation has been changed. 
    /// This method might be invoked more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, the method is invoked once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    /// <param name="clientTransaction"></param>
    /// <param name="domainObject">The domain object holding the relation being changed.</param>
    /// <param name="propertyName">The name of the property that changes.</param>
    void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName);

    QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject;

    void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects);
    void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects);
    void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects);
    void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects);

    void RelationEndPointMapRegistering (ClientTransaction clientTransaction, RelationEndPoint endPoint);
    void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID);
    void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPoint endPoint);

    void DataManagerMarkingObjectInvalid (ClientTransaction clientTransaction, ObjectID id);

    void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container);
    void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container);

    void DataContainerStateChanging (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState);
    void RelationEndPointStateChanging (ClientTransaction clientTransaction, RelationEndPoint endPoint, bool newChangeState);
  }
}
