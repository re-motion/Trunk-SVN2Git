// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface allowing clients to raise events for the associated <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IClientTransactionEventSink
  {
    void RaiseEvent (Action<ClientTransaction, IClientTransactionListener> action);

    void RaiseRelationChangingEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject);

    void RaiseRelationChangedEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject);

    void RaiseObjectDeletingEvent (DomainObject domainObject);

    void RaiseObjectDeletedEvent (DomainObject domainObject);

    void RaiseObjectsUnloadingEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects);

    void RaiseObjectsUnloadedEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects);

    void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID);

    void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint);

    void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID);

    void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState);

    void RaisePropertyValueReadingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess);

    void RaisePropertyValueReadEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess);

    void RaisePropertyValueChangingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue);

    void RaisePropertyValueChangedEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue);

    void RaiseDataContainerStateUpdatedEvent (DataContainer container, StateType newDataContainerState);

    void RaiseDataContainerMapRegisteringEvent (DataContainer container);

    void RaiseDataContainerMapUnregisteringEvent (DataContainer container);

    void RaiseSubTransactionCreatingEvent ();

    void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction);

    void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction);

    void RaiseObjectMarkedInvalidEvent (DomainObject domainObject);

    void RaiseObjectMarkedNotInvalidEvent (DomainObject domainObject);

    void RaiseNewObjectCreatingEvent (Type type);

    void RaiseObjectsLoadingEvent (ReadOnlyCollection<ObjectID> objectIDs);

    void RaiseObjectsLoadedEvent (ReadOnlyCollection<DomainObject> domainObjects);

    void RaiseObjectsNotFoundEvent (ReadOnlyCollection<ObjectID> objectIDs);

    void RaiseTransactionCommittingEvent (
        ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar);

    void RaiseTransactionCommitValidateEvent (ReadOnlyCollection<PersistableData> committedData);

    void RaiseTransactionCommittedEvent (ReadOnlyCollection<DomainObject> domainObjects);

    void RaiseTransactionRollingBackEvent (ReadOnlyCollection<DomainObject> domainObjects);

    void RaiseTransactionRolledBackEvent (ReadOnlyCollection<DomainObject> domainObjects);

    QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject;

    IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results);
  }
}