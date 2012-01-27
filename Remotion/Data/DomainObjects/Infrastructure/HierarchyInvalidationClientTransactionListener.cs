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
  /// Propagates <see cref="StateType.Invalid"/> state for New objects over the <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a new object is created (i.e., its <see cref="DataContainer"/> is registered), it is automatically invalidated in all parent transactions
  /// of the respective transaction.
  /// </para>
  /// <para>
  /// When a new object is discarded (i.e., its <see cref="DataContainer"/> is unregistered), it is automatically invalidated in all subtransactions 
  /// of the respective transaction.
  /// </para>
  /// </remarks>
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

    // TODO 4599: Refactor to use invalidation event: When an object is marked invalid in a transaction, also mark it invalid in all descendant
    // transactions; up to a transaction where the object is already invalid, or data exists for it (e.g., because it is new, or because it is deleted
    // and just about to be discarded by a running Commit operation). Refactor the method above to only mark the root 
    // invalid (unless it is the registering transaction), assert that all intermediate transactions are picked up by this method.
    public override void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("container", container);

      if (container.State == StateType.New)
      {
        foreach (var descendant in clientTransaction.SubTransaction.CreateSequence (tx => tx.SubTransaction))
        {
          var descendantDataContainer = descendant.DataManager.DataContainers[container.ID];
          if (descendantDataContainer != null)
            return;
          
          descendant.DataManager.MarkInvalid (container.DomainObject);
        }
      }
    }
  }
}