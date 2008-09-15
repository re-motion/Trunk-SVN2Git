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
  public class ControlStateCompletedStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ControlStateCompletedState _state;

    public override void SetUp ()
    {
      base.SetUp ();
      _testPageHolder = new TestPageHolder (false);
      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy> ();
      _replacer = SetupControlReplacerForIntegrationTest (_testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ControlStateCompletedState (_replacer, MemberCallerMock);
    }
    
    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadViewState ()
    {
      _replacer.ControlStateModificationState = _state;
      
      _state.LoadControlState (null);
    }

    [Test]
    public void AdddedControl ()
    {
      _replacer.ControlStateModificationState = _state;
     
      _state.AddedControl (_testPageHolder.NamingContainer);

      Assert.That (_replacer.ControlStateModificationState, Is.SameAs (_state));
    }
  }
}