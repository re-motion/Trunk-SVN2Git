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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Represents and tracks a <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  [Serializable]
  public class ClientTransactionHierarchy : IClientTransactionHierarchy
  {
    [NotNull]
    private readonly ClientTransaction _rootTransaction;

    [NotNull]
    private ClientTransaction _leafTransaction;

    [CanBeNull]
    private ClientTransaction _activatedTransaction;

    public ClientTransactionHierarchy (ClientTransaction rootTransaction)
    {
      ArgumentUtility.CheckNotNull ("rootTransaction", rootTransaction);
      _rootTransaction = rootTransaction;
      _leafTransaction = rootTransaction;
    }

    public ClientTransaction RootTransaction
    {
      get { return _rootTransaction; }
    }

    public ClientTransaction LeafTransaction
    {
      get { return _leafTransaction; }
    }

    public ClientTransaction ActiveTransaction
    {
      get { return _activatedTransaction ?? _leafTransaction; }
    }

    public void AppendLeafTransaction (ClientTransaction leafTransaction)
    {
      ArgumentUtility.CheckNotNull ("leafTransaction", leafTransaction);

      if (leafTransaction.ParentTransaction != _leafTransaction)
        throw new ArgumentException ("The new LeafTransaction must have the previous LeafTransaction as its parent.", "leafTransaction");

      _leafTransaction = leafTransaction;
      Assertion.IsTrue (_leafTransaction.RootTransaction == _rootTransaction);
    }

    public void RemoveLeafTransaction ()
    {
      if (_leafTransaction == _rootTransaction)
        throw new InvalidOperationException ("Cannot remove the root transaction.");
      _leafTransaction = _leafTransaction.ParentTransaction;

      Assertion.IsNotNull (_leafTransaction);
    }

    public IDisposable ActivateTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (clientTransaction.RootTransaction != _rootTransaction)
        throw new ArgumentException ("The activated transaction must be from this ClientTransactionHierarchy.", "clientTransaction");

      var previousActivatedTransaction = _activatedTransaction;
      _activatedTransaction = clientTransaction;

      return new ActivationScope (this, clientTransaction, previousActivatedTransaction);
    }

    private sealed class ActivationScope : IDisposable
    {
      private readonly ClientTransactionHierarchy _hierarchy;
      private readonly ClientTransaction _expectedActivatedTransaction;
      private readonly ClientTransaction _previousActivatedTransaction;

      public ActivationScope (
          ClientTransactionHierarchy hierarchy, ClientTransaction expectedActivatedTransaction, ClientTransaction previousActivatedTransaction)
      {
        _hierarchy = hierarchy;
        _expectedActivatedTransaction = expectedActivatedTransaction;
        _previousActivatedTransaction = previousActivatedTransaction;
      }

      public void Dispose ()
      {
        if (_hierarchy._activatedTransaction != _expectedActivatedTransaction)
          throw new InvalidOperationException ("The scopes returned by ActivateTransaction must be disposed inside out.");

        _hierarchy._activatedTransaction = _previousActivatedTransaction;
      }
    }
  }
}