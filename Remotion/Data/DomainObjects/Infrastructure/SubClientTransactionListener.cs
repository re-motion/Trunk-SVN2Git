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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;
using System.Linq;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements events that need to be specially handled in the context of sub-transactions.
  /// </summary>
  [Serializable]
  public class SubClientTransactionListener : ClientTransactionListenerBase
  {
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    public SubClientTransactionListener (IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public IInvalidDomainObjectManager ParentInvalidDomainObjectManager
    {
      get { return _parentInvalidDomainObjectManager; }
    }

    public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("container", container);

      if (container.State == StateType.New)
      {
        Assertion.IsTrue (
            clientTransaction.ParentTransaction.CreateSequence (tx => tx.ParentTransaction)
                .All (ancestor => ancestor.DataManager.DataContainerMap[container.DomainObject.ID] == null));
        _parentInvalidDomainObjectManager.MarkInvalidThroughHierarchy (container.DomainObject);
      }
    }
  }
}