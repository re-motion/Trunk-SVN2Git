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
  public abstract class ExecuteWithPermaUrlStateBase<TParameters> : IExecutionState
    where TParameters : ExecutionStateParameters
  {
    private readonly IExecutionStateContext _executionStateContext;
    private readonly TParameters _parameters;

    protected ExecuteWithPermaUrlStateBase (IExecutionStateContext executionStateContext, TParameters parameters)
    {
      ArgumentUtility.CheckNotNull ("executionStateContext", executionStateContext);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      _executionStateContext = executionStateContext;
      _parameters = parameters;
    }

    public abstract bool ExecuteSubFunction (WxeContext context);
    public abstract void PostProcessSubFunction (WxeContext context);

    public IExecutionStateContext ExecutionStateContext
    {
      get { return _executionStateContext; }
    }

    public TParameters Parameters
    {
      get { return _parameters; }
    }
  }
}
