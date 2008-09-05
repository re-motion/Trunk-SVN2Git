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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [Serializable]
  public abstract class PreProcessingSubFunctionStateBase<TParameters> : ExecutionStateBase<TParameters>
      where TParameters: PreProcessingSubFunctionStateParameters
  {
    protected PreProcessingSubFunctionStateBase (IExecutionStateContext executionStateContext, TParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override sealed bool IsExecuting
    {
      get { return false; }
    }

    public override void PreProcessSubFunction ()
    {
      Parameters.SubFunction.SetParentStep (Parameters.ParentStep);
      NameValueCollection postBackCollection = BackupPostBackCollection();

      if (Parameters.PermaUrlOptions.UsePermaUrl)
      {
        var parameters = new PreparingSubFunctionStateParameters (Parameters.SubFunction, postBackCollection, Parameters.PermaUrlOptions);
        ExecutionStateContext.SetExecutionState (new PreparingRedirectToSubFunctionState (ExecutionStateContext, parameters));
      }
      else
      {
        var parameters = new ExecutionStateParameters (Parameters.SubFunction, postBackCollection);
        ExecutionStateContext.SetExecutionState (new ExecutingSubFunctionWithoutPermaUrlState (ExecutionStateContext, parameters));
      }
    }

    public override sealed void ExecuteSubFunction (WxeContext context)
    {
      throw new NotSupportedException();
    }

    public override sealed void PostProcessSubFunction (WxeContext context)
    {
      throw new NotSupportedException();
    }

    protected virtual NameValueCollection BackupPostBackCollection ()
    {
      return Parameters.Page.GetPostBackCollection().Clone();
    }
  }
}