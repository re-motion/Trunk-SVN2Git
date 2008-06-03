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
using System.Collections.Generic;
using System.Text;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides an enumeration to configure <see cref="ClientTransactionScope">ClientTransctionScope's</see> automatic rollback behavior.
  /// </summary>
  public enum AutoRollbackBehavior
  {
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should not perform any automatic operation.
    /// </summary>
    None,
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should automatically call <see cref="ClientTransaction.Rollback"/> at its end when
    /// its transaction holds uncommitted changed.
    /// </summary>
    Rollback,
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should automatically call <see cref="ClientTransaction.Discard"/> at
    /// its end.
    /// </summary>
    Discard
  }
}
