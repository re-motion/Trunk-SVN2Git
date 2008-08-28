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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Development.Web.UnitTesting.AspNetFramework;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class WxeTest
{
  private HttpContext _currentHttpContext;
  private WxeContextMock _currentWxeContext;

  public HttpContext CurrentHttpContext
  {
    get { return _currentHttpContext; }
  }

  public WxeContextMock CurrentWxeContext
  {
    get { return _currentWxeContext; }
  }

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _currentWxeContext = new WxeContextMock (_currentHttpContext);
    WxeContext.SetCurrent (_currentWxeContext);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }
}

}
