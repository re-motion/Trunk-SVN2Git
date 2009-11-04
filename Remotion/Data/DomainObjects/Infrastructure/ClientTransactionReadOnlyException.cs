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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Thrown when a client transaction's state is tried to be modified and the ClientTransaction's internal state is set to read-only,
  /// usually because there is an active nested transaction.
  /// </summary>
  public class ClientTransactionReadOnlyException : DomainObjectException
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
