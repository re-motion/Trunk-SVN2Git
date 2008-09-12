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

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateLoadingStateTest : TestBase
  {
    [Test]
    public void LoadViewState ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, null, false);
      ControlStateLoadingState state = new ControlStateLoadingState (replacer, MemberCallerMock);

      state.LoadControlState (null);

      Assert.That (replacer.ControlStateModificationState, Is.InstanceOfType (typeof (ControlStateCompletedState)));
      Assert.That (((ControlStateModificationStateBase) replacer.ControlStateModificationState).Replacer, Is.SameAs (replacer));
    }
  }
}