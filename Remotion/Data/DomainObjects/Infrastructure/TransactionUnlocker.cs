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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Temporarily makes an inactive <see cref="ClientTransaction"/> writeable. This can destroy the integrity of a <see cref="ClientTransaction"/> 
  /// hierarchy. Use at your own risk.
  /// </summary>
  public struct TransactionUnlocker : IDisposable
  {
    public static IDisposable MakeWriteable (ClientTransaction transaction)
    {
      return new TransactionUnlocker (transaction);
    }

    public static IDisposable MakeWriteableIfRequired (ClientTransaction transaction)
    {
      return transaction.IsActive ? (IDisposable) null : new TransactionUnlocker (transaction);
    }

    private ClientTransaction _transaction;

    private TransactionUnlocker (ClientTransaction transaction)
    {
      if (transaction.IsActive)
      {
        string message = string.Format ("The {0} cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed " 
            + "while its parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.",
            transaction.GetType().Name);
        throw new InvalidOperationException (message);
      }
      transaction.IsActive = true;
      _transaction = transaction;
    }

    public void Dispose ()
    {
      if (_transaction != null)
      {
        Assertion.IsTrue (_transaction.IsActive);
        _transaction.IsActive = false;
        _transaction = null;
      }
    }
  }
}
