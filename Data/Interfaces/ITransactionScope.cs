/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Data
{
  /// <summary>
  /// Represents a transaction scope, ie. an execution region where a certain <cref see="ITransaction"/> is the current transaction.
  /// </summary>
  public interface ITransactionScope
  {
    /// <summary>
    /// Gets a flag that describes whether this is the active scope.
    /// </summary>
    bool IsActiveScope { get; }

    /// <summary>
    /// Gets the transaction managed by this scope.
    /// </summary>
    /// <value>The scoped transaction.</value>
    ITransaction ScopedTransaction { get; }

    /// <summary>
    /// Leaves the scope, which means that <see cref="ScopedTransaction"/> is no loner the current transaction. 
    /// </summary>
    /// <remarks>
    /// This method reactivates the scope surrounding this scope. If no surrounding scope exists, there is no current transaction after this method 
    /// is executed.
    /// </remarks>
    void Leave ();
  }
}
