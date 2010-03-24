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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides the default implementation of the <see cref="IDomainObjectTransactionContext"/> interface.
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public class DomainObjectTransactionContext : IDomainObjectTransactionContext
  {
    private readonly DomainObject _domainObject;
    private readonly ClientTransaction _associatedTransaction;

    public DomainObjectTransactionContext (DomainObject domainObject, ClientTransaction associatedTransaction)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("associatedTransaction", associatedTransaction);

      _domainObject = domainObject;
      _associatedTransaction = associatedTransaction;
    }

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _associatedTransaction; }
    }

    public StateType State
    {
      get
      {
        DomainObjectCheckUtility.CheckIfRightTransaction (DomainObject, ClientTransaction);

        if (IsDiscarded)
          return StateType.Discarded;

        var dataContainer = ClientTransaction.DataManager.DataContainerMap[DomainObject.ID];
        if (dataContainer == null)
          return StateType.NotLoadedYet;

        if (dataContainer.State == StateType.Unchanged)
          return ClientTransaction.HasRelationChanged (DomainObject) ? StateType.Changed : StateType.Unchanged;

        return dataContainer.State;
      }
    }

    public bool IsDiscarded
    {
      get
      {
        DomainObjectCheckUtility.CheckIfRightTransaction (DomainObject, ClientTransaction);
        return ClientTransaction.DataManager.IsDiscarded (DomainObject.ID); 
      }
    }

    public object Timestamp
    {
      get
      {
        return ClientTransaction.GetDataContainer (DomainObject).Timestamp;
      }
    }

    public void MarkAsChanged()
    {
      DataContainer dataContainer = ClientTransaction.GetDataContainer (DomainObject);
      try
      {
        dataContainer.MarkAsChanged ();
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException ("Only existing DomainObjects can be marked as changed.", ex);
      }
    }

    public void EnsureDataAvailable ()
    {
      DomainObjectCheckUtility.CheckIfRightTransaction (DomainObject, ClientTransaction);

      ClientTransaction.EnsureDataAvailable (DomainObject.ID);

      var dataContainer = ClientTransaction.DataManager.DataContainerMap[DomainObject.ID];
      Assertion.IsNotNull (dataContainer);
      Assertion.IsTrue (dataContainer.DomainObject == DomainObject, "Guaranteed because CheckIfRightTransaction ensures that domainObject is enlisted");

    }

    public T Execute<T> (Func<DomainObject, ClientTransaction, T> func)
    {
      ArgumentUtility.CheckNotNull ("func", func);
      return ClientTransaction.Execute (() => func (_domainObject, _associatedTransaction));
    }

    public void Execute (Action<DomainObject, ClientTransaction> action)
    {
      ArgumentUtility.CheckNotNull ("action", action);
      ClientTransaction.Execute (() => action (_domainObject, _associatedTransaction));
    }
  }
}
