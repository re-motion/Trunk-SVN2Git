/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  public class ExecutingSubFunctionState:IWxePageStepExecutionState
  {
    private readonly IWxePageStepExecutionStateContext _executionStateContext;
    private readonly WxeFunction _subFunction;
    private readonly string _resumeUrl;

    public ExecutingSubFunctionState (IWxePageStepExecutionStateContext executionStateContext, WxeFunction subFunction, string resumeUrl)
    {
      ArgumentUtility.CheckNotNull ("executionStateContext", executionStateContext);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNullOrEmpty ("resumeUrl", resumeUrl);

      _executionStateContext = executionStateContext;
      _subFunction = subFunction;
      _resumeUrl = resumeUrl;
    }

    public void RedirectToSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public void ExecuteSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public void ReturnFromSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public void PostProcessSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public void Cleanup (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public IWxePageStepExecutionStateContext ExecutionStateContext
    {
      get { return _executionStateContext; }
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public string ResumeUrl
    {
      get { return _resumeUrl; }
    }
  }
}