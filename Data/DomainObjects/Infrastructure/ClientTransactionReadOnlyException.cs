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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Thrown when a client transaction's state is tried to be modified and the ClientTransaction's internal state is set to read-only,
  /// usually because there is an active nested transaction.
  /// </summary>
  public class ClientTransactionReadOnlyException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientTransactionReadOnlyException"/> class, specifying an exception message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ClientTransactionReadOnlyException (string message)
        : base (message)
    {
    }
  }
}
