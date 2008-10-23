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
using System.Web;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxePageExecutor : IWxePageExecutor
  {
    public WxePageExecutor ()
    {
    }

    public void ExecutePage (WxeContext context, string page, bool isPostBack)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("page", page);

      string url = page;
      string queryString = context.HttpContext.Request.Url.Query;
      if (!string.IsNullOrEmpty (queryString))
      {
        queryString = queryString.Replace (":", HttpUtility.UrlEncode (":"));
        if (url.Contains ("?"))
          url = url + "&" + queryString.TrimStart ('?');
        else
          url = url + queryString;
      }

      WxeHandler wxeHandlerBackUp = context.HttpContext.Handler as WxeHandler;
      Assertion.IsNotNull (wxeHandlerBackUp, "The HttpHandler must be of type WxeHandler.");
      try
      {
        context.HttpContext.Server.Transfer (url, isPostBack);
      }
      catch (HttpException e)
      {
        Exception unwrappedException = PageUtility.GetUnwrappedExceptionFromHttpException (e);
        if (unwrappedException is WxeExecuteNextStepException)
          return;
        if (unwrappedException is WxeExecuteUserControlNextStepException)
          return;
        throw;
      }
      finally
      {
        context.HttpContext.Handler = wxeHandlerBackUp;
      }
    }
  }
}