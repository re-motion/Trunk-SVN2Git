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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [Serializable]
  public class RedirectingToSubFunctionState : ExecutionStateBase<PreparingSubFunctionStateParameters>
  {
    public RedirectingToSubFunctionState (IExecutionStateContext executionStateContext, PreparingSubFunctionStateParameters parameters)
      : base (executionStateContext, parameters)
    {
      if (!Parameters.PermaUrlOptions.UsePermaUrl)
      {
        throw new ArgumentException (
            string.Format ("The '{0}' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.", GetType().Name), "parameters");
      }
    }

    public override bool IsExecuting
    {
      get { return true; }
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      string destinationUrl = GetDestinationPermanentUrl (context);
      string resumeUrl = context.GetResumePath();
      try
      {
        context.HttpContext.Response.Redirect (destinationUrl);
        throw new InvalidOperationException (string.Format ("Redirect to '{0}' failed.", destinationUrl));
      }
      catch (ThreadAbortException)
      {
        ExecutionStateContext.SetExecutionState (
            new ExecutingSubFunctionState (
                ExecutionStateContext,
                new ReturningFromSubFunctionStateParameters (Parameters.SubFunction, Parameters.PostBackCollection, resumeUrl)));
        throw;
      }
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new NotSupportedException ();
    }

    private string GetDestinationPermanentUrl (WxeContext context)
    {
      NameValueCollection urlParameters;
      if (Parameters.PermaUrlOptions.UrlParameters == null)
        urlParameters = Parameters.SubFunction.SerializeParametersForQueryString();
      else
        urlParameters = Parameters.PermaUrlOptions.UrlParameters.Clone ();

      urlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

      return context.GetPermanentUrl (Parameters.SubFunction.GetType (), urlParameters, Parameters.PermaUrlOptions.UseParentPermaUrl);
    }
  }
}