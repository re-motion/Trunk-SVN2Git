// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
