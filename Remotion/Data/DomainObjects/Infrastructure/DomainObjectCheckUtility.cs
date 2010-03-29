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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Contains commonly used get and check methods dealing with <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCheckUtility
  {
    /// <summary>
    /// Checks if an object is discarded, and, if yes, throws an <see cref="ObjectDiscardedException"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="transaction">The transaction to check the object against.</param>
    /// <returns>Returns <see langword="true"/> if the method succeeds without throwing an exception. This return value is available so that the 
    /// method can be used from within an expression.</returns>
    /// <exception cref="ObjectDiscardedException">The object was discarded in the given <see cref="ClientTransaction"/>.</exception>
    public static void CheckIfObjectIsDiscarded (DomainObject domainObject, ClientTransaction transaction)
    {
      if (domainObject.TransactionContext[transaction].IsDiscarded)
        throw new ObjectDiscardedException (domainObject.ID);
    }

    /// <summary>
    /// Checks if the given <see cref="DomainObject"/> can be used in the given transaction, and, if not, throws a 
    /// <see cref="ClientTransactionsDifferException"/>. If the method succeeds, <see cref="ClientTransaction.IsEnlisted"/> is guaranteed to be
    /// <see langword="true" /> for the given <see cref="DomainObject"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="transaction">The transaction to check the object against.</param>
    /// <returns>Returns <see langword="true"/> if the method succeeds without throwing an exception. This return value is available so that the 
    /// method can be used from within an expression.</returns>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public static void CheckIfRightTransaction (DomainObject domainObject, ClientTransaction transaction)
    {
      if (!transaction.IsEnlisted (domainObject))
      {
        string message = String.Format (
            "Domain object '{0}' cannot be used in the given transaction as it was loaded or created in another "
            + "transaction. Enter a scope for the transaction, or enlist the object "
            + "in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
            domainObject.ID);
        throw new ClientTransactionsDifferException (message);
      }
    }

    [Obsolete ("This method has been removed. Use DomainObject.DefaultTransactionContext.ClientTransaction instead. (1.13.41)", true)]
    public static ClientTransaction GetNonNullClientTransaction (DomainObject domainObject)
    {
      throw new NotImplementedException ();
    }
  }
}
