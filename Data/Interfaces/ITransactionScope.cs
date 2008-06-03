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
  /// Represents a transaction scope, ie. an execution region where a certain <typeparam ref="TTransaction"/> is the current transaction.
  /// </summary>
  /// <typeparam name="TTransaction">The type of transaction held active by the scope.</typeparam>
  public interface ITransactionScope<TTransaction>
      where TTransaction : class, ITransaction
  {
    /// <summary>
    /// Gets the transaction managed by this scope.
    /// </summary>
    /// <value>The scoped transaction.</value>
    TTransaction ScopedTransaction { get; }

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
