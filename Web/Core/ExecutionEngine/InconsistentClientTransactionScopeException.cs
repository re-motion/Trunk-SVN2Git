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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Thrown when an execution step of a <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/> leaves an incorrect transaction scope.
  /// </summary>
  public class InconsistentClientTransactionScopeException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InconsistentClientTransactionScopeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InconsistentClientTransactionScopeException (string message)
      : base (message)
    {
    }
  }
}
