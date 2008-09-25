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
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  [TestFixture]
  public class ViewStateClearingAfterParentLoadedStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ViewStateClearingAfterParentLoadedState _state;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _testPageHolder = new TestPageHolder (false);

      _replacer = new ControlReplacer (MemberCallerMock);
      _replacer.ViewStateModificationState = MockRepository.GenerateStub<ViewStateModificationStateBase> (_replacer, MemberCallerMock);
      _replacer.ControlStateModificationState = new ControlStateLoadingState (_replacer, MemberCallerMock);
      _replacer.Controls.Add (_testPageHolder.NamingContainer);
      
      _state = new ViewStateClearingAfterParentLoadedState (_replacer, MemberCallerMock);
      _replacer.ViewStateModificationState = _state;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadViewState ()
    {
      _state.LoadViewState (null);
    }

    [Test]
    public void AdddedControl ()
    {
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl>();

      _state.AddedControl (_testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      Assert.That (_replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateCompletedState)));
      Assert.That (((ViewStateModificationStateBase) _replacer.ViewStateModificationState).Replacer, Is.SameAs (_replacer));
      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.False);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
      addedControlMock.AssertWasCalled (mock => mock.AddedControl (_testPageHolder.NamingContainer, 0));

      ControlInvoker namingContainerInvoker = new ControlInvoker (_testPageHolder.NamingContainer);
      namingContainerInvoker.LoadRecursive();

      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
    }
  }
}