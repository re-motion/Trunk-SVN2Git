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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  /// <summary>
  /// Adds security checks to the following re-store operations: object creation and deletion, queries, relation and property reads and writes.
  /// </summary>
  [Serializable]
  public class SecurityClientTransactionExtension : ClientTransactionExtensionBase
  {
    public static string DefaultKey
    {
      get { return typeof (SecurityClientTransactionExtension).FullName; }
    }

    private bool _isActive;
    [NonSerialized]
    private SecurityClient _securityClient;

    public SecurityClientTransactionExtension ()
        : this (DefaultKey)
    {
    }

    protected SecurityClientTransactionExtension (string key)
      : base (key)
    {
    }

    public override QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
    {
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      if (clientTransaction.ParentTransaction != null)
        return queryResult; // filtering already done in parent transaction

      if (_isActive)
        return queryResult;

      if (SecurityFreeSection.IsActive)
        return queryResult;

      var queryResultList = new List<T> (queryResult.AsEnumerable ());
      var securityClient = GetSecurityClient ();
      
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

    public override void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (_isActive)
        return;

      if (!(typeof (ISecurableObject).IsAssignableFrom (type)))
        return;

      if (SecurityFreeSection.IsActive)
        return;

      var securityClient = GetSecurityClient ();
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

    public override void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
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

      var securityClient = GetSecurityClient ();
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

    public override void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      PropertyReading (clientTransaction, domainObject, propertyDefinition.PropertyInfo);
    }

    public override void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      PropertyReading (clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    private void PropertyReading (ClientTransaction clientTransaction, DomainObject domainObject, IPropertyInformation propertyInfo)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      var securityClient = GetSecurityClient();
      try
      {
        _isActive = true;
        var methodInformation = propertyInfo.GetGetMethod (true) ?? new NullMethodInformation();
        clientTransaction.Execute (() => securityClient.CheckPropertyReadAccess (securableObject, methodInformation));
      }
      finally
      {
        _isActive = false;
      }
    }

    public override void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      PropertyChanging (clientTransaction, domainObject, propertyDefinition.PropertyInfo);
    }

    public override void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      PropertyChanging (clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    public override void SubTransactionInitialize (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("parentClientTransaction", parentClientTransaction);
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);

      TryInstall (subTransaction);
    }

    private void PropertyChanging (ClientTransaction clientTransaction, DomainObject domainObject, IPropertyInformation propertyInfo)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      var securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        var methodInformation = propertyInfo.GetSetMethod (true) ?? new NullMethodInformation ();
        clientTransaction.Execute (() => securityClient.CheckPropertyWriteAccess (securableObject, methodInformation));
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
  }
}
