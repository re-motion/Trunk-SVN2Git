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
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateLoadingStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ControlStateLoadingState _state;

    public override void SetUp ()
    {
      base.SetUp ();
      _testPageHolder = new TestPageHolder (false);
      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy>();
      _replacer = SetupControlReplacerForIntegrationTest (_testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ControlStateLoadingState (_replacer, MemberCallerMock);
    }

    [Test]
    public void LoadViewState ()
    {
      _state.LoadControlState (null);

      Assert.That (_replacer.ControlStateModificationState, Is.InstanceOfType (typeof (ControlStateCompletedState)));
      Assert.That (((ControlStateModificationStateBase) _replacer.ControlStateModificationState).Replacer, Is.SameAs (_replacer));
    }

    [Test]
    public void AdddedControl ()
    {
      _replacer.ControlStateModificationState = _state;
      _state.AddedControl ();

      Assert.That (_replacer.ControlStateModificationState, Is.SameAs (_state));
    }
  }
}