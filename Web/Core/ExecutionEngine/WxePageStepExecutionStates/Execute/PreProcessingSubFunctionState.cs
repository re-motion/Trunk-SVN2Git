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
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [Serializable]
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    public PreProcessingSubFunctionState (IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      Parameters.SubFunction.SetParentStep (ExecutionStateContext.CurrentStep);
      NameValueCollection postBackCollection = BackupPostBackCollection ();

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

    private NameValueCollection BackupPostBackCollection ()
    {
      var postBackCollection = Parameters.Page.GetPostBackCollection ().Clone ();

      if (!Parameters.RepostOptions.SuppressRepost)
      {
        if (Parameters.RepostOptions.UsesEventTarget)
        {
          postBackCollection.Remove (ControlHelper.PostEventSourceID);
          postBackCollection.Remove (ControlHelper.PostEventArgumentID);
        }
        else
          postBackCollection.Remove (Parameters.RepostOptions.Sender.UniqueID);
      }

      return postBackCollection;
    }
  }
}