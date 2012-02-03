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
using System.Collections.ObjectModel;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  // TODO 3658: Inject event sink
  /// <summary>
  /// Unloads a <see cref="DomainObject"/> instance.
  /// </summary>
  public class UnloadCommand : IDataManagementCommand
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly DomainObject[] _domainObjects;
    private readonly IDataManagementCommand _unloadDataCommand;

    public UnloadCommand (
        ClientTransaction clientTransaction,
        ICollection<DomainObject> domainObjects,
        IDataManagementCommand unloadDataCommand)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull ("unloadDataCommand", unloadDataCommand);

      _clientTransaction = clientTransaction;
      _domainObjects = domainObjects.ToArray();
      _unloadDataCommand = unloadDataCommand;

      if (_domainObjects.Length == 0)
        throw new ArgumentEmptyException ("domainObjects");
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ReadOnlyCollection<DomainObject> DomainObjects
    {
      get { return Array.AsReadOnly (_domainObjects); }
    }

    public IDataManagementCommand UnloadDataCommand
    {
      get { return _unloadDataCommand; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _unloadDataCommand.GetAllExceptions ();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      this.EnsureCanExecute ();

      _clientTransaction.Execute (() => _clientTransaction.ListenerManager.RaiseEvent ((tx, l) => l.ObjectsUnloading (tx, Array.AsReadOnly (_domainObjects))));

      _unloadDataCommand.NotifyClientTransactionOfBegin ();
    }

    public void Begin ()
    {
      this.EnsureCanExecute();

      // 4619: Moved rest to TopClientTransactionListener
      _unloadDataCommand.Begin ();
    }

    public void Perform ()
    {
      this.EnsureCanExecute ();

      _unloadDataCommand.Perform ();
    }

    public void End ()
    {
      this.EnsureCanExecute ();

      // 4619: Moved rest to TopClientTransactionListener
      _unloadDataCommand.End ();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      this.EnsureCanExecute ();

      _unloadDataCommand.NotifyClientTransactionOfEnd ();

      _clientTransaction.Execute (() => _clientTransaction.ListenerManager.RaiseEvent ((tx, l) => l.ObjectsUnloaded (tx, Array.AsReadOnly (_domainObjects))));
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}