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
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  [TestFixture]
  public class ViewStateLoadingStateTest : TestBase
  {
    private TestPageHolder _testPageHolder;
    private ControlReplacer _replacer;
    private ViewStateLoadingState _state;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _testPageHolder = new TestPageHolder (false);

      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy> ();
      _replacer = SetupControlReplacer (MemberCallerMock, _testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ViewStateLoadingState (_replacer, MemberCallerMock);
      modificationStateSelectionStrategy.Stub (stub => stub.CreateViewStateModificationState (_replacer, MemberCallerMock)).Return (_state);
    }

    [Test]
    public void LoadViewState ()
    {
      InternalControlMemberCaller memberCaller = new InternalControlMemberCaller ();
      MemberCallerMock.Stub (stub => stub.GetControlState (Arg<Control>.Is.Anything))
          .Do ((Func<Control, ControlState>) memberCaller.GetControlState).Repeat.Any ();
      MemberCallerMock.Stub (stub => stub.SetCollectionReadOnly (Arg<ControlCollection>.Is.Anything, Arg<string>.Is.Anything))
          .Do ((Func<ControlCollection, string, string>) memberCaller.SetCollectionReadOnly).Repeat.Any ();
      MemberCallerMock.Replay ();

      _testPageHolder.Page.SetRequestValueCollection (new NameValueCollection ());
      _testPageHolder.PageInvoker.InitRecursive ();
      
      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);

      _state.LoadViewState (null);

      MemberCallerMock.VerifyAllExpectations();

      Assert.That (_replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateCompletedState)));
      Assert.That (((ViewStateModificationStateBase) _replacer.ViewStateModificationState).Replacer, Is.SameAs (_replacer));
      Assert.That (_testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (_testPageHolder.Parent.EnableViewState, Is.True);
    }
  }
}