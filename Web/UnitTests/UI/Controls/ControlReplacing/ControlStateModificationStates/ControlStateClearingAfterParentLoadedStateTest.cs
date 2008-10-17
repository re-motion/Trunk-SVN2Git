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
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateClearingAfterParentLoadedStateTest : TestBase
  {
    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadControlState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);

      var state = new ControlStateClearingAfterParentLoadedState (replacer, MemberCallerMock);
      replacer.ControlStateModificationState = state;

      state.LoadControlState (null);
    }

    [Test]
    public void AddedControl ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      replacer.ViewStateModificationState = new ViewStateLoadingState (replacer, MemberCallerMock);
      replacer.ControlStateModificationState = MockRepository.GenerateStub<ControlStateModificationStateBase> (replacer, MemberCallerMock);
      replacer.Controls.Add (testPageHolder.NamingContainer);

      IAddedControl addedControlMock = MockRepository.StrictMock<IAddedControl> ();

      var state = new ControlStateClearingAfterParentLoadedState (replacer, MemberCallerMock);
      replacer.ControlStateModificationState = state;

      using (MockRepository.Ordered ())
      {
        MemberCallerMock.Expect (mock => mock.ClearChildControlState (replacer));
        addedControlMock.Expect (mock => mock.AddedControl (testPageHolder.NamingContainer, 0));
      }
      MockRepository.ReplayAll();

      state.AddedControl (testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      MockRepository.VerifyAll();
      Assert.That (replacer.ControlStateModificationState, Is.InstanceOfType (typeof (ControlStateCompletedState)));
      Assert.That (((ControlStateModificationStateBase) replacer.ControlStateModificationState).Replacer, Is.SameAs (replacer));
    }
  }
}