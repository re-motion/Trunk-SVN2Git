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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [Serializable]
  public abstract class ExecuteWithPermaUrlStateBase : IWxePageStepExecutionState
  {
    private readonly IWxePageStepExecutionStateContext _executionStateContext;
    private readonly WxeFunction _subFunction;

    protected ExecuteWithPermaUrlStateBase (IWxePageStepExecutionStateContext executionStateContext, WxeFunction subFunction)
    {
      ArgumentUtility.CheckNotNull ("executionStateContext", executionStateContext);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);

      _executionStateContext = executionStateContext;
      _subFunction = subFunction;
    }

    public abstract void RedirectToSubFunction (WxeContext context);
    public abstract void ExecuteSubFunction (WxeContext context);
    public abstract void ReturnFromSubFunction (WxeContext context);
    public abstract void PostProcessSubFunction (WxeContext context);
    public abstract void Cleanup (WxeContext context);

    public IWxePageStepExecutionStateContext ExecutionStateContext
    {
      get { return _executionStateContext; }
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }
  }
}