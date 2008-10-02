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
using System.Collections;

namespace Remotion.Data
{
  /// <summary> The <see cref="ITransaction"/> interface provides functionality needed when working with a transaction.</summary>
  public interface ITransaction
  {
    /// <summary> Commits the transaction. </summary>
    void Commit ();

    /// <summary> Rolls the transaction back. </summary>
    void Rollback ();

    /// <summary> 
    ///   Gets a flag that describes whether the transaction supports creating child transactions by invoking
    ///   <see cref="CreateChild"/>.
    /// </summary>
    bool CanCreateChild { get; }

    /// <summary> Creates a new child transaction for the current transaction. </summary>
    /// <returns> 
    ///   A new instance of the of a type implementing <see cref="ITransaction"/> that has the creating transaction
    ///   as a parent.
    /// </returns>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if the method is invoked while <see cref="CanCreateChild"/> is <see langword="false"/>.
    /// </exception>
    ITransaction CreateChild ();

    /// <summary> Allows the transaction to implement clean up logic. </summary>
    /// <remarks> This method is called when the transaction is no longer needed. </remarks>
    void Release ();

    /// <summary> Gets the transaction's parent transaction. </summary>
    /// <value> 
    ///   An instance of the of a type implementing <see cref="ITransaction"/> or <see langword="null"/> if the
    ///   transaction is a root transaction.
    /// </value>
    ITransaction Parent { get; }

    /// <summary>Gets a flag describing whether the transaction is a child transaction.</summary>
    /// <value> <see langword="true"/> if the transaction is a child transaction. </value>
    bool IsChild { get; }

    /// <summary>Gets a flag describing whether the transaction has been changed since the last commit or rollback.</summary>
    /// <value> <see langword="true"/> if the transaction has uncommitted changes. </value>
    bool HasUncommittedChanges { get; }

    /// <summary>Gets a flag describing whether the transaction is in a read-only state.</summary>
    /// <value> <see langword="true"/> if the transaction cannot be modified. </value>
    /// <remarks>Implementations that do not support read-only transactions should always return false.</remarks>
    bool IsReadOnly { get; }

    /// <summary>
    /// Enters a new scope for the given transaction, making it the active transaction while the scope exists.
    /// </summary>
    /// <returns>The scope keeping the transaction active.</returns>
    /// <remarks>The scope must not discard the transaction when it is left.</remarks>
    ITransactionScope EnterScope ();

    /// <summary>Registers the <paramref name="objects"/> with the transaction.</summary>
    /// <param name="objects">The objects to be registered. Must not be <see langword="null" />.</param>
    /// <remarks>If the type of of of the objects is not supported by the transaction, the object must be ignored.</remarks>
    void RegisterObjects (IEnumerable objects);

    /// <summary>Resets the transaction.</summary>
    /// <remarks>Object references must still be valid after the reset.</remarks>
    void Reset ();
  }
}
