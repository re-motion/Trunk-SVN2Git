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
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  [TestFixture]
  public class ViewStateReplacingAfterParentLoadedStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ViewStateReplacingAfterParentLoadedState _state;
    private object _viewState;

    public override void SetUp ()
    {
      base.SetUp();

      _testPageHolder = new TestPageHolder (false);
      _replacer = new ControlReplacer (MemberCallerMock);

      _replacer.ViewStateModificationState = MockRepository.GenerateStub<ViewStateModificationStateBase> (_replacer, MemberCallerMock);
      _replacer.ControlStateModificationState = new ControlStateLoadingState (_replacer, MemberCallerMock);

      _replacer.Controls.Add (_testPageHolder.NamingContainer);
      
      _viewState = new object();
      _state = new ViewStateReplacingAfterParentLoadedState (_replacer, MemberCallerMock, _viewState);
      _replacer.ViewStateModificationState = _state;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadViewState ()
    {
      _state.LoadViewState (null);
    }

    [Test]
    public void AddedControl ()
    {
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl>();
      addedControlMock.Expect (mock => mock.AddedControl (_testPageHolder.NamingContainer, 0))
          .Do (
          invocation =>
          {
            Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.False);
            Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
          });

      MemberCallerMock.Expect (mock => mock.LoadViewStateRecursive (_replacer, _viewState))
          .Do (
          invocation =>
          {
            Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.True);
            Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
            Assert.That (_replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateLoadingState)));
            Assert.That (((ViewStateModificationStateBase) _replacer.ViewStateModificationState).Replacer, Is.SameAs (_replacer));
          });

      _state.AddedControl (_testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      MemberCallerMock.VerifyAllExpectations();
    }
  }
}