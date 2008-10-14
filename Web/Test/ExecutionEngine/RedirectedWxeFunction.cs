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
using System.Web.SessionState;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class RedirectedSubWxeFunction: WxeFunction
  {
    public RedirectedSubWxeFunction ()
      : base (new NoneTransactionMode ())
    {
    }

    void Step1 (WxeContext context)
    {
      context.HttpContext.Response.Redirect ("~/Start.aspx?Redirected");
    }
  }

  public class RedirectedWxeFunction: WxeFunction
  {
    public RedirectedWxeFunction ()
      : base (new NoneTransactionMode ())
    {
    }

    public RedirectedWxeFunction (params object[] args)
        : base (new NoneTransactionMode(), args)
    {
    }

    // steps

    WxeStep Step1 = new RedirectedSubWxeFunction();

    WxeStep Step2 = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
  }
}
