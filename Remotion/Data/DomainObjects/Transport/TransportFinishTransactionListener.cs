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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Transport
{
  internal class TransportFinishTransactionListener : ClientTransactionListenerBase
  {
    private readonly ClientTransaction _transaction;
    private readonly Func<DomainObject, bool> _filter;

    public TransportFinishTransactionListener (ClientTransaction transaction, Func<DomainObject, bool> filter)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("filter", filter);

      _transaction = transaction;
      _filter = filter;
    }

    public override void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        foreach (var domainObject in domainObjects)
        {
          if (!_filter (domainObject))
          {
            // Note that we do not roll back any end points - this will cause us to create dangling end points. Doesn't matter, though, the transaction
            // is discarded after transport anyway.

            var dataContainer = _transaction.GetDataContainer (domainObject);
            if (dataContainer.State == StateType.New)
            {
              var deleteCommand = _transaction.DataManager.CreateDeleteCommand (domainObject);
              deleteCommand.Perform(); // no events, no bidirectional changes
              Assertion.IsTrue (dataContainer.IsDiscarded);
            }
            else
            {
              dataContainer.RollbackState();
            }
          }
        }
      }
    }
  }
}
