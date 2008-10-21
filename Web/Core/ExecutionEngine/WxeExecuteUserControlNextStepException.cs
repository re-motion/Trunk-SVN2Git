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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This exception is used by the execution engine to end the execution of a <see cref="WxeUserControlStep"/>. </summary>
  [Serializable]
  public class WxeExecuteUserControlNextStepException : WxeExecutionControlExceptionBase
  {
    private readonly IUserControlExecutor _userControlExecutor;

    public WxeExecuteUserControlNextStepException (IUserControlExecutor userControlExecutor)
      : base (
      "This exception does not indicate an error. It is used to roll back the call stack. "
      + "It is recommended to disable breaking on this exeption type while debugging."
      )
    {
      ArgumentUtility.CheckNotNull ("userControlExecutor", userControlExecutor);
      _userControlExecutor = userControlExecutor;
    }

    protected WxeExecuteUserControlNextStepException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _userControlExecutor = (IUserControlExecutor) info.GetValue ("_userControlExecutor", typeof (IUserControlExecutor));
    }

    public IUserControlExecutor UserControlExecutor
    {
      get { return _userControlExecutor; }
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      base.GetObjectData (info, context);
      
      info.AddValue ("_userControlExecutor", _userControlExecutor);
    }
  }
}