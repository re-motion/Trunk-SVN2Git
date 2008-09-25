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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateReplacingStateTest : TestBase
  {
    [Test]
    public void LoadControlState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      var controlState = new Hashtable();
      ControlStateReplacingState state = new ControlStateReplacingState (replacer, MemberCallerMock, controlState);

      state.LoadControlState (null);

      MemberCallerMock.AssertWasCalled (mock => mock.SetChildControlState (replacer, controlState));
      Assert.That (replacer.ControlStateModificationState, Is.InstanceOfType (typeof (ControlStateCompletedState)));
      Assert.That (((ControlStateModificationStateBase) replacer.ControlStateModificationState).Replacer, Is.SameAs (replacer));
    }
  }
}