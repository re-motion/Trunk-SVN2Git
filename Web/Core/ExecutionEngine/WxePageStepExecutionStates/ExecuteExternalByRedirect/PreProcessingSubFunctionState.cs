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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [Serializable]
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    public PreProcessingSubFunctionState (IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override bool IsExecuting
    {
      get { return false; }
    }

    public override void PreProcessSubFunction ()
    {
      NameValueCollection postBackCollection = BackupPostBackCollection();

      var parameters = new PreparingSubFunctionStateParameters (
          Parameters.SubFunction, postBackCollection, Parameters.PermaUrlOptions, Parameters.ReturnOptions);
      ExecutionStateContext.SetExecutionState (new PreparingRedirectToSubFunctionState (ExecutionStateContext, parameters));
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      throw new NotSupportedException();
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new NotSupportedException();
    }

    private NameValueCollection BackupPostBackCollection ()
    {
      return Parameters.Page.GetPostBackCollection().Clone();
    }
  }
}