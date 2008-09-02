/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  public interface IWxePageStepExecutionState
  {
    void RedirectToSubFunction (WxeContext context);
    void ExecuteSubFunction (WxeContext context);
    void ReturnFromSubFunction (WxeContext context);
    void PostProcessSubFunction (WxeContext context);
    void Cleanup (WxeContext context);
    
  }

  class WxePageStepExecutionState : IWxePageStepExecutionState
  {
    public void RedirectToSubFunction (WxeContext context)
    {
    }

    public void ExecuteSubFunction (WxeContext context)
    {
    }

    public void ReturnFromSubFunction (WxeContext context)
    {
    }

    public void PostProcessSubFunction (WxeContext context)
    {
    }

    public void Cleanup (WxeContext context)
    {
    }
  }
}