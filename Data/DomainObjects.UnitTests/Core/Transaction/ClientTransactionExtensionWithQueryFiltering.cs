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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  [Serializable]
  public class ClientTransactionExtensionWithQueryFiltering : IClientTransactionExtension
  {
    public void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
    }

    public void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
    }

    public void ObjectLoading (ClientTransaction clientTransaction, ObjectID id)
    {
    }

    public virtual void NewObjectCreated (DomainObject newDomainObject)
    {
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, DomainObjectCollection loadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public virtual void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
    }

    public virtual void FilterQueryResult (ClientTransaction clientTransaction, DomainObjectCollection queryResult, IQuery query)
    {
      if (queryResult.Count >0)
        queryResult.Remove (queryResult[0]);
    }

    public virtual void Committing (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void Committed (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void RollingBack (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void RolledBack (ClientTransaction clientTransaction, DomainObjectCollection changedDomainObjects)
    {
    }
  }
}
