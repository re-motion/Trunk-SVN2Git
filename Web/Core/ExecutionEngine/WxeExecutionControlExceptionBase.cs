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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This exception is used by the execution engine to control the call stack. </summary>
  [Serializable]
  public abstract class WxeExecutionControlExceptionBase : WxeException
  {
    protected WxeExecutionControlExceptionBase (string message)
        : base(message)
    {
    }

    protected WxeExecutionControlExceptionBase (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}