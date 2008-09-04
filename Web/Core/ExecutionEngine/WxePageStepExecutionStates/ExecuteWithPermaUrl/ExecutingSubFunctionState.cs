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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrl;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrl
{
  public class ExecutingSubFunctionState : ExecutionStateBase<RedirectingToSubFunctionStateParameters>
  {
    public ExecutingSubFunctionState (IExecutionStateContext executionStateContext, RedirectingToSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override bool IsExecuting
    {
      get { return true; }
    }

    public override void PreProcessSubFunction ()
    {
      throw new NotSupportedException ();
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      Parameters.SubFunction.Execute (context);
      ExecutionStateContext.SetExecutionState (new ReturningFromSubFunctionState (ExecutionStateContext, Parameters));
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new NotSupportedException ();
    }
  }
}