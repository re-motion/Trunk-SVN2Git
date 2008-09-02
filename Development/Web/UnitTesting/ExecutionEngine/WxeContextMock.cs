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
using System.Web;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  /// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
  public class WxeContextMock : WxeContext
  {
    public static HttpContext CreateHttpContext (NameValueCollection queryString)
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Other.wxe", null);
      context.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetQueryString (context, queryString);
      HttpContextHelper.SetCurrent (context);
      return context;
    }

    public static HttpContext CreateHttpContext ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.ReturnUrl, "/Root.wxe");

      return CreateHttpContext (queryString);
    }

    public WxeContextMock (HttpContext context)
      : base (new HttpContextWrapper (context), new WxeFunctionState (new TestFunction (), false), null)
    {
    }

    public WxeContextMock (HttpContext context, NameValueCollection queryString)
        : base (new HttpContextWrapper (context), new WxeFunctionState (new TestFunction(), false), queryString)
    {
    }
  }
}