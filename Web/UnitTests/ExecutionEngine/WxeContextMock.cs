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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

/// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
public class WxeContextMock: WxeContext
{
  public WxeContextMock (HttpContext context)
    : base (context, new WxeFunctionState (new TestFunction (), false), null)
  {
  }

  public WxeContextMock (HttpContext context, NameValueCollection queryString)
    : base (context, new WxeFunctionState (new TestFunction (), false), queryString)
  {
  }
}

}
