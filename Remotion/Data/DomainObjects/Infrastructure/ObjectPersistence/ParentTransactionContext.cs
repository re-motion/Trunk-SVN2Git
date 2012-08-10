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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction for <see cref="SubPersistenceStrategy"/>, opening a scope in which modifying operations are supported 
  /// even when the parent transaction is read-only.
  /// </summary>
  [Serializable]
  public class ParentTransactionContext : IParentTransactionContext
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    public ParentTransactionContext (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      if (parentTransaction.IsActive)
      {
        throw new ArgumentException (
            "In order for the parent transaction access to work correctly, the parent transaction needs to be inactive. "
            + "Use ClientTransaction.CreateSubTransaction() to create a subtransaction and automatically set the parent transaction inactive.",
            "parentTransaction");
      }

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public IInvalidDomainObjectManager ParentInvalidDomainObjectManager
    {
      get { return _parentInvalidDomainObjectManager; }
    }

    public IParentTransactionOperations AccessParentTransaction ()
    {
      var scope = _parentTransaction.HierarchyManager.Unlock();
      return new ParentTransactionOperations (_parentTransaction, _parentInvalidDomainObjectManager, scope);
    }
  }
}