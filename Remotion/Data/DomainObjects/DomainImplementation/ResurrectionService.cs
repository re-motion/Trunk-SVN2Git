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
using System.Linq;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality for resurrecting objects marked as <see cref="StateType.Invalid"/> within a <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Only objects that are invalid within the whole <see cref="ClientTransaction"/> hierarchy can be resurrected. For example, when a 
  /// <see cref="DomainObject"/> is newly created in a subtransaction, it will be marked invalid in the subtransaction's parent transaction. 
  /// If the <see cref="ResurrectInvalidObject"/> API is used to resurrect that <see cref="DomainObject"/>,
  /// it will throw an <see cref="InvalidOperationException"/> (and leave the invalid state in the parent transaction unchanged). Similarly,
  /// <see cref="TryResurrectInvalidObject"/> will return <see langword="false" />.
  /// </para>
  /// <para>
  /// Examples of situations in which a resurrection is not possible:
  /// <list type="bullet">
  /// <item><description>
  /// An object is newly created in a subtransaction (it will be invalid only in the subtransaction's ancestor transactions),
  /// </description></item>
  /// <item><description>
  /// an object is deleted in a parent transaction, or deleted in a subtransaction and then committed to the parent transaction (it will be invalid 
  /// only in the parent transaction's descendant transactions).
  /// </description></item>
  /// </list>
  /// </para>
  /// <para>
  /// Examples of situations in which a resurrection is possible:
  /// <list type="bullet">
  /// <item><description>
  /// An object is newly created, then deleted (or rolled back),
  /// </description></item>
  /// <item><description>
  /// an existing object is deleted in a root transaction, then committed,
  /// </description></item>
  /// <item><description>
  /// an object's data could not be found in the data source (e.g., when calling 
  /// <see cref="LifetimeService.GetObject(Remotion.Data.DomainObjects.ClientTransaction,Remotion.Data.DomainObjects.ObjectID,bool)"/>).
  /// </description></item>
  /// </list>
  /// </para>
  /// </remarks>
  public static class ResurrectionService
  {
    /// <summary>
    /// Resurrects the invalid <see cref="DomainObject"/> with the given <paramref name="objectID"/> in the hierarchy of the given 
    /// <paramref name="clientTransaction"/>, throwing an exception if resurrection is not possible.
    /// </summary>
    /// <param name="clientTransaction">A <see cref="ClientTransaction"/> identifying the hierarchy in which to resurrect the object. The object
    /// is resurrected in all transactions of the hierarchy.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object to resurrect.</param>
    /// <exception cref="InvalidOperationException">The <see cref="DomainObject"/> identified by <paramref name="objectID"/> is not invalid in at 
    /// least one <see cref="ClientTransaction"/> of the transaction hierarchy identified by <paramref name="clientTransaction"/>.</exception>
    public static void ResurrectInvalidObject (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var allTransactions = GetAllTransactions (clientTransaction);
      var blockingTransaction = GetFirstBlockingTransaction (objectID, allTransactions);
      if (blockingTransaction != null)
      {
        var message = string.Format (
            "Cannot resurrect object '{0}' because it is not invalid within the whole transaction hierarchy. In transaction '{1}', the object has "
            + "state '{2}'.",
                objectID,
                blockingTransaction,
                blockingTransaction.GetObjectReference (objectID).TransactionContext[blockingTransaction].State);
        throw new InvalidOperationException (message);
      }

      MarkNotInvalidInAllTransactions (objectID, allTransactions);
    }

    /// <summary>
    /// Resurrects the invalid <see cref="DomainObject"/> with the given <paramref name="objectID"/> in the hierarchy of the given 
    /// <paramref name="clientTransaction"/>, returning a value indicating if the resurrection was successful.
    /// </summary>
    /// <param name="clientTransaction">A <see cref="ClientTransaction"/> identifying the hierarchy in which to resurrect the object. The object
    /// is resurrected in all transactions of the hierarchy.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object to resurrect.</param>
    /// <returns><see langword="true" /> if the resurrection succeeded, <see langword="false" /> otherwise. Resurrection does not succeed if
    /// the <see cref="DomainObject"/> identified by <paramref name="objectID"/> is not invalid in at 
    /// least one <see cref="ClientTransaction"/> of the transaction hierarchy identified by <paramref name="clientTransaction"/>.
    /// </returns>
    public static bool TryResurrectInvalidObject (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var allTransactions = GetAllTransactions (clientTransaction);
      var blockingTransaction = GetFirstBlockingTransaction (objectID, allTransactions);
      if (blockingTransaction != null)
        return false;

      MarkNotInvalidInAllTransactions (objectID, allTransactions);
      return true;
    }

    private static IEnumerable<ClientTransaction> GetAllTransactions (ClientTransaction clientTransaction)
    {
      return clientTransaction.RootTransaction.CreateSequence (tx => tx.SubTransaction);
    }

    private static ClientTransaction GetFirstBlockingTransaction (ObjectID objectID, IEnumerable<ClientTransaction> allTransactions)
    {
      return allTransactions.FirstOrDefault (tx => !tx.IsInvalid (objectID));
    }

    private static void MarkNotInvalidInAllTransactions (ObjectID objectID, IEnumerable<ClientTransaction> allTransactions)
    {
      foreach (var transaction in allTransactions)
        transaction.DataManager.MarkNotInvalid (objectID);
    }
  }
}