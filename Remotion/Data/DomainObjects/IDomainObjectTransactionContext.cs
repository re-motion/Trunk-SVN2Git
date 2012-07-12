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
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IDomainObjectTransactionContext
  {
    /// <summary>
    /// Gets the <see cref="ClientTransaction"/> this context is associated with.
    /// </summary>
    /// <value>The client transaction.</value>
    ClientTransaction ClientTransaction { get; }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    StateType State { get; }

    /// <summary>
    /// Gets a value indicating the invalid status of the object in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object becomes invalid see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    bool IsInvalid { get; }

    [Obsolete ("This state is now called Invalid. (1.13.60)", true)]
    bool IsDiscarded { get; }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    object Timestamp { get; }

    /// <summary>
    /// Marks the <see cref="DomainObject"/> as changed. If the object's previous <see cref="DomainObject.State"/> was <see cref="StateType.Unchanged"/>, it
    /// will be <see cref="StateType.Changed"/> after this method has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is not in state <see cref="StateType.Changed"/> or <see cref="StateType.Unchanged"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    void MarkAsChanged ();

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded. If it hasn't, this method causes the object's data to be loaded.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">No data could be loaded for the <see cref="DomainObject"/> because the object was not
    /// found in the data source.</exception>
    void EnsureDataAvailable ();

    // TODO 4920: Doc, Test
    bool TryEnsureDataAvailable ();

    /// <summary>
    /// Executes the specified delegate in the context of the <see cref="ClientTransaction"/> associated with this 
    /// <see cref="IDomainObjectTransactionContext"/>, returning the result of the delegate. While the
    /// delegate is being executed, the <see cref="ClientTransaction"/> is made the <see cref="DomainObjects.ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the delegate.</typeparam>
    /// <param name="func">The delegate to be executed. The delegate gets the <see cref="DomainObject"/> and the <see cref="ClientTransaction"/>
    /// associated with this <see cref="IDomainObjectTransactionContext"/> as its parameters.
    /// </param>
    /// <returns>The result of <paramref name="func"/>.</returns>
    T Execute<T> (Func<DomainObject, ClientTransaction, T> func);

    /// <summary>
    /// Executes the specified delegate in the context of the <see cref="ClientTransaction"/> associated with this 
    /// <see cref="IDomainObjectTransactionContext"/>. While the
    /// delegate is being executed, the <see cref="ClientTransaction"/> is made the <see cref="DomainObjects.ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <param name="action">The delegate to be executed. The delegate gets the <see cref="DomainObject"/> and the <see cref="ClientTransaction"/>
    /// associated with this <see cref="IDomainObjectTransactionContext"/> as its parameters.</param>
    void Execute (Action<DomainObject, ClientTransaction> action);
  }
}
