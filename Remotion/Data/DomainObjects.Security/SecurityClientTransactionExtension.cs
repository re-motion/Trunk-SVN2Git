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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  [Serializable]
  public class SecurityClientTransactionExtension : IClientTransactionExtension
  {
    // types

    // static members

    // member fields

    private bool _isActive;
    [NonSerialized]
    private SecurityClient _securityClient;

    // construction and disposing

    // methods and properties

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      if (clientTransaction.ParentTransaction != null)
        return queryResult; // filtering already done in parent transaction

      if (_isActive)
        return queryResult;

      if (SecurityFreeSection.IsActive)
        return queryResult;

      var queryResultList = new List<T> (queryResult.AsEnumerable ());
      SecurityClient securityClient = GetSecurityClient ();
      
      clientTransaction.Execute (() =>
      {
        for (int i = queryResultList.Count - 1; i >= 0; i--)
        {
          var securableObject = queryResultList[i] as ISecurableObject;
          if (securableObject == null)
            continue;

          bool hasAccess;
          try
          {
            _isActive = true;
            hasAccess = securityClient.HasAccess (securableObject, AccessType.Get (GeneralAccessTypes.Find));
          }
          finally
          {
            _isActive = false;
          }
          if (!hasAccess)
            queryResultList.RemoveAt (i);
        }
      });

      if (queryResultList.Count != queryResult.Count)
        return new QueryResult<T> (queryResult.Query, queryResultList.ToArray ());
      else
        return queryResult;
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (_isActive)
        return;

      if (!(typeof (ISecurableObject).IsAssignableFrom (type)))
        return;

      if (SecurityFreeSection.IsActive)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        clientTransaction.Execute (() => securityClient.CheckConstructorAccess (type));
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      if (domainObject.TransactionContext[clientTransaction].State == StateType.New)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        clientTransaction.Execute (() => securityClient.CheckAccess (securableObject, AccessType.Get (GeneralAccessTypes.Delete)));
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      if (!propertyValue.Definition.IsPropertyInfoResolved)
      {
        var message = "The propertyValue does not contain a resolved property information. Security checks are only supported if the mapping can provide the IPropertyInformation object associated with the propertyValue";
        throw new InvalidOperationException (message);
      }

      PropertyReading (clientTransaction, dataContainer.DomainObject, propertyValue.Definition.PropertyInfo);
    }

    public virtual void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (!relationEndPointDefinition.IsPropertyInfoResolved)
      {
        var message = "The relationEndPointDefinition does not contain a resolved property information. Security checks are only supported if the mapping can provide the IPropertyInformation object associated with the relationEndPointDefinition";
        throw new InvalidOperationException (message);
      }

      PropertyReading (clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    private void PropertyReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyInfo propertyInfo)
    {
      if (_isActive)
        return;
      
      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        clientTransaction.Execute (() => securityClient.CheckPropertyReadAccess (securableObject, propertyInfo));
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      if (!propertyValue.Definition.IsPropertyInfoResolved)
      {
        var message = "The propertyValue does not contain a resolved property information. Security checks are only supported if the mapping can provide the IPropertyInformation object associated with the propertyValue";
        throw new InvalidOperationException (message);
      }

      PropertyChanging (clientTransaction, dataContainer.DomainObject, propertyValue.Definition.PropertyInfo);
    }

    public virtual void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (!relationEndPointDefinition.IsPropertyInfoResolved)
      {
        var message = "The relationEndPointDefinition does not contain a resolved property information. Security checks are only supported if the mapping can provide the IPropertyInformation object associated with the relationEndPointDefinition";
        throw new InvalidOperationException (message);
      }

      PropertyChanging (clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    private void PropertyChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyInfo propertyInfo)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        clientTransaction.Execute (() => securityClient.CheckPropertyWriteAccess (securableObject, propertyInfo));
      }
      finally
      {
        _isActive = false;
      }
    }

    private SecurityClient GetSecurityClient ()
    {
      if (_securityClient == null)
        _securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return _securityClient;
    }

    #region IClientTransactionExtension Implementation

    void IClientTransactionExtension.SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
    }

    void IClientTransactionExtension.SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
    }

    void IClientTransactionExtension.ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
    }


    void IClientTransactionExtension.ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> loadedDomainObjects)
    {
    }

    void IClientTransactionExtension.ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    void IClientTransactionExtension.ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    void IClientTransactionExtension.ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    void IClientTransactionExtension.PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    void IClientTransactionExtension.RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition)
    {
    }

    void IClientTransactionExtension.Committing (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    void IClientTransactionExtension.Committed (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    #endregion

  }
}
