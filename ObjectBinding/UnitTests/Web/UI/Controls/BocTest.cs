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
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

public class BocTest
{
  private WcagHelperMock _wcagHelperMock;
  private Page _page;
  private NamingContainerMock _namingContainer;
  private ControlInvoker _invoker;

  public BocTest()
  {
  }
  
  [SetUp]
  public virtual void SetUp()
  {
    _wcagHelperMock = new WcagHelperMock();
    WcagHelper.SetInstance (_wcagHelperMock);

    _page = new Page();

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";
    _page.Controls.Add (_namingContainer);

    _invoker = new ControlInvoker (_namingContainer);
  }

  [TearDown]
  public virtual void TearDown()
  {
    WcagHelper.SetInstance (new WcagHelperMock ());
    HttpContextHelper.SetCurrent (null);
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }

  public Page Page
  {
    get { return _page; }
  }

  public NamingContainerMock NamingContainer
  {
    get { return _namingContainer; }
  }

  public ControlInvoker Invoker
  {
    get { return _invoker; }
  }
}

}
