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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Propagates <see cref="StateType.Invalid"/> state for newly created or discarded objects over the <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  [Serializable]
  public class HierarchyInvalidationClientTransactionListener : ClientTransactionListenerBase
  {
    public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("container", container);

      if (container.State == StateType.New)
      {
        foreach (var ancestor in clientTransaction.ParentTransaction.CreateSequence (tx => tx.ParentTransaction))
        {
          Assertion.IsNull (ancestor.DataManager.DataContainers[container.ID]);
          ancestor.DataManager.MarkInvalid (container.DomainObject);
        }
      }
    }
  }
}