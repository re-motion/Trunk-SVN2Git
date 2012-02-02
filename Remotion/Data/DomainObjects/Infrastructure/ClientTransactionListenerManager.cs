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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Manages the <see cref="IClientTransactionListener"/> instances attached to a <see cref="DomainObjects.ClientTransaction"/> instance and
  /// allows clients to raise events for the <see cref="ClientTransaction"/>.
  /// </summary>
  public class ClientTransactionListenerManager : IClientTransactionListenerManager
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRootClientTransactionListener _rootListener;

    public ClientTransactionListenerManager (ClientTransaction clientTransaction, IRootClientTransactionListener rootListener)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("rootListener", rootListener);

      _clientTransaction = clientTransaction;
      _rootListener = rootListener;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IRootClientTransactionListener RootListener
    {
      get { return _rootListener; }
    }

    public IEnumerable<IClientTransactionListener> Listeners
    {
      get { return _rootListener.Listeners; }
    }

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);
      _rootListener.AddListener (listener);
    }

    public void RemoveListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);
      _rootListener.RemoveListener (listener);
    }

    public void RaiseEvent (Action<ClientTransaction, IClientTransactionListener> action)
    {
      ArgumentUtility.CheckNotNull ("action", action);
      action (_clientTransaction, _rootListener);
    }
  }
}