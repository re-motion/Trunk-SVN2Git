/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
    StateType State { get; }

    /// <summary>
    /// Gets a value indicating the discarded status of the object in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    bool IsDiscarded { get; }

    /// <summary>
    /// Marks the <see cref="DomainObject"/> as changed. If the object's previous <see cref="DomainObject.State"/> was <see cref="StateType.Unchanged"/>, it
    /// will be <see cref="StateType.Changed"/> after this method has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is not in state <see cref="StateType.Changed"/> or <see cref="StateType.Unchanged"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    void MarkAsChanged ();
  }
}