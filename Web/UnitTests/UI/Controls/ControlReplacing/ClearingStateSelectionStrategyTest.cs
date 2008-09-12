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
  public class LoadingStateSelectionStrategyTest : TestBase
  {
    [Test]
    public void CreateViewStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IModificationStateSelectionStrategy selectionStrategy = new LoadingStateSelectionStrategy();

      IViewStateModificationState viewState = selectionStrategy.CreateViewStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ViewStateLoadingState)));
      Assert.That (((ViewStateLoadingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ViewStateLoadingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }

    [Test]
    public void CreateControlStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IModificationStateSelectionStrategy selectionStrategy = new LoadingStateSelectionStrategy ();

      IControlStateModificationState viewState = selectionStrategy.CreateControlStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ControlStateLoadingState)));
      Assert.That (((ControlStateLoadingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ControlStateLoadingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }
  }
}