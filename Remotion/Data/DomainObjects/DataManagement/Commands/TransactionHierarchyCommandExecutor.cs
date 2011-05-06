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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Provides functionality for executing <see cref="IDataManagementCommand"/> instances on a whole <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  public class TransactionHierarchyCommandExecutor
  {
    private readonly Func<ClientTransaction, IDataManagementCommand> _commandFactory;

    public TransactionHierarchyCommandExecutor (Func<ClientTransaction, IDataManagementCommand> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);
      _commandFactory = commandFactory;
    }

    public bool TryExecuteCommandForTransactionHierarchy (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      return ApplyToTransactionHierarchy (
          clientTransaction,
          delegate (ClientTransaction tx)
          {
            var command = _commandFactory (tx);
            if (!command.CanExecute ())
              return false;

            command.NotifyAndPerform ();
            return true;
          });
    }

    public void ExecuteCommandForTransactionHierarchy (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      ApplyToTransactionHierarchy (
          clientTransaction,
          delegate (ClientTransaction tx)
          {
            var command = _commandFactory (tx);
            command.NotifyAndPerform ();
            return true;
          });
    }

    private bool ApplyToTransactionHierarchy (ClientTransaction clientTransaction, Func<ClientTransaction, bool> operation)
    {
      var currentTransaction = clientTransaction.LeafTransaction;
      bool result = true;
      while (currentTransaction != null && result)
      {
        if (currentTransaction.IsReadOnly)
        {
          using (TransactionUnlocker.MakeWriteable (currentTransaction))
          {
            result = operation (currentTransaction);
          }
        }
        else
          result = operation (currentTransaction);

        currentTransaction = currentTransaction.ParentTransaction;
      }

      return result;
    }
  }
}