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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Holds all the <see cref="IClientTransactionListener"/> instances attached to a <see cref="ClientTransaction"/>, forwarding events to them, 
  /// and raising <see cref="DomainObject"/> and <see cref="ClientTransaction"/> events.
  /// </summary>
  [Serializable]
  public class TopClientTransactionListener : CompoundClientTransactionListener, ITopClientTransactionListener
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
      base.ObjectDeleting (clientTransaction, domainObject);
    }

    public override void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      base.ObjectDeleted (clientTransaction, domainObject);
    }

    public override void PropertyValueReading (ClientTransaction clientTransaction, DataManagement.DataContainer dataContainer, DataManagement.PropertyValue propertyValue, DataManagement.ValueAccess valueAccess)
    {
      base.PropertyValueReading (clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public override void PropertyValueRead (ClientTransaction clientTransaction, DataManagement.DataContainer dataContainer, DataManagement.PropertyValue propertyValue, object value, DataManagement.ValueAccess valueAccess)
    {
      base.PropertyValueRead (clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public override void PropertyValueChanging (ClientTransaction clientTransaction, DataManagement.DataContainer dataContainer, DataManagement.PropertyValue propertyValue, object oldValue, object newValue)
    {
      base.PropertyValueChanging (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public override void PropertyValueChanged (ClientTransaction clientTransaction, DataManagement.DataContainer dataContainer, DataManagement.PropertyValue propertyValue, object oldValue, object newValue)
    {
      base.PropertyValueChanged (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public override void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, Mapping.IRelationEndPointDefinition relationEndPointDefinition, DataManagement.ValueAccess valueAccess)
    {
      base.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public override void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, Mapping.IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, DataManagement.ValueAccess valueAccess)
    {
      base.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public override void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, Mapping.IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, DataManagement.ValueAccess valueAccess)
    {
      base.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public override void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, Mapping.IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      base.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, Mapping.IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      base.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override void TransactionCommitting (ClientTransaction clientTransaction, System.Collections.ObjectModel.ReadOnlyCollection<DomainObject> domainObjects)
    {
      base.TransactionCommitting (clientTransaction, domainObjects);
    }

    public override void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      base.TransactionCommitted (clientTransaction, domainObjects);
    }

    public override void TransactionRollingBack (ClientTransaction clientTransaction, System.Collections.ObjectModel.ReadOnlyCollection<DomainObject> domainObjects)
    {
      base.TransactionRollingBack (clientTransaction, domainObjects);
    }

    public override void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      base.TransactionRolledBack (clientTransaction, domainObjects);
    }
  }
}