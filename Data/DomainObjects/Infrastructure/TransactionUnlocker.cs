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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Temporarily makes a read-only transaction writeable.
  /// </summary>
  internal struct TransactionUnlocker : IDisposable
  {
    public static TransactionUnlocker MakeWriteable (ClientTransaction transaction)
    {
      return new TransactionUnlocker (transaction);
    }

    private ClientTransaction _transaction;

    private TransactionUnlocker (ClientTransaction transaction)
    {
      if (!transaction.IsReadOnly)
      {
        string message = string.Format ("The {0} cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed " 
            + "while its parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.",
            transaction.GetType().Name);
        throw new InvalidOperationException (message);
      }
      transaction.IsReadOnly = false;
      _transaction = transaction;
    }

    public void Dispose ()
    {
      if (_transaction != null)
      {
        Assertion.IsFalse (_transaction.IsReadOnly);
        _transaction.IsReadOnly = true;
        _transaction = null;
      }
    }
  }
}
