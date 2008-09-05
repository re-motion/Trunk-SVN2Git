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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  //TODO: doc
  [Serializable]
  public class PostProcessingSubFunctionState : ExecutionStateBase<ExecutionStateParameters>
  {
    public PostProcessingSubFunctionState (IExecutionStateContext executionStateContext, ExecutionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    //TODO: CleanUp duplication with other PostProcessSubFunction-implemenations
    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      //  Provide the executed sub-function to the executing page
      context.ReturningFunction = Parameters.SubFunction;

      // Correct the PostBack-Sequence number
      Parameters.PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID] = context.PostBackID.ToString();

      //  Provide the backed up postback data to the executing page
      context.PostBackCollection = Parameters.PostBackCollection;

      context.SetIsPostBack (true);
      context.SetIsReturningPostBack (true);

      ExecutionStateContext.SetExecutionState (NullExecutionState.Null);
    }
  }
}