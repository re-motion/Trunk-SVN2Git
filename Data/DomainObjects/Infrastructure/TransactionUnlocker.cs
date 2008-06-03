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
