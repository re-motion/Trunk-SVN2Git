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
using System.Text;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.StateModificationStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.StateModificationStates
{
  [TestFixture]
  public class ViewStateClearingStateTest : TestBase
  {
    [Test]
    public void LoadViewState ()
    {
      TestPageHolder testPageHolder = new TestPageHolder (false);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, null, false);
      ViewStateClearingState state = new ViewStateClearingState (replacer);
      testPageHolder.Page.SetRequestValueCollection (new NameValueCollection ());      
      testPageHolder.PageInvoker.InitRecursive();

      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);

      state.LoadViewState(null);

      Assert.That (replacer.ViewStateModificationState, Is.InstanceOfType (typeof (ViewStateCompletedState)));
      Assert.That (((ViewStateModificationStateBase) replacer.ViewStateModificationState).Replacer, Is.SameAs (replacer));
      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.False);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);

      testPageHolder.PageInvoker.LoadRecursive();

      Assert.That (testPageHolder.NamingContainer.EnableViewState, Is.True);
      Assert.That (testPageHolder.Parent.EnableViewState, Is.True);
    }
  }
}