/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  public class PostProcessingSubFunctionState:ExecuteWithPermaUrlStateBase
  {
    public PostProcessingSubFunctionState (IWxePageStepExecutionStateContext executionStateContext, WxeFunction subFunction)
        : base(executionStateContext, subFunction)
    {
    }

    public override void RedirectToSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void ReturnFromSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new System.NotImplementedException();
    }

    public override void Cleanup (WxeContext context)
    {
      throw new System.NotImplementedException();
    }
  }
}