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

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class ClearingStateSelectionStrategyTest : TestBase
  {
    [Test]
    public void CreateViewStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IModificationStateSelectionStrategy state = new ClearingStateSelectionStrategy();

      IViewStateModificationState viewState = state.CreateViewStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ViewStateClearingState)));
      Assert.That (((ViewStateClearingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ViewStateClearingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }

    [Test]
    public void CreateControlStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IModificationStateSelectionStrategy state = new ClearingStateSelectionStrategy();

      IControlStateModificationState viewState = state.CreateControlStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ControlStateClearingState)));
      Assert.That (((ControlStateClearingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ControlStateClearingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }
  }
}