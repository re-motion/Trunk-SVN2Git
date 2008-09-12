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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  [TestFixture]
  public class ViewStateLoadingStateTest:TestBase
  {
    [Test]
    public void LoadViewState ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false);

      ControlReplacer replacer = SetupControlReplacer (MemberCallerMock, testPageHolder.NamingContainer, false, null);
      ViewStateLoadingState state = new ViewStateLoadingState (replacer);

      InternalControlMemberCaller memberCaller = new InternalControlMemberCaller ();
      MemberCallerMock.Stub (stub => stub.GetControlState (Arg<Control>.Is.Anything)).Do ((Func<Control, ControlState>) memberCaller.GetControlState).Repeat.Any ();
      MemberCallerMock.Stub (stub => stub.SetCollectionReadOnly (Arg<ControlCollection>.Is.Anything, Arg<string>.Is.Anything)).Do ((Func<ControlCollection, string, string>) memberCaller.SetCollectionReadOnly).Repeat.Any ();
      MemberCallerMock.Replay ();

      testPageHolder.Page.SetRequestValueCollection (new NameValueCollection ());
      testPageHolder.PageInvoker.InitRecursive ();

      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);

      state.LoadViewState (null);

      MemberCallerMock.VerifyAllExpectations ();

      Assert.That (replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateCompletedState)));
      Assert.That (((ViewStateModificationStateBase) replacer.ViewStateModificationState).Replacer, Is.SameAs (replacer));
      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);
    }
  }
}