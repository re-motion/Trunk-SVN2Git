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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [Serializable]
  public class ReturningFromSubFunctionState : ExecutionStateBase<RedirectingToSubFunctionStateParameters>
  {
    public ReturningFromSubFunctionState (IExecutionStateContext executionStateContext, RedirectingToSubFunctionStateParameters parameters)
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

      try
      {
        context.HttpContext.Response.Redirect (Parameters.ResumeUrl);
        throw new InvalidOperationException (string.Format("Redirect to '{0}' failed.", Parameters.ResumeUrl));
      }
      catch (ThreadAbortException)
      {
        ExecutionStateContext.SetExecutionState (new PostProcessingSubFunctionState (ExecutionStateContext, Parameters));
        throw;
      }
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new NotSupportedException ();
    }
  }
}