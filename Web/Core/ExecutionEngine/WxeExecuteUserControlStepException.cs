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
  /// <summary> 
  /// This exception is used by the execution engine to transfer control of the exution of a <see cref="WxeUserControlStep"/> to the parent <see cref="WxePageStep"/>. 
  /// </summary>
  [Serializable]
  public class WxeExecuteUserControlStepException : WxeExecutionControlExceptionBase
  {
    public WxeExecuteUserControlStepException ()
      : base (
      "This exception does not indicate an error. It is used to transfer control of the call stack. "
      + "It is recommended to disable breaking on this exeption type while debugging."
      )
    {
    }
    
    protected WxeExecuteUserControlStepException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }
}