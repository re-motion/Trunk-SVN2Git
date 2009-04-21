// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Data.DomainObjects
{
  /// <summary>The <see cref="ClientTransactionFactory"/> is the default implementation of the <see cref="ITransactionFactory"/> interface.</summary>
  [Serializable]
  public class ClientTransactionFactory : ITransactionFactory
  {
    /// <summary>
    /// Creates a new root transaction instance. This instance is not yet managed by a scope.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    public ITransaction CreateRootTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();

      OnTransactionCreated (transaction);

      return transaction.ToITransation ();
    }

    protected virtual void OnTransactionCreated (ClientTransaction transaction)
    {
    }
  }
}