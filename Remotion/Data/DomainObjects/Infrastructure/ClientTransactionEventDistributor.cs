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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Forwards event notifications to <see cref="IClientTransactionListener"/>, <see cref="IClientTransactionExtension"/>, 
  /// <see cref="ClientTransaction"/>, and <see cref="DomainObject"/> instances.
  /// </summary>
  [Serializable]
  public class ClientTransactionEventDistributor : CompoundClientTransactionListener, IClientTransactionEventDistributor
  {
    private readonly ClientTransactionExtensionCollection _extensions;

    public ClientTransactionEventDistributor ()
    {
      _extensions = new ClientTransactionExtensionCollection ("root");
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
    }

    public override void TransactionInitialize (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      base.TransactionInitialize (clientTransaction);
      _extensions.TransactionInitialize (clientTransaction);
    }

    public override void TransactionDiscard (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      base.TransactionDiscard (clientTransaction);
      _extensions.TransactionDiscard (clientTransaction);
    }

    public override void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      base.SubTransactionCreating (clientTransaction);
      _extensions.SubTransactionCreating (clientTransaction);
    }

    public override void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);

      base.SubTransactionInitialize (clientTransaction, subTransaction);
      _extensions.SubTransactionInitialize (clientTransaction, subTransaction);
    }

    public override void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);

      using (EnterScopeOnDemand (clientTransaction))
      {
        clientTransaction.OnSubTransactionCreated (new SubTransactionCreatedEventArgs (subTransaction));
      }

      _extensions.SubTransactionCreated (clientTransaction, subTransaction);
      base.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public override void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("type", type);

      base.NewObjectCreating (clientTransaction, type);
      _extensions.NewObjectCreating (clientTransaction, type);
    }

    public override void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      base.ObjectsLoading (clientTransaction, objectIDs);
      _extensions.ObjectsLoading (clientTransaction, objectIDs);
    }

    public override void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      using (EnterScopeOnDemand (clientTransaction))
      {
        foreach (var domainObject in domainObjects)
          domainObject.OnLoaded();

        clientTransaction.OnLoaded (new ClientTransactionEventArgs (domainObjects));
      }

      _extensions.ObjectsLoaded (clientTransaction, domainObjects);
      base.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public override void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("unloadedDomainObjects", unloadedDomainObjects);

      base.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
      _extensions.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
      using (EnterScopeOnDemand (clientTransaction))
      {
        // This is a for loop for symmetry with ObjectsUnloaded
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < unloadedDomainObjects.Count; i++)
            // ReSharper restore ForCanBeConvertedToForeach
        {
          var domainObject = unloadedDomainObjects[i];
          domainObject.OnUnloading();
        }
      }
    }

    public override void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("unloadedDomainObjects", unloadedDomainObjects);

      using (EnterScopeOnDemand (clientTransaction))
      {
        for (int i = unloadedDomainObjects.Count - 1; i >= 0; i--)
        {
          var domainObject = unloadedDomainObjects[i];
          domainObject.OnUnloaded();
        }
      }
      _extensions.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
      base.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public override void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      base.ObjectDeleting (clientTransaction, domainObject);
      _extensions.ObjectDeleting (clientTransaction, domainObject);
      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnDeleting (EventArgs.Empty);
      }
    }

    public override void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnDeleted (EventArgs.Empty);
      }
      _extensions.ObjectDeleted (clientTransaction, domainObject);
      base.ObjectDeleted (clientTransaction, domainObject);
    }

    public override void PropertyValueReading (
        ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      base.PropertyValueReading (clientTransaction, domainObject, propertyDefinition, valueAccess);
      _extensions.PropertyValueReading (clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public override void PropertyValueRead (
        ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      _extensions.PropertyValueRead (clientTransaction, domainObject, propertyDefinition, value, valueAccess);
      base.PropertyValueRead (clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public override void PropertyValueChanging (
        ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      base.PropertyValueChanging (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      _extensions.PropertyValueChanging (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnPropertyChanging (new PropertyChangeEventArgs (propertyDefinition, oldValue, newValue));
      }
    }

    public override void PropertyValueChanged (
        ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnPropertyChanged (new PropertyChangeEventArgs (propertyDefinition, oldValue, newValue));
      }

      _extensions.PropertyValueChanged (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
      base.PropertyValueChanged (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public override void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      base.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
      _extensions.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public override void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      _extensions.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
      base.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public override void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);

      _extensions.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
      base.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public override void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      base.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      _extensions.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnRelationChanging (new RelationChangingEventArgs (relationEndPointDefinition, oldRelatedObject, newRelatedObject));
      }
    }

    public override void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      using (EnterScopeOnDemand (clientTransaction))
      {
        domainObject.OnRelationChanged (new RelationChangedEventArgs (relationEndPointDefinition, oldRelatedObject, newRelatedObject));
      }
      _extensions.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
      base.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      queryResult = base.FilterQueryResult (clientTransaction, queryResult);
      queryResult = _extensions.FilterQueryResult (clientTransaction, queryResult);
      return queryResult;
    }

    public override void TransactionCommitting (
        ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull ("eventRegistrar", eventRegistrar);

      base.TransactionCommitting (clientTransaction, domainObjects, eventRegistrar);
      _extensions.Committing (clientTransaction, domainObjects, eventRegistrar);
      using (EnterScopeOnDemand (clientTransaction))

      {
        clientTransaction.OnCommitting (new ClientTransactionCommittingEventArgs (domainObjects, eventRegistrar));
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < domainObjects.Count; i++)
        {
          var domainObject = domainObjects[i];
          if (!domainObject.IsInvalid)
            domainObject.OnCommitting (new DomainObjectCommittingEventArgs (eventRegistrar));
        }
        // ReSharper restore ForCanBeConvertedToForeach
      }
    }

    public override void TransactionCommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectPersistence.PersistableData> committedData)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("committedData", committedData);

      base.TransactionCommitValidate (clientTransaction, committedData);
      _extensions.CommitValidate (clientTransaction, committedData);
    }

    public override void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      using (EnterScopeOnDemand (clientTransaction))
      {
        for (int i = domainObjects.Count - 1; i >= 0; i--)
          domainObjects[i].OnCommitted (EventArgs.Empty);
        clientTransaction.OnCommitted (new ClientTransactionEventArgs (domainObjects));
      }

      _extensions.Committed (clientTransaction, domainObjects);
      base.TransactionCommitted (clientTransaction, domainObjects);
    }

    public override void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      base.TransactionRollingBack (clientTransaction, domainObjects);
      _extensions.RollingBack (clientTransaction, domainObjects);

      using (EnterScopeOnDemand (clientTransaction))
      {
        clientTransaction.OnRollingBack (new ClientTransactionEventArgs (domainObjects));
        // ReSharper disable ForCanBeConvertedToForeach
        for (int i = 0; i < domainObjects.Count; i++)
        {
          var domainObject = domainObjects[i];
          if (!domainObject.IsInvalid)
            domainObject.OnRollingBack (EventArgs.Empty);
        }
        // ReSharper restore ForCanBeConvertedToForeach
      }
    }

    public override void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      using (EnterScopeOnDemand (clientTransaction))
      {
        for (int i = domainObjects.Count - 1; i >= 0; i--)
          domainObjects[i].OnRolledBack (EventArgs.Empty);
        clientTransaction.OnRolledBack (new ClientTransactionEventArgs (domainObjects));
      }

      _extensions.RolledBack (clientTransaction, domainObjects);
      base.TransactionRolledBack (clientTransaction, domainObjects);
    }

    private ClientTransactionScope EnterScopeOnDemand (ClientTransaction clientTransaction)
    {
      if (ClientTransaction.Current != clientTransaction)
        return clientTransaction.EnterNonDiscardingScope();
      return null;
    }
  }
}