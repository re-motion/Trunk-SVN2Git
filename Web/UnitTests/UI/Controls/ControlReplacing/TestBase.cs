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
using System.Text;
using System.Web;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;
using Rhino.Mocks;
using Assertion=Remotion.Utilities.Assertion;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  public class TestBase
  {
    private IInternalControlMemberCaller _memberCallerMock;
    private MockRepository _mockRepository;

    [SetUp]
    public virtual void SetUp ()
    {
      SetupHttpContext();
      _mockRepository = new MockRepository();
      _memberCallerMock = _mockRepository.StrictMultiMock<IInternalControlMemberCaller> ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    protected void SetupHttpContext ()
    {
      var httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      httpContext.Response.ContentEncoding = Encoding.UTF8;
      HttpContextHelper.SetCurrent (httpContext);
    }

    protected ControlReplacer SetupControlReplacerForIntegrationTest (
        ReplaceableControlMock wrappedControl, IStateModificationStrategy stateModificationStrategy)
    {
      return SetupControlReplacer (new InternalControlMemberCaller(), wrappedControl, stateModificationStrategy);
    }

    protected ControlReplacer SetupControlReplacer (
        IInternalControlMemberCaller memberCaller, ReplaceableControlMock wrappedControl, IStateModificationStrategy stateModificationStrategy)
    {
      ControlReplacer replacer = new ControlReplacer (memberCaller) { ID = "TheReplacer" };
      wrappedControl.OnInitParameters = new Tuple<ControlReplacer, IStateModificationStrategy>  (replacer, stateModificationStrategy);
      return replacer;
    }

    protected object CreateViewState ()
    {
      return CreateViewState (new TestPageHolder (true, RequestMode.Get));
    }

    protected object CreateViewState (TestPageHolder testPageHolder)
    {
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new StateLoadingStrategy());
      testPageHolder.PageInvoker.InitRecursive();
      if (testPageHolder.Page.IsPostBack)
        new ControlInvoker (testPageHolder.NamingContainer).LoadControlState (null);
      Assertion.IsTrue (replacer.Controls.Count == 1);

      return testPageHolder.PageInvoker.SaveViewStateRecursive();
    }

    protected object CreateControlState ()
    {
      return CreateControlState (new TestPageHolder (true, RequestMode.Get));
    }

    protected object CreateControlState (TestPageHolder testPageHolder)
    {
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new StateLoadingStrategy ());
      testPageHolder.PageInvoker.InitRecursive();
      if (testPageHolder.Page.IsPostBack)
        new ControlInvoker (testPageHolder.NamingContainer).LoadControlState (null);
      Assertion.IsTrue (replacer.Controls.Count == 1);
      
      testPageHolder.Page.SaveAllState();

      return testPageHolder.Page.GetPageStatePersister().ControlState;
    }

    protected IInternalControlMemberCaller MemberCallerMock
    {
      get { return _memberCallerMock; }
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }
  }
}