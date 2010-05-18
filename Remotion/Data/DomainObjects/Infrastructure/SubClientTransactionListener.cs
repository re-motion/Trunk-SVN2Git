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
  /// Implements events that need to be specially handled in the context of sub-transactions.
  /// </summary>
  [Serializable]
  public class SubClientTransactionListener : ClientTransactionListenerBase
  {
    private readonly SubClientTransaction _clientTransaction;

    public SubClientTransactionListener (SubClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
    }

    public SubClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public override void DataContainerMapRegistering (DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("container", container);

      if (container.State == StateType.New)
      {
        for (var ancestor = _clientTransaction.ParentTransaction; ancestor != null; ancestor = ancestor.ParentTransaction)
        {
          Assertion.IsNull (ancestor.DataManager.DataContainerMap[container.DomainObject.ID]);
          ancestor.DataManager.MarkObjectInvalid (container.DomainObject);
        }
      }
    }
  }
}