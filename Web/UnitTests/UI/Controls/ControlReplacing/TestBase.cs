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
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  public class TestBase
  {
    private HttpContext _httpContext;
    private IInternalControlMemberCaller _memberCallerMock;

    [SetUp]
    public virtual void SetUp ()
    {
      _httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = Encoding.UTF8;
      HttpContextHelper.SetCurrent (_httpContext);

      _memberCallerMock = MockRepository.GenerateMock<IInternalControlMemberCaller>();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    protected ControlReplacer SetupControlReplacerForIntegrationTest (ReplaceableControlMock wrappedControl, string state, bool clearChildState)
    {
      return SetupControlReplacer(new InternalControlMemberCaller(), wrappedControl, clearChildState, state);
    }

    protected ControlReplacer SetupControlReplacer (IInternalControlMemberCaller memberCaller, ReplaceableControlMock wrappedControl, bool clearChildState, string state)
    {
      ControlReplacer replacer = new ControlReplacer (memberCaller) { ID = "TheReplacer" };
      bool isInitialized = false;
      wrappedControl.Init += delegate
      {
        if (isInitialized)
          return;
        isInitialized = true;
        replacer.ReplaceAndWrap (wrappedControl, wrappedControl, clearChildState, state);
      };

      return replacer;
    }

    protected object CreateViewState ()
    {
      return CreateViewState (new TestPageHolder (true));
    }

    protected object CreateViewState (TestPageHolder testPageHolder)
    {
      SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, null, false);

      testPageHolder.PageInvoker.InitRecursive();

      return testPageHolder.PageInvoker.SaveViewStateRecursive();
    }

    protected object CreateControlState ()
    {
      return CreateControlState (new TestPageHolder (true));
    }

    protected object CreateControlState (TestPageHolder testPageHolder)
    {
      SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, null, false);
      testPageHolder.PageInvoker.InitRecursive ();

      testPageHolder.Page.SaveAllState ();

      return testPageHolder.Page.GetPageStatePersister ().ControlState;
    }

    protected IInternalControlMemberCaller MemberCallerMock
    {
      get { return _memberCallerMock; }
    }
  }
}