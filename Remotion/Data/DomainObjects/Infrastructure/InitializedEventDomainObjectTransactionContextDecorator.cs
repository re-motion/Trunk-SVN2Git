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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an implementation of the <see cref="IDomainObjectTransactionContext"/> interface that is returned while the 
  /// <see cref="DomainObjects.DomainObject.OnReferenceInitialized"/> is run. It does not allow access to properties and methods that read or modify
  /// the state of the <see cref="DomainObject"/> in the associated <see cref="ClientTransaction"/>.
  /// </summary>
  public class InitializedEventDomainObjectTransactionContextDecorator : IDomainObjectTransactionContext
  {
    private readonly IDomainObjectTransactionContext _actualContext;

    public InitializedEventDomainObjectTransactionContextDecorator (IDomainObjectTransactionContext actualContext)
    {
      ArgumentUtility.CheckNotNull ("actualContext", actualContext);

      _actualContext = actualContext;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _actualContext.ClientTransaction; }
    }

    public StateType State
    {
      get
      {
        throw new InvalidOperationException ("While the OnReferenceInitialized event is executing, this member cannot be used.");
      }
    }

    public bool IsDiscarded
    {
      get { return _actualContext.IsDiscarded; }
    }

    public object Timestamp
    {
      get
      {
        throw new InvalidOperationException ("While the OnReferenceInitialized event is executing, this member cannot be used.");
      }
    }

    public void MarkAsChanged()
    {
      throw new InvalidOperationException ("While the OnReferenceInitialized event is executing, this member cannot be used.");
    }

    public void EnsureDataAvailable ()
    {
      throw new InvalidOperationException ("While the OnReferenceInitialized event is executing, this member cannot be used.");
    }

    public T Execute<T> (Func<DomainObject, ClientTransaction, T> func)
    {
      return _actualContext.Execute (func);
    }

    public void Execute (Action<DomainObject, ClientTransaction> action)
    {
      _actualContext.Execute (action);
    }
  }
}
