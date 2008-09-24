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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;
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
      var modificationStateSelectionStrategy = MockRepository.GenerateStub<IModificationStateSelectionStrategy> ();
      _replacer = SetupControlReplacerForIntegrationTest (_testPageHolder.NamingContainer, modificationStateSelectionStrategy);
      _state = new ViewStateClearingAfterParentLoadedState (_replacer, MemberCallerMock);
      modificationStateSelectionStrategy.Stub (stub => stub.CreateViewStateModificationState (Arg.Is (_replacer), Arg<IInternalControlMemberCaller>.Is.NotNull)).Return (_state);
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
    }
  }
}