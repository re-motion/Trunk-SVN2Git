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

    [Test]
    public void AdddedControl ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      ControlReplacer replacer = new ControlReplacer  (MemberCallerMock);
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl> ();
      
      ControlStateModificationStateBase state = MockRepository.GenerateStub<ControlStateModificationStateBase> (replacer, MemberCallerMock);
      state.Stub (stub => stub.AddedControl (Arg<Control>.Is.Anything, Arg<int>.Is.Anything, Arg<Action<Control, int>>.Is.Anything))
          .CallOriginalMethod (OriginalCallOptions.NoExpectation);
      
      replacer.ControlStateModificationState = state;

      state.AddedControl (testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      Assert.That (replacer.ControlStateModificationState, Is.SameAs (state));
      addedControlMock.AssertWasCalled (mock => mock.AddedControl (testPageHolder.NamingContainer, 0));
    }
  }
}