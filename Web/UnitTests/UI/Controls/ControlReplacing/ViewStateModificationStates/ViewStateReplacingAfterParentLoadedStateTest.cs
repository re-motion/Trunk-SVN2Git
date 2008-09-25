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
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
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
      _viewState = new object();
      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy>();
      _replacer = SetupControlReplacer (MemberCallerMock, _testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ViewStateReplacingAfterParentLoadedState (_replacer, MemberCallerMock, _viewState);


      IViewStateModificationState stateStub = MockRepository.GenerateStub<ViewStateModificationStateBase> (_replacer, MemberCallerMock);
      stateStub
          .Stub (stub => stub.AddedControl (Arg<Control>.Is.Anything, Arg<int>.Is.Anything, Arg<Action<Control, int>>.Is.Anything))
          .CallOriginalMethod (OriginalCallOptions.NoExpectation);

      modificationStateSelectionStrategy
          .Stub (stub => stub.CreateViewStateModificationState (Arg.Is (_replacer), Arg<IInternalControlMemberCaller>.Is.NotNull))
          .Return (stateStub);
      modificationStateSelectionStrategy
          .Stub (stub => stub.CreateControlStateModificationState (_replacer, MemberCallerMock))
          .Return (new ControlStateLoadingState (_replacer, MemberCallerMock));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void LoadViewState ()
    {
      _replacer.ViewStateModificationState = _state;

      _state.LoadViewState (null);
    }

    [Test]
    public void AddedControl ()
    {
      IAddedControl addedControlMock = MockRepository.GenerateMock<IAddedControl>();
      
      InternalControlMemberCaller memberCaller = new InternalControlMemberCaller();
      MemberCallerMock
          .Stub (stub => stub.GetControlState (Arg<Control>.Is.Anything)).Do ((Func<Control, ControlState>) memberCaller.GetControlState)
          .Repeat.Any();
      MemberCallerMock.Stub (stub => stub.SetCollectionReadOnly (Arg<ControlCollection>.Is.Anything, Arg<string>.Is.Anything))
          .Do ((Func<ControlCollection, string, string>) memberCaller.SetCollectionReadOnly).Repeat.Any();
      
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

      _testPageHolder.Page.SetRequestValueCollection (new NameValueCollection());
      _testPageHolder.PageInvoker.InitRecursive();

      _state.AddedControl (_testPageHolder.NamingContainer, 0, addedControlMock.AddedControl);

      MemberCallerMock.VerifyAllExpectations();
    }
  }
}