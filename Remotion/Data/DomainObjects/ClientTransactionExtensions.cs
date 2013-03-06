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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Defines useful extension methods working on <see cref="ClientTransaction"/> instances.
  /// </summary>
  public static class ClientTransactionExtensions
  {
    /// <summary>
    /// Executes the specified delegate in the context of the <see cref="ClientTransaction"/>, returning the result of the delegate. While the
    /// delegate is being executed, the <see cref="ClientTransaction"/> is made the <see cref="ClientTransaction.Current"/> transaction, as if 
    ///  <see cref="ClientTransaction.EnterNonDiscardingScope"/> had been called.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> in whose context to execute the delegate.</param>
    /// <param name="func">The delegate to be executed.</param>
    /// <param name="inactiveTransactionBehavior">Defines what should happen when this <see cref="ClientTransaction"/> is currently not active, e.g., 
    /// due to an active subtransaction. The default behavior is <see cref="InactiveTransactionBehavior.Throw"/>, i.e., to throw an exception.</param>
    /// <returns>The result of <paramref name="func"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// The <paramref name="clientTransaction"/> is not the <see cref="ClientTransaction.ActiveTransaction"/> of the hierarchy and 
    /// <paramref name="inactiveTransactionBehavior"/> is set to <see cref="InactiveTransactionBehavior.Throw"/>.
    /// </exception>
    public static T ExecuteInScope<T> (
        this ClientTransaction clientTransaction,
        Func<T> func,
        InactiveTransactionBehavior inactiveTransactionBehavior = InactiveTransactionBehavior.Throw)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("func", func);

      using (CreateScopeIfRequired (clientTransaction, inactiveTransactionBehavior))
      {
        return func();
      }
    }

    /// <summary>
    /// Executes the specified delegate in the context of the <see cref="ClientTransaction"/>. While the
    /// delegate is being executed, the <see cref="ClientTransaction"/> is made the <see cref="ClientTransaction.Current"/> transaction, as if 
    ///  <see cref="ClientTransaction.EnterNonDiscardingScope"/> had been called.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> in whose context to execute the delegate.</param>
    /// <param name="action">The delegate to be executed.</param>
    /// <param name="inactiveTransactionBehavior">Defines what should happen when this <see cref="ClientTransaction"/> is currently not active, e.g., 
    /// due to an active subtransaction. The default behavior is <see cref="InactiveTransactionBehavior.Throw"/>, i.e., to throw an exception.</param>
    /// <exception cref="InvalidOperationException">
    /// The <paramref name="clientTransaction"/> is not the <see cref="ClientTransaction.ActiveTransaction"/> of the hierarchy and 
    /// <paramref name="inactiveTransactionBehavior"/> is set to <see cref="InactiveTransactionBehavior.Throw"/>.
    /// </exception>
    public static void ExecuteInScope (
        this ClientTransaction clientTransaction,
        Action action,
        InactiveTransactionBehavior inactiveTransactionBehavior = InactiveTransactionBehavior.Throw)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("action", action);

      using (CreateScopeIfRequired (clientTransaction, inactiveTransactionBehavior))
      {
        action();
      }
    }

    private static IDisposable CreateScopeIfRequired (ClientTransaction clientTransaction, InactiveTransactionBehavior inactiveTransactionBehavior)
    {
      if (ClientTransaction.Current == clientTransaction && clientTransaction.ActiveTransaction == clientTransaction)
        return null;

      return clientTransaction.EnterNonDiscardingScope (inactiveTransactionBehavior);
    }

    [Obsolete ("This API has been renamed to ExecuteInScope. (1.13.188.0)", true)]
    public static T Execute<T> (this ClientTransaction clientTransaction, Func<T> func)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This API has been renamed to ExecuteInScope. (1.13.188.0)", true )]
    public static void Execute (this ClientTransaction clientTransaction, Action action)
    {
      throw new NotImplementedException();
    }
  }
}