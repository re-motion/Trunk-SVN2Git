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
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.Adapters;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class WxeUserControlParentContainerTest
  {
    private HttpContext _httpContext;
    private IInternalControlMemberCaller _memberCallerMock;

    [SetUp]
    public void SetUp ()
    {
      _httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetCurrent (_httpContext);

      _memberCallerMock = MockRepository.GenerateMock<IInternalControlMemberCaller>();
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }


    [Test]
    public void SaveViewStateRecursive ()
    {
      var testPageHolder = new TestPageHolder (true);
      SetupParentContainerForIntegrationTest (testPageHolder.NamingContainer, null);

      testPageHolder.PageInvoker.InitRecursive ();
      testPageHolder.PageInvoker.LoadRecursive ();
      object viewState = testPageHolder.PageInvoker.SaveViewStateRecursive ();

      Assert.That (viewState, Is.InstanceOfType (typeof (Pair)));
      var parentContainerViewState = (Pair) ((IList) ((Pair) viewState).Second)[1];
      Assert.That (parentContainerViewState.First, Is.EqualTo ("value"));
      var namingContainerViewState = (Pair) ((IList) (parentContainerViewState).Second)[1];
      Assert.That (namingContainerViewState.First, Is.EqualTo ("NamingContainerValue"));
      var parentViewState = (Pair) ((IList) (namingContainerViewState).Second)[1];
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive ()
    {
      object viewState = CreateViewState();
      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupParentContainerForIntegrationTest (testPageHolderWithoutState.NamingContainer, null);

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (viewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void SaveAllState_ViewState ()
    {
      var testPageHolder = new TestPageHolder (true);
      WxeUserControlParentContainer parentContainer = SetupParentContainerForIntegrationTest (testPageHolder.NamingContainer, null);
      testPageHolder.PageInvoker.InitRecursive ();

      var formatter = new LosFormatter ();
      var state = (Pair) formatter.Deserialize (parentContainer.SaveAllState ());

      Pair parentContainerViewState = (Pair)state.Second;
      Assert.That (parentContainerViewState.First, Is.EqualTo ("value"));
      var namingContainerViewState = (Pair) ((IList) (parentContainerViewState).Second)[1];
      Assert.That (namingContainerViewState.First, Is.EqualTo ("NamingContainerValue"));
      var parentViewState = (Pair) ((IList) (namingContainerViewState).Second)[1];
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ReplaceViewState ()
    {
      object originalViewState = CreateViewState ();

      var testPageHolderWithChangedState = new TestPageHolder (false);
      var parentContainerWithChangedState = SetupParentContainerForIntegrationTest (testPageHolderWithChangedState.NamingContainer, null);
      testPageHolderWithChangedState.PageInvoker.InitRecursive ();
      testPageHolderWithChangedState.Parent.ValueInViewState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInViewState = "NewNamingContainerValue";
      string backedUpState = parentContainerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupParentContainerForIntegrationTest (testPageHolderWithoutState.NamingContainer, backedUpState);

      testPageHolderWithoutState.Page.SetRequestValueCollection (new NameValueCollection ());
      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (originalViewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NewNamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("NewParentValue"));
    }

    [Test]
    public void WrapControlWithParentContainer_ReplacesControl ()
    {
      var testPageHolder = new TestPageHolder (true);
      WxeUserControlParentContainer parentContainer = new WxeUserControlParentContainer (_memberCallerMock, "TheContainer", null);
      Control control = new Control();
      _memberCallerMock.Stub (stub => stub.GetControlState(control)).Return (ControlState.ChildrenInitialized);

      testPageHolder.Page.Controls.Add (control);
      parentContainer.BeginWrapControlWithParentContainer (control);
      parentContainer.EndWrapControlWithParentContainer (control);

      _memberCallerMock.AssertWasCalled (mock => mock.InitRecursive (parentContainer, testPageHolder.Page));

      Assert.That (testPageHolder.Page.Controls, Is.EqualTo (new Control[] { testPageHolder.NamingContainer, testPageHolder.OtherNamingContainer, parentContainer }));
      Assert.That (parentContainer.Controls, Is.EqualTo (new[] { control }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Controls can only be wrapped during OnInit phase.")]
    public void WrapControlWithParentContainer_ThrowsIfNotInOnInit ()
    {
      var testPageHolder = new TestPageHolder (true);
      WxeUserControlParentContainer parentContainer = new WxeUserControlParentContainer (_memberCallerMock, "TheContainer", null);
      Control control = new Control ();
      _memberCallerMock.Stub (stub => stub.GetControlState(control)).Return (ControlState.Initialized);

      testPageHolder.Page.Controls.Add (control);
      parentContainer.BeginWrapControlWithParentContainer (control);
    }

    private WxeUserControlParentContainer SetupParentContainerForIntegrationTest (NamingContainerMock wrappedControl, string state)
    {
      WxeUserControlParentContainer parentContainer = new WxeUserControlParentContainer (new InternalControlMemberCaller (), "TheContainer", state);
      bool isInitialized = false;
      wrappedControl.Init += delegate
      {
        if (isInitialized)
          return;
        isInitialized = true;
        parentContainer.BeginWrapControlWithParentContainer (wrappedControl);
        parentContainer.EndWrapControlWithParentContainer (wrappedControl);
      };

      return parentContainer;
    }

    private object CreateViewState (TestPageHolder testPageHolder)
    {
      SetupParentContainerForIntegrationTest (testPageHolder.NamingContainer, null);

      testPageHolder.PageInvoker.InitRecursive ();

      return testPageHolder.PageInvoker.SaveViewStateRecursive ();
    }

    private object CreateViewState ()
    {
      return CreateViewState (new TestPageHolder(true));
    }
  }
}