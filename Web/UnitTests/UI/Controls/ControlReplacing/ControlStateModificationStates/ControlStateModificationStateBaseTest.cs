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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateModificationStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ControlStateModificationStateBase _state;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _testPageHolder = new TestPageHolder (false);

      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy>();
      _replacer = SetupControlReplacer (MemberCallerMock, _testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = MockRepository.GenerateStub<ControlStateModificationStateBase> (_replacer, MemberCallerMock);
      modificationStateSelectionStrategy.Stub (stub => stub.CreateControlStateModificationState (_replacer, MemberCallerMock)).Return (_state);
    }

    [Test]
    public void AdddedControl ()
    {
      _state.Stub (stub => stub.AddedControl (Arg<Control>.Is.Anything, Arg<int>.Is.Anything, Arg<Action<Control, int>>.Is.Anything))
          .CallOriginalMethod (OriginalCallOptions.NoExpectation);
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl>();
      _replacer.ControlStateModificationState = _state;

      _state.AddedControl (_testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      Assert.That (_replacer.ControlStateModificationState, Is.SameAs (_state));
      addedControlMock.AssertWasCalled (mock => mock.AddedControl (_testPageHolder.NamingContainer, 0));
    }
  }
}