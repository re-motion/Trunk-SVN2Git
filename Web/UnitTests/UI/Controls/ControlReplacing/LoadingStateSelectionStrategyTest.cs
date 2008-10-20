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
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class LoadingStateSelectionStrategyTest : TestBase
  {
    [Test]
    public void CreateViewStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IStateModificationStrategy stateModificationStrategy = new LoadingStateSelectionStrategy ();

      IViewStateModificationState viewState = stateModificationStrategy.CreateViewStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ViewStateLoadingState)));
      Assert.That (((ViewStateLoadingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ViewStateLoadingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }

    [Test]
    public void CreateControlStateModificationState ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock);
      IStateModificationStrategy stateModificationStrategy = new LoadingStateSelectionStrategy ();

      IControlStateModificationState viewState = stateModificationStrategy.CreateControlStateModificationState (replacer, MemberCallerMock);

      Assert.That (viewState, Is.InstanceOfType (typeof (ControlStateLoadingState)));
      Assert.That (((ControlStateLoadingState) viewState).Replacer, Is.SameAs (replacer));
      Assert.That (((ControlStateLoadingState) viewState).MemberCaller, Is.SameAs (MemberCallerMock));
    }

    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new LoadingStateSelectionStrategy ();
      var replacer = new ControlReplacer (MemberCallerMock);
      replacer.StateModificationStrategy = stateModificationStrategy;
      replacer.Controls.Add (testPageHolder.NamingContainer);

      MockRepository.ReplayAll ();

      stateModificationStrategy.LoadControlState (replacer, MemberCallerMock);

      MockRepository.VerifyAll ();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new LoadingStateSelectionStrategy ();
      var replacer = new ControlReplacer (MemberCallerMock);
      replacer.StateModificationStrategy = stateModificationStrategy;
      replacer.Controls.Add (testPageHolder.NamingContainer);

      MockRepository.ReplayAll ();

      stateModificationStrategy.LoadViewState (replacer, MemberCallerMock);

      MockRepository.VerifyAll ();
    }
  }
}