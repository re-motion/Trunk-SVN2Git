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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Forwards event notifications to <see cref="IClientTransactionListener"/>, <see cref="ClientTransaction"/>, and <see cref="DomainObject"/> 
  /// instances.
  /// </summary>
  [Serializable]
  public class ClientTransactionEventDistributor : CompoundClientTransactionListener, IClientTransactionEventDistributor
  {
    public override void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);

      clientTransaction.Execute (() => clientTransaction.OnSubTransactionCreated (new SubTransactionCreatedEventArgs (subTransaction)));
      base.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public override void ObjectsLoaded (ClientTransaction clientTransaction, System.Collections.ObjectModel.ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      clientTransaction.Execute (
          () =>
          {
            foreach (var domainObject in domainObjects)
              domainObject.OnLoaded();

            clientTransaction.OnLoaded (new ClientTransactionEventArgs (domainObjects));
          });

      base.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public override void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("unloadedDomainObjects", unloadedDomainObjects);

      base.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
      clientTransaction.Execute (
          () =>
          {
            // This is a for loop for symmetry with ObjectsUnloaded
            // ReSharper disable ForCanBeConvertedToForeach
            for (int i = 0; i < unloadedDomainObjects.Count; i++)
            // ReSharper restore ForCanBeConvertedToForeach
            {
              var domainObject = unloadedDomainObjects[i];
              domainObject.OnUnloading();
            }
          });
    }

    public override void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("unloadedDomainObjects", unloadedDomainObjects);

      clientTransaction.Execute (
         () =>
         {
           for (int i = unloadedDomainObjects.Count - 1; i >= 0; i--)
           {
             var domainObject = unloadedDomainObjects[i];
             domainObject.OnUnloaded();
           }
         });
      base.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public override void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      base.ObjectDeleting (clientTransaction, domainObject);
      clientTransaction.Execute (() => domainObject.OnDeleting (EventArgs.Empty));
    }

    public override void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      clientTransaction.Execute (() => domainObject.OnDeleted (EventArgs.Empty));
      base.ObjectDeleted (clientTransaction, domainObject);
    }

    public override void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      base.PropertyValueChanging (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);

      clientTransaction.Execute (() => domainObject.OnPropertyChanging (new PropertyChangeEventArgs (propertyDefinition, oldValue, newValue)));
    }

    public override void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      clientTransaction.Execute (() => domainObject.OnPropertyChanged (new PropertyChangeEventArgs (propertyDefinition, oldValue, newValue)));
      
      base.PropertyValueChanged (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
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
      clientTransaction.Execute (
          () => domainObject.OnRelationChanging (new RelationChangingEventArgs (relationEndPointDefinition, oldRelatedObject, newRelatedObject)));
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

      clientTransaction.Execute (
            () => domainObject.OnRelationChanged (new RelationChangedEventArgs (relationEndPointDefinition, oldRelatedObject, newRelatedObject)));
      base.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override void TransactionCommitting (
        ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull ("eventRegistrar", eventRegistrar);

      base.TransactionCommitting (clientTransaction, domainObjects, eventRegistrar);
      clientTransaction.Execute (
          () =>
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
          });
    }

    public override void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      clientTransaction.Execute (
          () =>
          {
            for (int i = domainObjects.Count - 1; i >= 0; i--)
              domainObjects[i].OnCommitted (EventArgs.Empty);
            clientTransaction.OnCommitted (new ClientTransactionEventArgs (domainObjects));
          });
      
      base.TransactionCommitted (clientTransaction, domainObjects);
    }

    public override void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      base.TransactionRollingBack (clientTransaction, domainObjects);
      clientTransaction.Execute (
          () =>
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
          });
    }

    public override void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      clientTransaction.Execute (
          () =>
          {
            for (int i = domainObjects.Count - 1; i >= 0; i--)
              domainObjects[i].OnRolledBack (EventArgs.Empty);
            clientTransaction.OnRolledBack (new ClientTransactionEventArgs (domainObjects));
          });

      base.TransactionRolledBack (clientTransaction, domainObjects);
    }
  }
}