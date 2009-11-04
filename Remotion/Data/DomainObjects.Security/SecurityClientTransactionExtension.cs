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
using Remotion.Data.DomainObjects.DataManagement;
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

    #region IClientTransactionExtension Implementation

    void IClientTransactionExtension.ObjectLoading (ClientTransaction clientTransaction, ObjectID id)
    {
    }


    void IClientTransactionExtension.ObjectsLoaded (ClientTransaction clientTransaction, DomainObjectCollection loadedDomainObjects)
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

    void IClientTransactionExtension.RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
    }

    void IClientTransactionExtension.Committing (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.Committed (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RollingBack (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RolledBack (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    #endregion

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      if (_isActive)
        return queryResult;

      if (SecurityFreeSection.IsActive)
        return queryResult;

      var queryResultList = new List<T> (queryResult.AsEnumerable());

      SecurityClient securityClient = GetSecurityClient ();

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

      if (queryResultList.Count != queryResult.Count)
        return new QueryResult<T> (queryResult.Query, queryResultList.ToArray ());
      else
        return queryResult;
    }

    public void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
    }

    public void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
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
        securityClient.CheckConstructorAccess (type);
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

      if (domainObject.State == StateType.New)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckAccess (securableObject, AccessType.Get (GeneralAccessTypes.Delete));
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

      PropertyReading (dataContainer.DomainObject, propertyValue.Name);
    }

    public virtual void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      PropertyReading (domainObject, propertyName);
    }

    private void PropertyReading (DomainObject domainObject, string propertyName)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckPropertyReadAccess (securableObject, GetSimplePropertyName (propertyName));
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

      PropertyChanging (dataContainer.DomainObject, propertyValue.Name);
    }

    public virtual void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      PropertyChanging (domainObject, propertyName);
    }

    private void PropertyChanging (DomainObject domainObject, string propertyName)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckPropertyWriteAccess (securableObject, GetSimplePropertyName (propertyName));
      }
      finally
      {
        _isActive = false;
      }
    }

    //TODO: Move to reflection Utility and test
    private string GetSimplePropertyName (string propertyName)
    {
      int lastIndex = propertyName.LastIndexOf ('.');
      if (lastIndex != -1 && lastIndex + 1 < propertyName.Length)
        return propertyName.Substring (lastIndex + 1);
      return propertyName;
    }

    private SecurityClient GetSecurityClient ()
    {
      if (_securityClient == null)
        _securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return _securityClient;
    }
  }
}
