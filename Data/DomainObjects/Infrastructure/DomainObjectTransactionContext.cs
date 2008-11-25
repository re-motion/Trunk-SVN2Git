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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides the default implementation of the <see cref="IDomainObjectTransactionContext"/> interface.
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public class DomainObjectTransactionContext : IDomainObjectTransactionContext
  {
    private readonly DomainObject _domainObject;
    private readonly ClientTransaction _associatedTransaction;

    public DomainObjectTransactionContext (DomainObject domainObject, ClientTransaction associatedTransaction)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("associatedTransaction", associatedTransaction);

      _domainObject = domainObject;
      _associatedTransaction = associatedTransaction;
    }

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public ClientTransaction AssociatedTransaction
    {
      get { return _associatedTransaction; }
    }

    public bool CanBeUsedInTransaction
    {
      get
      {
        if (AssociatedTransaction.IsEnlisted (DomainObject))
          return true;
        else if (ClientTransactionScope.ActiveScope != null && ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects)
        {
          AssociatedTransaction.EnlistDomainObject (DomainObject);
          return true;
        }
        else
          return false;
      }
    }

    public StateType State
    {
      get
      {
        DomainObjectUtility.CheckIfRightTransaction (DomainObject, AssociatedTransaction);

        if (IsDiscarded)
          return StateType.Discarded;
        else
        {
          DataContainer dataContainer = AssociatedTransaction.GetDataContainer(DomainObject);
          if (dataContainer.State == StateType.Unchanged)
          {
            if (AssociatedTransaction.HasRelationChanged (DomainObject))
              return StateType.Changed;
            else
              return StateType.Unchanged;
          }

          return dataContainer.State;
        }
      }
    }

    public bool IsDiscarded
    {
      get
      {
        DomainObjectUtility.CheckIfRightTransaction (DomainObject, AssociatedTransaction);
        return AssociatedTransaction.DataManager.IsDiscarded (DomainObject.ID); 
      }
    }

    public object Timestamp
    {
      get { return AssociatedTransaction.GetDataContainer (DomainObject).Timestamp; }
    }

    public void MarkAsChanged()
    {
      DomainObjectUtility.CheckIfObjectIsDiscarded (DomainObject, AssociatedTransaction);

      DataContainer dataContainer = AssociatedTransaction.GetDataContainer(DomainObject);
      try
      {
        dataContainer.MarkAsChanged ();
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException ("Only existing DomainObjects can be marked as changed.", ex);
      }
    }
  }
}