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
  //TODO: doc
  [Serializable]
  public class PreparingRedirectToSubFunctionState : ExecutionStateBase<PreparingRedirectToSubFunctionStateParameters>
  {
    public PreparingRedirectToSubFunctionState (IExecutionStateContext executionStateContext, PreparingRedirectToSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
      if (!Parameters.PermaUrlOptions.UsePermaUrl)
      {
        throw new ArgumentException (
            string.Format ("The '{0}' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.", GetType().Name), "parameters");
      }
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      string destinationUrl = GetDestinationPermanentUrl (context);
      string resumeUrl = context.GetResumePath();

      ExecutionStateContext.SetExecutionState (
          new RedirectingToSubFunctionState (
              ExecutionStateContext,
              new RedirectingToSubFunctionStateParameters (Parameters.SubFunction, Parameters.PostBackCollection, destinationUrl, resumeUrl)));
    }

    private string GetDestinationPermanentUrl (WxeContext context)
    {
      NameValueCollection urlParameters;
      if (Parameters.PermaUrlOptions.UrlParameters == null)
        urlParameters = Parameters.SubFunction.SerializeParametersForQueryString();
      else
        urlParameters = Parameters.PermaUrlOptions.UrlParameters.Clone();

      urlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

      return context.GetPermanentUrl (Parameters.SubFunction.GetType(), urlParameters, Parameters.PermaUrlOptions.UseParentPermaUrl);
    }
  }
}