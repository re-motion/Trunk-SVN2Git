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
using System.Collections.Specialized;
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

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
      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy> ();
      _replacer = SetupControlReplacerForIntegrationTest (_testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ViewStateClearingAfterParentLoadedState(_replacer, MemberCallerMock);
      
      IViewStateModificationState stateStub = MockRepository.GenerateStub<ViewStateModificationStateBase>(_replacer, MemberCallerMock);
      stateStub
        .Stub (stub => stub.AddedControl (Arg<Control>.Is.Anything, Arg<int>.Is.Anything, Arg<Action<Control, int>>.Is.Anything))
        .CallOriginalMethod (OriginalCallOptions.NoExpectation);
          
      modificationStateSelectionStrategy
          .Stub (stub => stub.CreateViewStateModificationState (Arg.Is (_replacer), Arg<IInternalControlMemberCaller>.Is.NotNull))
          .Return (stateStub);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadViewState ()
    {
      _replacer.ViewStateModificationState = _state;

      _state.LoadViewState (null);
    }

    [Test]
    public void AdddedControl ()
    {
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl> ();
      _testPageHolder.Page.SetRequestValueCollection (new NameValueCollection ());
      _testPageHolder.PageInvoker.InitRecursive ();

      _state.AddedControl (_testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      Assert.That (_replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateCompletedState)));
      Assert.That (((ViewStateModificationStateBase) _replacer.ViewStateModificationState).Replacer, Is.SameAs (_replacer));
      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.False);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
      addedControlMock.AssertWasCalled (mock => mock.AddedControl (_testPageHolder.NamingContainer, 0));

      ControlInvoker namingContainerInvoker = new ControlInvoker (_testPageHolder.NamingContainer);
      namingContainerInvoker.LoadRecursive ();

      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
    }
  }
}