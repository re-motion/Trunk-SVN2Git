/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

    // TODO refactoring: Move to utility class.
    public void CheckIfRightTransaction ()
    {
      if (!CanBeUsedInTransaction)
      {
        string message = string.Format (
            "Domain object '{0}' cannot be used in the given transaction as it was loaded or created in another "
            + "transaction. Enter a scope for the transaction, or call EnlistInTransaction to enlist the object "
            + "in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
            DomainObject.ID);
        throw new ClientTransactionsDifferException (message);
      }
    }

    public StateType State
    {
      get
      {
        CheckIfRightTransaction ();
        if (IsDiscarded)
          return StateType.Discarded;
        else
        {
          DataContainer dataContainer = DomainObject.GetDataContainerForTransaction (AssociatedTransaction);
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
        CheckIfRightTransaction ();
        return AssociatedTransaction.DataManager.IsDiscarded (DomainObject.ID); 
      }
    }
  }
}