// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Contains commonly used get and check methods dealing with <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCheckUtility
  {
    /// <summary>
    /// Returns the <see cref="DomainObject.BindingTransaction"/> of the given <see cref="DomainObject"/>, or the 
    /// <see cref="ClientTransaction.Current"/> if the object has not been bound. If there are neither binding nor current transaction,
    /// an exception is thrown.
    /// </summary>
    /// <param name="domainObject">The domain object to get a transaction for.</param>
    /// <returns><see cref="DomainObject.BindingTransaction"/> or <see cref="ClientTransaction.Current"/>.</returns>
    /// <exception cref="InvalidOperationException">No <see cref="ClientTransaction"/> has been associated with the current thread or this 
    /// <paramref name="domainObject"/>.</exception>
    public static ClientTransaction GetNonNullClientTransaction (DomainObject domainObject)
    {
      ClientTransaction transaction = domainObject.ClientTransaction;
      if (transaction == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread or this object.");
      else
        return transaction;
    }

    /// <summary>
    /// Checks if an object is discarded, and, if yes, throws an <see cref="ObjectDiscardedException"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="transaction">The transaction to check the object against.</param>
    /// <exception cref="ObjectDiscardedException">The object was discarded in the given <see cref="ClientTransaction"/>.</exception>
    public static void CheckIfObjectIsDiscarded (DomainObject domainObject, ClientTransaction transaction)
    {
      if (domainObject.TransactionContext[transaction].IsDiscarded)
        throw new ObjectDiscardedException (domainObject.ID);
    }

    /// <summary>
    /// Checks if the given <see cref="DomainObject"/> can be used in the given transaction, and, if not, throws a <see cref="ClientTransactionsDifferException"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="transaction">The transaction to check the object against.</param>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public static void CheckIfRightTransaction (DomainObject domainObject, ClientTransaction transaction)
    {
      if (!domainObject.TransactionContext[transaction].CanBeUsedInTransaction)
      {
        string message = String.Format (
            "Domain object '{0}' cannot be used in the given transaction as it was loaded or created in another "
            + "transaction. Enter a scope for the transaction, or call EnlistInTransaction to enlist the object "
            + "in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
            domainObject.ID);
        throw new ClientTransactionsDifferException (message);
      }
    }
  }
}
