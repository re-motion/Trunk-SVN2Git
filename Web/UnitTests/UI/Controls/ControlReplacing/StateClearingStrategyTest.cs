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
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class StateClearingStrategyTest : TestBase
  {
    [Test]
    public void LoadControlState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new StateClearingStrategy ();
      var replacer = new ControlReplacer (MemberCallerMock);
      replacer.StateModificationStrategy = stateModificationStrategy;
      replacer.Controls.Add (testPageHolder.NamingContainer);

      MemberCallerMock.Expect (mock => mock.ClearChildControlState (replacer));
      MockRepository.ReplayAll ();

      stateModificationStrategy.LoadControlState (replacer, MemberCallerMock);

      MockRepository.VerifyAll ();
    }

    [Test]
    public void LoadViewState ()
    {
      var testPageHolder = new TestPageHolder (false, RequestMode.PostBack);
      IStateModificationStrategy stateModificationStrategy = new StateClearingStrategy ();
      var replacer = new ControlReplacer (MemberCallerMock);
      replacer.StateModificationStrategy = stateModificationStrategy;
      replacer.Controls.Add (testPageHolder.NamingContainer);
      var controlToReplace = new ControlMock();
      PrivateInvoke.SetNonPublicField (replacer, "_controlToWrap", controlToReplace);

      Assert.That (controlToReplace.EnableViewState, Is.True);

      stateModificationStrategy.LoadViewState (replacer, MemberCallerMock);

      Assert.That (controlToReplace.EnableViewState, Is.False);

      ControlInvoker controlToReplaceInvoker = new ControlInvoker (controlToReplace);
      controlToReplaceInvoker.LoadRecursive ();

      Assert.That (controlToReplace.EnableViewState, Is.True);
    }
  }
}