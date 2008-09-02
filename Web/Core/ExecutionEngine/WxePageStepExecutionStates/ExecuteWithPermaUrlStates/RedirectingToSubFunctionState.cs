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
  public class RedirectingToSubFunctionState : IWxePageStepExecutionState
  {
    private readonly WxeFunction _subFunction;
    private readonly WxePermaUrlOptions _permaUrlOptions;
    private readonly IWxePageStepExecutionStateContext _executionStateContext;

    public RedirectingToSubFunctionState (IWxePageStepExecutionStateContext executionStateContext, WxeFunction subFunction, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("executionStateContext", executionStateContext);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      _executionStateContext = executionStateContext;
      _subFunction = subFunction;
      _permaUrlOptions = permaUrlOptions;
    }

    public void RedirectToSubFunction (WxeContext context)
    {
      string destinationUrl = GetDestinationPermanentUrl (context);
      string resumeUrl = context.GetResumePath();
      try
      {
        context.HttpContext.Response.Redirect (destinationUrl);
      }
      catch (ThreadAbortException)
      {
        _executionStateContext.SetExecutionState (new ExecutingSubFunctionState (_executionStateContext, _subFunction, resumeUrl));
        throw;
      }
    }

    public void ExecuteSubFunction (WxeContext context)
    {
      throw new InvalidOperationException ();
    }

    public void ReturnFromSubFunction (WxeContext context)
    {
      throw new InvalidOperationException ();
    }

    public void PostProcessSubFunction (WxeContext context)
    {
      throw new InvalidOperationException ();
    }

    public void Cleanup (WxeContext context)
    {
      throw new InvalidOperationException ();
    }

    private string GetDestinationPermanentUrl (WxeContext context)
    {
      NameValueCollection urlParameters;
      if (_permaUrlOptions.UrlParameters == null)
        urlParameters = _subFunction.SerializeParametersForQueryString();
      else
        urlParameters = _permaUrlOptions.UrlParameters.Clone ();

      urlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

      return context.GetPermanentUrl (_subFunction.GetType (), urlParameters, _permaUrlOptions.UseParentPermaUrl);
    }

  }
}