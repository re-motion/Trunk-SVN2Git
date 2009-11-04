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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IDomainObjectTransactionContext
  {
    /// <summary>
    /// Determines whether this instance can be used in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>If this property returns false, <see cref="DomainObjects.ClientTransaction.EnlistDomainObject"/> can be used to enlist the object
    /// in the transaction.</remarks>
    bool CanBeUsedInTransaction { get; }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    StateType State { get; }

    /// <summary>
    /// Gets a value indicating the discarded status of the object in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    bool IsDiscarded { get; }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    object Timestamp { get; }

    /// <summary>
    /// Marks the <see cref="DomainObject"/> as changed. If the object's previous <see cref="DomainObject.State"/> was <see cref="StateType.Unchanged"/>, it
    /// will be <see cref="StateType.Changed"/> after this method has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is not in state <see cref="StateType.Changed"/> or <see cref="StateType.Unchanged"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    void MarkAsChanged ();
  }
}
