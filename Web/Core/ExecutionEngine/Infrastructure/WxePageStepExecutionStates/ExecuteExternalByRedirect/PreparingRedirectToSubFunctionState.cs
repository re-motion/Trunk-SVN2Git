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

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  /// <summary>
  /// The <see cref="PreparingRedirectToSubFunctionState"/> is responsible for evaluating the perma-URL information required for a sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="RedirectingToSubFunctionState"/>.
  /// </summary>
  [Serializable]
  public class PreparingRedirectToSubFunctionState : ExecutionStateBase<PreparingRedirectToSubFunctionStateParameters>
  {
    private readonly WxeReturnOptions _returnOptions;

    public PreparingRedirectToSubFunctionState (IExecutionStateContext executionStateContext, PreparingRedirectToSubFunctionStateParameters parameters, WxeReturnOptions returnOptions)
        : base (executionStateContext, parameters)
    {
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);
      
      _returnOptions = returnOptions;
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      string functionToken = context.GetFunctionTokenForExternalFunction (Parameters.SubFunction, _returnOptions.IsReturning);
      string destinationUrl = context.GetDestinationUrlForExternalFunction (Parameters.SubFunction, functionToken, Parameters.PermaUrlOptions);

      if (_returnOptions.IsReturning)
      {
        NameValueCollection callerUrlParameters = _returnOptions.CallerUrlParameters.Clone();
        callerUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);
        Parameters.SubFunction.ReturnUrl = context.GetPermanentUrl (ExecutionStateContext.CurrentFunction.GetType (), callerUrlParameters);
      }

      ExecutionStateContext.SetExecutionState (
          new RedirectingToSubFunctionState (
              ExecutionStateContext,
              new RedirectingToSubFunctionStateParameters (Parameters.SubFunction, Parameters.PostBackCollection, destinationUrl)));
    }

    public WxeReturnOptions ReturnOptions
    {
      get { return _returnOptions; }
    }
  }
}