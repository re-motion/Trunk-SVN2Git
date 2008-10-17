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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  [TestFixture]
  public class ViewStateReplacingStateTest : TestBase
  {
    [Test]
    public void LoadViewState_BeforeParentLoaded ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      object viewState = new object();
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      replacer.ViewStateModificationState = new ViewStateLoadingState (replacer, MemberCallerMock);
      replacer.ControlStateModificationState = new ControlStateLoadingState (replacer, MemberCallerMock);
      replacer.Controls.Add (testPageHolder.NamingContainer);

      ViewStateReplacingState state = new ViewStateReplacingState (replacer, MemberCallerMock, viewState);
      replacer.ViewStateModificationState = state;

      MemberCallerMock.Expect (mock => mock.LoadViewStateRecursive (replacer, viewState))
          .Do (
          invocation =>
          {
            Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
            Assert.That (testPageHolder.Parent.EnableViewState, Is.True);
            Assert.That (replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateLoadingState)));
          });

      MockRepository.ReplayAll ();

      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);

      state.LoadViewState (null);

      MemberCallerMock.VerifyAllExpectations();

      Assert.That (replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateLoadingState)));
      Assert.That (((ViewStateModificationStateBase) replacer.ViewStateModificationState).Replacer, Is.SameAs (replacer));
      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.False);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);

      ControlInvoker namingContainerInvoker = new ControlInvoker (testPageHolder.NamingContainer);
      namingContainerInvoker.LoadRecursive();

      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);
    }

    [Test]
    public void LoadViewState_AfterParentLoaded ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      object viewState = new object();
      ViewStateReplacingState state = new ViewStateReplacingState (replacer, MemberCallerMock, viewState);

      state.LoadViewState (null);

      Assert.That (replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateReplacingAfterParentLoadedState)));
      Assert.That (((ViewStateModificationStateBase) replacer.ViewStateModificationState).Replacer, Is.SameAs (replacer));
      Assert.That (((ViewStateReplacingAfterParentLoadedState) replacer.ViewStateModificationState).ViewState, Is.SameAs (viewState));
    }
  }
}